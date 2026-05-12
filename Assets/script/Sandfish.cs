using UnityEngine;
using System.Collections;

public class SandFish : MonoBehaviour, IDamageable
{
    [Header("References")]
    public Transform player;
    public GameObject visualObject;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public GameObject deathPoofPrefab;
    public GameObject coinPrefab;

    [Header("Detection")]
    public float detectionRange = 1.6f;
    public float emergeDelay = 0.2f;

    [Header("Dash")]
    public float dashSpeed = 6f;
    public float dashDuration = 0.45f;

    [Header("Health")]
    public int maxHealth = 1;

    [Header("Contact Damage")]
    public int contactDamage = 1;
    public float damageCooldown = 0.5f;

    [Header("Hit Flash")]
    public float hitFlashDuration = 0.08f;
    public Color hitFlashColor = Color.red;

    [Header("Drops")]
    public int coinsToDrop = 1;

    [Header("Audio")]
    public AudioClip deathSfx;
    [Range(0f, 1f)] public float deathVolume = 1f;

    private int currentHealth;
    private bool isDead = false;
    private bool isAttacking = false;
    private float lastDamageTime = -10f;
    private Color originalColor;
    private Vector2 dashDirection;

    void Start()
    {
        currentHealth = maxHealth;

        if (visualObject == null && transform.childCount > 0)
            visualObject = transform.GetChild(0).gameObject;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);

        if (animator == null)
            animator = GetComponentInChildren<Animator>(true);

        if (visualObject != null)
            visualObject.SetActive(true);

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            spriteRenderer.enabled = false;
        }

        if (animator != null)
            animator.enabled = false;
    }

    void Update()
    {
        if (isDead || isAttacking || player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= detectionRange)
            StartCoroutine(EmergeAndDash());
    }

    IEnumerator EmergeAndDash()
    {
        isAttacking = true;

        dashDirection = ((Vector2)player.position - (Vector2)transform.position).normalized;

        if (visualObject != null)
            visualObject.SetActive(true);

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;

            if (dashDirection.x > 0.05f)
                spriteRenderer.flipX = false;
            else if (dashDirection.x < -0.05f)
                spriteRenderer.flipX = true;
        }

        if (animator != null)
        {
            animator.enabled = true;
            animator.Rebind();
            animator.Update(0f);
            animator.Play("Attack", 0, 0f);
        }

        yield return new WaitForSeconds(emergeDelay);

        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            transform.position += (Vector3)(dashDirection * dashSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.15f);

        Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isDead || !isAttacking) return;
        if (!other.CompareTag("Player")) return;
        if (Time.time < lastDamageTime + damageCooldown) return;

        PlayerHealth ph = other.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            Vector2 hitDir = ((Vector2)other.transform.position - (Vector2)transform.position).normalized;
            ph.TakeDamage(contactDamage, hitDir);
            lastDamageTime = Time.time;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead || !isAttacking) return;
        if (!collision.gameObject.CompareTag("Player")) return;
        if (Time.time < lastDamageTime + damageCooldown) return;

        PlayerHealth ph = collision.gameObject.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            Vector2 hitDir = ((Vector2)collision.transform.position - (Vector2)transform.position).normalized;
            ph.TakeDamage(contactDamage, hitDir);
            lastDamageTime = Time.time;
        }
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        if (isDead) return;

        currentHealth -= damage;
        StartCoroutine(HitFlash());

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

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
            spriteRenderer.enabled = false;
        }

        if (animator != null)
            animator.enabled = false;

        if (deathPoofPrefab != null)
            Instantiate(deathPoofPrefab, transform.position, Quaternion.identity);

        if (deathSfx != null)
            AudioSource.PlayClipAtPoint(deathSfx, transform.position, deathVolume);

        DropCoins();

        Destroy(gameObject);
    }

    void DropCoins()
    {
        if (coinPrefab == null || coinsToDrop <= 0) return;

        for (int i = 0; i < coinsToDrop; i++)
        {
            Vector3 offset = new Vector3(
                Random.Range(-0.15f, 0.15f),
                Random.Range(-0.15f, 0.15f),
                0f
            );

            Instantiate(coinPrefab, transform.position + offset, Quaternion.identity);
        }
    }
}