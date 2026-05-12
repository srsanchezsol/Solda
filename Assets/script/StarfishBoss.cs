using System.Collections;
using UnityEngine;

public class StarfishBoss : MonoBehaviour, IDamageable
{
    [Header("References")]
    public Transform player;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hurtSfx;
    public AudioClip deathSfx;
    public AudioClip dashSfx;

    [Header("Stats")]
    public int maxHealth = 15;
    public float moveSpeed = 0.25f;
    public int contactDamage = 1;
    public float contactDamageCooldown = 0.7f;

    [Header("Dormant")]
    public bool startsDormant = true;

    [Header("Erratic Movement")]
    public float minMoveTime = 1.2f;
    public float maxMoveTime = 2.6f;
    public float minIdleTime = 0.8f;
    public float maxIdleTime = 2.0f;
    public float directionChangeChance = 0.35f;

    [Header("Dash Attack")]
    public bool canDash = true;
    public float dashCooldown = 3.5f;
    public float dashSpeed = 4.5f;
    public float dashDuration = 0.28f;
    public float dashWindup = 0.35f;
    public Color dashWarningColor = new Color(1f, 0.25f, 0.15f, 1f);

    [Header("Phase 2")]
    public bool hasPhase2 = true;
    public float phase2HealthPercent = 0.5f;
    public Color phase2Color = new Color(1f, 0.1f, 0.05f, 1f);
    public float phase2MoveSpeedMultiplier = 1.4f;
    public float phase2DashCooldownMultiplier = 0.65f;
    public float phase2DashSpeedMultiplier = 1.2f;

    [Header("Damage Flash")]
    public float flashDuration = 0.12f;
    public Color flashColor = Color.white;

    [Header("Death")]
    public float deathDelay = 0.8f;
    public GameObject deathEffectPrefab;

    [Header("Loot")]
    public GameObject specificLootPrefab;
    public Transform lootSpawnPoint;
    public Vector3 lootSpawnOffset = new Vector3(0f, 0.35f, 0f);
    public bool debugLoot = true;

    private int currentHealth;
    private bool isDormant;
    private bool isDead;
    private bool canDealContactDamage = true;
    private bool isDashing = false;
    private bool phase2Active = false;

    private Vector2 moveDirection = Vector2.zero;
    private Coroutine movementRoutine;
    private Coroutine dashRoutine;
    private Color originalColor;

    private float baseMoveSpeed;
    private float baseDashCooldown;
    private float baseDashSpeed;

    private static readonly int IsAwake = Animator.StringToHash("isAwake");
    private static readonly int DieTrigger = Animator.StringToHash("Die");

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        currentHealth = maxHealth;
        isDormant = startsDormant;

        baseMoveSpeed = moveSpeed;
        baseDashCooldown = dashCooldown;
        baseDashSpeed = dashSpeed;

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    private void Start()
    {
        FindPlayerIfNeeded();
        UpdateAnimationState();

        if (!isDormant)
            StartBossBehavior();
        else if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }

    private void FindPlayerIfNeeded()
    {
        if (player != null) return;

        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");

        if (foundPlayer != null)
            player = foundPlayer.transform;
    }

    private void StartBossBehavior()
    {
        if (movementRoutine == null)
            movementRoutine = StartCoroutine(ErraticMovementRoutine());

        if (canDash && dashRoutine == null)
            dashRoutine = StartCoroutine(DashAttackRoutine());
    }

    private void UpdateAnimationState()
    {
        if (animator == null) return;

        animator.SetBool(IsAwake, !isDormant && !isDead);
    }

    private IEnumerator ErraticMovementRoutine()
    {
        while (!isDead)
        {
            if (isDormant || isDashing)
            {
                if (rb != null)
                    rb.linearVelocity = Vector2.zero;

                yield return null;
                continue;
            }

            moveDirection = GetRandomDirection();

            float moveTime = Random.Range(minMoveTime, maxMoveTime);
            float timer = 0f;

            while (timer < moveTime && !isDormant && !isDead && !isDashing)
            {
                if (Random.value < directionChangeChance * Time.deltaTime)
                    moveDirection = GetRandomDirection();

                if (rb != null)
                    rb.linearVelocity = moveDirection * moveSpeed;

                timer += Time.deltaTime;
                yield return null;
            }

            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            float idleTime = Random.Range(minIdleTime, maxIdleTime);
            yield return new WaitForSeconds(idleTime);
        }
    }

    private IEnumerator DashAttackRoutine()
    {
        yield return new WaitForSeconds(1.2f);

        while (!isDead)
        {
            if (isDormant || player == null)
            {
                yield return null;
                continue;
            }

            yield return new WaitForSeconds(dashCooldown);

            if (isDead || isDormant || player == null)
                continue;

            yield return StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        isDashing = true;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (spriteRenderer != null)
            spriteRenderer.color = dashWarningColor;

        PlaySfx(dashSfx);

        yield return new WaitForSeconds(dashWindup);

        if (isDead || isDormant || player == null)
        {
            isDashing = false;
            yield break;
        }

        Vector2 dashDirection = (player.position - transform.position).normalized;

        float timer = 0f;

        while (timer < dashDuration && !isDead)
        {
            if (rb != null)
                rb.linearVelocity = dashDirection * dashSpeed;

            timer += Time.deltaTime;
            yield return null;
        }

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (spriteRenderer != null && !isDead)
            spriteRenderer.color = phase2Active ? phase2Color : originalColor;

        isDashing = false;
    }

    private Vector2 GetRandomDirection()
    {
        Vector2 dir = Random.insideUnitCircle.normalized;

        if (dir == Vector2.zero)
            dir = Vector2.right;

        return dir;
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        if (isDead) return;

        if (isDormant)
            WakeUp();

        currentHealth -= damage;

        PlaySfx(hurtSfx);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        CheckPhase2();
        StartCoroutine(DamageFlashRoutine());
    }

    private void WakeUp()
    {
        isDormant = false;
        UpdateAnimationState();
        StartBossBehavior();
    }

    private void CheckPhase2()
    {
        if (!hasPhase2 || phase2Active) return;

        float healthPercent = (float)currentHealth / maxHealth;

        if (healthPercent <= phase2HealthPercent)
        {
            phase2Active = true;

            moveSpeed = baseMoveSpeed * phase2MoveSpeedMultiplier;
            dashCooldown = baseDashCooldown * phase2DashCooldownMultiplier;
            dashSpeed = baseDashSpeed * phase2DashSpeedMultiplier;

            if (spriteRenderer != null)
                spriteRenderer.color = phase2Color;
        }
    }

    private IEnumerator DamageFlashRoutine()
    {
        if (spriteRenderer == null) yield break;

        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);

        if (!isDead)
            spriteRenderer.color = phase2Active ? phase2Color : originalColor;
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        isDormant = false;
        isDashing = false;

        if (movementRoutine != null)
        {
            StopCoroutine(movementRoutine);
            movementRoutine = null;
        }

        if (dashRoutine != null)
        {
            StopCoroutine(dashRoutine);
            dashRoutine = null;
        }

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        PlaySfx(deathSfx);

        if (animator != null)
        {
            animator.SetBool(IsAwake, false);
            animator.SetTrigger(DieTrigger);
        }

        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
            col.enabled = false;

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        if (deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(deathDelay);

        SpawnLoot();

        Destroy(gameObject);
    }

    private void SpawnLoot()
    {
        if (debugLoot)
            Debug.Log("STARFISH: SpawnLoot called");

        if (specificLootPrefab == null)
        {
            Debug.LogWarning("STARFISH: No specificLootPrefab assigned in Inspector.");
            return;
        }

        Vector3 spawnPos;

        if (lootSpawnPoint != null)
            spawnPos = lootSpawnPoint.position;
        else
            spawnPos = transform.position + lootSpawnOffset;

        GameObject loot = Instantiate(specificLootPrefab, spawnPos, Quaternion.identity);

        loot.SetActive(true);

        SpriteRenderer lootRenderer = loot.GetComponent<SpriteRenderer>();

        if (lootRenderer != null)
        {
            lootRenderer.sortingOrder = 100;
        }

        if (debugLoot)
            Debug.Log("STARFISH: Loot spawned: " + loot.name + " at " + spawnPos);
    }

    private void PlaySfx(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;

        audioSource.PlayOneShot(clip);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead || isDormant || !canDealContactDamage) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                Vector2 hitDirection = (collision.transform.position - transform.position).normalized;
                playerHealth.TakeDamage(contactDamage, hitDirection);
                StartCoroutine(ContactDamageCooldownRoutine());
            }
        }
    }

    private IEnumerator ContactDamageCooldownRoutine()
    {
        canDealContactDamage = false;
        yield return new WaitForSeconds(contactDamageCooldown);
        canDealContactDamage = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.3f);

        Gizmos.color = Color.yellow;

        Vector3 spawnPos = lootSpawnPoint != null ? lootSpawnPoint.position : transform.position + lootSpawnOffset;
        Gizmos.DrawWireSphere(spawnPos, 0.18f);

        if (canDash)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.6f);
        }
    }
}