using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    [Header("Attack")]
    public bool hasSword = false;
    public GameObject swordHitbox;
    public float attackDuration = 0.12f;
    public float attackCooldown = 0.18f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 mobileMovement = Vector2.zero;

    private PlayerAnimator playerAnimator;
    private Animator animator;
    private PlayerHealth playerHealth;

    private bool isAttacking = false;
    private bool canAttack = true;
    private bool mobileAttackPressed = false;

    private Vector2 lastAttackDirection = Vector2.down;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<PlayerAnimator>();
        animator = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();

        if (swordHitbox != null)
            swordHitbox.SetActive(false);
    }

    void Update()
    {
        if (!isAttacking)
        {
            float keyboardX = Input.GetAxisRaw("Horizontal");
            float keyboardY = Input.GetAxisRaw("Vertical");

            movement.x = keyboardX + mobileMovement.x;
            movement.y = keyboardY + mobileMovement.y;

            movement.x = Mathf.Clamp(movement.x, -1f, 1f);
            movement.y = Mathf.Clamp(movement.y, -1f, 1f);

            if (movement.x != 0)
                movement.y = 0;

            if (movement != Vector2.zero)
                lastAttackDirection = movement;
        }
        else
        {
            movement = Vector2.zero;
        }

        if (playerAnimator != null)
            playerAnimator.UpdateAnimation(movement);

        if (hasSword && canAttack && !isAttacking &&
            (Input.GetKeyDown(KeyCode.Space) || mobileAttackPressed))
        {
            mobileAttackPressed = false;
            StartCoroutine(AttackRoutine());
        }
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        if (playerHealth != null && playerHealth.IsKnockedBack())
            return;

        rb.linearVelocity = movement.normalized * speed;
    }

    IEnumerator AttackRoutine()
    {
        if (!hasSword)
            yield break;

        canAttack = false;
        isAttacking = true;

        movement = Vector2.zero;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (playerAnimator != null)
            playerAnimator.SetAttacking(true);

        if (animator != null)
            animator.SetTrigger("attack");

        PositionSwordHitbox();

        if (swordHitbox != null)
            swordHitbox.SetActive(true);

        yield return new WaitForSeconds(attackDuration);

        if (swordHitbox != null)
            swordHitbox.SetActive(false);

        if (playerAnimator != null)
            playerAnimator.SetAttacking(false);

        isAttacking = false;

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    void PositionSwordHitbox()
    {
        if (swordHitbox == null) return;

        Vector3 offset;

        if (Mathf.Abs(lastAttackDirection.x) > Mathf.Abs(lastAttackDirection.y))
            offset = new Vector3(lastAttackDirection.x > 0 ? 0.85f : -0.85f, 0f, 0f);
        else
            offset = new Vector3(0f, lastAttackDirection.y > 0 ? 0.85f : -0.85f, 0f);

        swordHitbox.transform.localPosition = offset;
    }

    public void GetSword()
    {
        hasSword = true;
        Debug.Log("Espada obtenida");
    }

    public void MobileAttack()
    {
        mobileAttackPressed = true;
    }

    public void MoveUp()
    {
        mobileMovement = Vector2.up;
    }

    public void MoveDown()
    {
        mobileMovement = Vector2.down;
    }

    public void MoveLeft()
    {
        mobileMovement = Vector2.left;
    }

    public void MoveRight()
    {
        mobileMovement = Vector2.right;
    }

    public void StopMove()
    {
        mobileMovement = Vector2.zero;
    }
}