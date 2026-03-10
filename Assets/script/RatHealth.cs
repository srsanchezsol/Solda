using UnityEngine;
using System.Collections;

public class RatHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Knockback")]
    public float knockbackForce = 4f;
    public float knockbackTime = 0.15f;
    private Rigidbody2D rb;
    private bool isKnockedBack = false;

    [Header("Death")]
    public GameObject deathEffect;
    private bool isDead = false;

    [Header("Loot")]
    public GameObject gemDropPrefab;
    public Transform dropPoint;
    [Range(0f, 1f)] public float dropChance = 0.7f;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        if (!isKnockedBack)
        {
            StartCoroutine(ApplyKnockback(hitDirection));
        }
    }

    IEnumerator ApplyKnockback(Vector2 hitDirection)
    {
        isKnockedBack = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.linearVelocity = hitDirection.normalized * knockbackForce;
        }

        yield return new WaitForSeconds(knockbackTime);

        if (rb != null && !isDead)
        {
            rb.linearVelocity = Vector2.zero;
        }

        isKnockedBack = false;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        if (gemDropPrefab != null && Random.value <= dropChance)
        {
            Transform spawnPoint = dropPoint != null ? dropPoint : transform;
            Instantiate(gemDropPrefab, spawnPoint.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}