using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("Heart Images")]
    public Image heart1;
    public Image heart2;
    public Image heart3;

    [Header("Heart Sprites")]
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Damage")]
    public float invulnerabilityTime = 1f;
    private bool isInvulnerable = false;

    [Header("Knockback")]
    public float knockbackForce = 2f;
    public float knockbackDuration = 0.10f;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool isKnockedBack = false;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        UpdateHearts();
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        if (isInvulnerable)
            return;

        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        UpdateHearts();

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(InvulnerabilityRoutine());
        StartCoroutine(KnockbackRoutine(hitDirection));
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        UpdateHearts();
    }

    void UpdateHearts()
    {
        if (heart1 != null)
            heart1.sprite = currentHealth >= 1 ? fullHeart : emptyHeart;

        if (heart2 != null)
            heart2.sprite = currentHealth >= 2 ? fullHeart : emptyHeart;

        if (heart3 != null)
            heart3.sprite = currentHealth >= 3 ? fullHeart : emptyHeart;
    }

    IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;

        float timer = 0f;

        if (spriteRenderer != null)
        {
            while (timer < invulnerabilityTime)
            {
                spriteRenderer.enabled = false;
                yield return new WaitForSeconds(0.08f);
                spriteRenderer.enabled = true;
                yield return new WaitForSeconds(0.08f);
                timer += 0.16f;
            }

            spriteRenderer.enabled = true;
        }
        else
        {
            yield return new WaitForSeconds(invulnerabilityTime);
        }

        isInvulnerable = false;
    }

    IEnumerator KnockbackRoutine(Vector2 hitDirection)
    {
        isKnockedBack = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(hitDirection.normalized * knockbackForce, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(knockbackDuration);

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        isKnockedBack = false;
    }

    void Die()
    {
        Debug.Log("Player died");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool IsKnockedBack()
    {
        return isKnockedBack;
    }
}