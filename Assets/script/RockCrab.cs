using System.Collections;
using UnityEngine;

public class RockCrab : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private SpriteRenderer crabSprite;
    [SerializeField] private GameObject poofEffectPrefab;
    [SerializeField] private GameObject droppedLootPrefab;

    private Rigidbody2D rb;
    private Collider2D crabCollider;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float stopDistance = 0.9f;
    [SerializeField] private float retreatDistance = 1.2f;
    [SerializeField] private float retreatSpeedMultiplier = 1.15f;

    [Header("Combat")]
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private float damageCooldown = 1f;

    [Header("Health")]
    [SerializeField] private int maxHealth = 2;
    [SerializeField] private float hitFlashDuration = 0.08f;
    [SerializeField] private float damageInvulnerabilityTime = 0.12f;
    [SerializeField] private float hitRetreatDistance = 0.45f;

    private int currentHealth;
    private float lastDamageTime = -999f;
    private float lastHitTime = -999f;
    private bool isRetreating = false;
    private Vector2 retreatTarget;
    private bool isDead = false;
    private Coroutine flashRoutine;
    private Color originalColor = Color.white;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        crabCollider = GetComponent<Collider2D>();

        if (crabSprite == null)
            crabSprite = GetComponent<SpriteRenderer>();

        if (crabSprite != null)
            originalColor = crabSprite.color;

        currentHealth = maxHealth;
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    private void FixedUpdate()
    {
        if (isDead || player == null)
            return;

        Vector2 currentPos = rb.position;

        if (isRetreating)
        {
            Vector2 toRetreatTarget = retreatTarget - currentPos;

            if (toRetreatTarget.magnitude <= 0.05f)
            {
                isRetreating = false;
                rb.linearVelocity = Vector2.zero;
                return;
            }

            Vector2 retreatStep = toRetreatTarget.normalized * moveSpeed * retreatSpeedMultiplier * Time.fixedDeltaTime;
            rb.MovePosition(currentPos + retreatStep);
            return;
        }

        float distanceToPlayer = Vector2.Distance(currentPos, player.position);

        if (distanceToPlayer > chaseRange)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (distanceToPlayer <= stopDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = ((Vector2)player.position - currentPos).normalized;
        rb.MovePosition(currentPos + direction * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamagePlayer(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryDamagePlayer(collision.collider);
    }

    private void TryDamagePlayer(Collider2D other)
    {
        if (isDead)
            return;

        if (!other.CompareTag("Player"))
            return;

        if (Time.time < lastDamageTime + damageCooldown)
            return;

        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable == null)
            return;

        lastDamageTime = Time.time;

        Vector2 hitDirection = ((Vector2)other.transform.position - rb.position).normalized;
        damageable.TakeDamage(contactDamage, hitDirection);

        Vector2 awayFromPlayer = (rb.position - (Vector2)player.position).normalized;
        if (awayFromPlayer == Vector2.zero)
            awayFromPlayer = Vector2.down;

        retreatTarget = rb.position + awayFromPlayer * retreatDistance;
        isRetreating = true;
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        if (isDead)
            return;

        if (Time.time < lastHitTime + damageInvulnerabilityTime)
            return;

        lastHitTime = Time.time;
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        if (hitDirection != Vector2.zero)
        {
            Vector2 pushBack = hitDirection.normalized;
            retreatTarget = rb.position + pushBack * hitRetreatDistance;
            isRetreating = true;
        }

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(HitFlash());
    }

    private IEnumerator HitFlash()
    {
        if (crabSprite == null)
            yield break;

        crabSprite.color = Color.red;
        yield return new WaitForSeconds(hitFlashDuration);
        crabSprite.color = originalColor;
        flashRoutine = null;
    }

    private void Die()
    {
        isDead = true;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (crabCollider != null)
            crabCollider.enabled = false;

        if (poofEffectPrefab != null)
            Instantiate(poofEffectPrefab, transform.position, Quaternion.identity);

        if (droppedLootPrefab != null)
            Instantiate(droppedLootPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}