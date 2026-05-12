using UnityEngine;
using System.Collections;

public class RatMovement : MonoBehaviour, IDamageable
{
    public enum RatVariant
    {
        Normal,
        Fast,
        Lazy
    }

    [Header("Rat Variant")]
    public RatVariant variant;

    [Header("Movement Settings")]
    [Range(0.5f, 5f)] public float speed = 1.5f;
    [Range(0.5f, 5f)] public float distance = 1.5f;
    [Range(0f, 2f)] public float waitTime = 0.5f;

    [Header("Movement Direction")]
    public bool verticalMovement = true;

    [Header("Health")]
    public int maxHealth = 2;

    [Header("Hit Reaction")]
    public float hitKnockbackDistance = 0.22f;
    public float hitKnockbackDuration = 0.08f;
    public float hitStunTime = 0.10f;
    public float hitFlashDuration = 0.08f;
    public Color hitFlashColor = Color.red;

    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public GameObject deathPoofPrefab;
    public GameObject coinPrefab;

    [Header("Drops")]
    public int coinsToDrop = 1;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool movingToTarget = true;

    private Animator animator;
    private Color originalColor;

    private int currentHealth;
    private bool isDead = false;
    private bool isStunned = false;

    private Coroutine flashRoutine;
    private Coroutine knockbackRoutine;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        ApplyVariant();

        startPosition = transform.position;

        if (verticalMovement)
            targetPosition = startPosition + Vector3.up * distance;
        else
            targetPosition = startPosition + Vector3.right * distance;

        currentHealth = maxHealth;

        StartCoroutine(MoveRoutine());
    }

    void ApplyVariant()
    {
        switch (variant)
        {
            case RatVariant.Normal:
                speed = 1.5f;
                waitTime = 0.5f;
                break;

            case RatVariant.Fast:
                speed = 2.5f;
                waitTime = 0.2f;
                break;

            case RatVariant.Lazy:
                speed = 1f;
                waitTime = 1f;
                break;
        }
    }

    IEnumerator MoveRoutine()
    {
        while (!isDead)
        {
            if (isStunned)
            {
                if (animator != null)
                    animator.SetBool("IsWalking", false);

                yield return null;
                continue;
            }

            Vector3 destination = movingToTarget ? targetPosition : startPosition;

            if (animator != null)
            {
                animator.SetBool("IsWalking", true);

                if (verticalMovement)
                {
                    float moveY = destination.y > transform.position.y ? 1f : -1f;
                    animator.SetFloat("MoveY", moveY);
                }
            }

            while (Vector3.Distance(transform.position, destination) > 0.01f)
            {
                if (isDead) yield break;

                if (isStunned)
                {
                    if (animator != null)
                        animator.SetBool("IsWalking", false);

                    yield return null;
                    break;
                }

                transform.position = Vector3.MoveTowards(
                    transform.position,
                    destination,
                    speed * Time.deltaTime
                );

                yield return null;
            }

            if (animator != null)
                animator.SetBool("IsWalking", false);

            if (!isStunned)
                yield return new WaitForSeconds(waitTime);

            movingToTarget = !movingToTarget;
        }
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(HitFlash());

        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine);
        knockbackRoutine = StartCoroutine(HitKnockback(hitDirection));

        if (currentHealth <= 0)
            Die();
    }

    IEnumerator HitFlash()
    {
        if (spriteRenderer == null) yield break;

        spriteRenderer.color = hitFlashColor;
        yield return new WaitForSeconds(hitFlashDuration);

        if (!isDead && spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    IEnumerator HitKnockback(Vector2 hitDirection)
    {
        isStunned = true;

        Vector3 start = transform.position;
        Vector3 end = start + (Vector3)(hitDirection.normalized * hitKnockbackDistance);

        float elapsed = 0f;

        while (elapsed < hitKnockbackDuration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / hitKnockbackDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;

        yield return new WaitForSeconds(hitStunTime);

        isStunned = false;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        StopAllCoroutines();

        if (animator != null)
            animator.SetBool("IsWalking", false);

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        if (deathPoofPrefab != null)
            Instantiate(deathPoofPrefab, transform.position, Quaternion.identity);

        DropCoins();

        Destroy(gameObject);
    }

    void DropCoins()
    {
        if (coinPrefab == null || coinsToDrop <= 0) return;

        for (int i = 0; i < coinsToDrop; i++)
        {
            Vector3 spawnOffset = new Vector3(
                Random.Range(-0.15f, 0.15f),
                Random.Range(-0.15f, 0.15f),
                0f
            );

            Instantiate(coinPrefab, transform.position + spawnOffset, Quaternion.identity);
        }
    }
}