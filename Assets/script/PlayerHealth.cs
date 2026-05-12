using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
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
    public float knockbackForce = 4f;
    public float knockbackDuration = 0.10f;

    [Header("Respawn")]
    public Transform respawnPoint;
    public float deathDelay = 0.4f;

    [Header("Respawn Camera")]
    public BoxCollider2D respawnCameraBounds;

    [Header("Death FX")]
    public GameObject deathPoofPrefab;
    public ScreenFade screenFade;

    [Header("Death Audio")]
    public AudioSource audioSource;
    public AudioClip deathSfx;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private PlayerController playerController;

    private bool isKnockedBack = false;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();

        if (screenFade == null)
            screenFade = FindFirstObjectByType<ScreenFade>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        UpdateHearts();
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        if (isInvulnerable || isDead)
            return;

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHearts();

        if (currentHealth <= 0)
        {
            StartCoroutine(DieRoutine());
            return;
        }

        StartCoroutine(InvulnerabilityRoutine());
        StartCoroutine(KnockbackRoutine(hitDirection));
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        UpdateHearts();
    }

    void UpdateHearts()
    {
        if (heart1 != null) heart1.sprite = currentHealth >= 1 ? fullHeart : emptyHeart;
        if (heart2 != null) heart2.sprite = currentHealth >= 2 ? fullHeart : emptyHeart;
        if (heart3 != null) heart3.sprite = currentHealth >= 3 ? fullHeart : emptyHeart;
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

    IEnumerator DieRoutine()
    {
        isDead = true;
        isInvulnerable = true;
        isKnockedBack = false;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (playerController != null)
            playerController.enabled = false;

        if (deathPoofPrefab != null)
            Instantiate(deathPoofPrefab, transform.position, Quaternion.identity);

        if (audioSource != null && deathSfx != null)
            audioSource.PlayOneShot(deathSfx);

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        yield return new WaitForSeconds(deathDelay);

        if (screenFade != null)
            yield return StartCoroutine(screenFade.FadeOut());

        Respawn();

        yield return new WaitForSeconds(0.15f);

        if (screenFade != null)
            yield return StartCoroutine(screenFade.FadeIn());
    }

    void Respawn()
    {
        if (respawnPoint != null)
            transform.position = respawnPoint.position;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        CameraFollow cameraFollow = Camera.main != null ? Camera.main.GetComponent<CameraFollow>() : null;

        if (cameraFollow != null)
        {
            if (respawnCameraBounds != null)
                cameraFollow.SetBounds(respawnCameraBounds);

            cameraFollow.ForceSnapToPlayer();
        }

        currentHealth = maxHealth;
        UpdateHearts();

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        if (playerController != null)
            playerController.enabled = true;

        isDead = false;
        isInvulnerable = false;
    }

    public bool IsKnockedBack()
    {
        return isKnockedBack;
    }
}