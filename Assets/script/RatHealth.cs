using UnityEngine;
using System.Collections;

public class RatHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Hit Reaction")]
    public float hitKnockbackDistance = 0.22f;
    public float hitKnockbackDuration = 0.08f;
    public float hitStunTime = 0.10f;
    public float hitFlashDuration = 0.08f;
    public Color hitFlashColor = Color.red;

    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public GameObject deathPoofPrefab;

    [Header("Audio")]
    public AudioClip deathSfx;
    [Range(0f, 1f)] public float deathVolume = 1f;

    [Header("Drops")]
    public GameObject dropPrefab;
    public Transform dropPoint;
    public int dropsToSpawn = 1;
    [Range(0f, 1f)] public float dropChance = 0.7f;

    private Rigidbody2D rb;
    private Color originalColor;

    private bool isDead = false;
    private bool isStunned = false;

    private Coroutine flashRoutine;
    private Coroutine knockbackRoutine;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
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
        {
            Die();
        }
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

        Vector2 start = rb != null ? rb.position : (Vector2)transform.position;
        Vector2 end = start + hitDirection.normalized * hitKnockbackDistance;

        float elapsed = 0f;

        while (elapsed < hitKnockbackDuration)
        {
            Vector2 newPos = Vector2.Lerp(start, end, elapsed / hitKnockbackDuration);

            if (rb != null)
                rb.MovePosition(newPos);
            else
                transform.position = newPos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (rb != null)
            rb.MovePosition(end);
        else
            transform.position = end;

        yield return new WaitForSeconds(hitStunTime);

        isStunned = false;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        StopAllCoroutines();

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        if (deathPoofPrefab != null)
            Instantiate(deathPoofPrefab, transform.position, Quaternion.identity);

        if (deathSfx != null)
            AudioSource.PlayClipAtPoint(deathSfx, transform.position, Mathf.Clamp01(deathVolume));

        DropItem();

        Destroy(gameObject);
    }

    void DropItem()
    {
        if (dropPrefab == null) return;
        if (Random.value > dropChance) return;

        Transform spawnPoint = dropPoint != null ? dropPoint : transform;

        for (int i = 0; i < dropsToSpawn; i++)
        {
            Vector3 offset = new Vector3(
                Random.Range(-0.15f, 0.15f),
                Random.Range(-0.15f, 0.15f),
                0f
            );

            Instantiate(dropPrefab, spawnPoint.position + offset, Quaternion.identity);
        }
    }

    public bool IsStunned()
    {
        return isStunned;
    }
}