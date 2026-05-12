using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 lastMoveDirection = Vector2.down;
    private bool isAttacking = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetAttacking(bool attacking)
    {
        isAttacking = attacking;

        if (animator != null)
            animator.SetBool("isAttacking", attacking);
    }

    public void ForceDirection(Vector2 direction)
    {
        if (direction == Vector2.zero)
            return;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            lastMoveDirection = new Vector2(Mathf.Sign(direction.x), 0f);
        else
            lastMoveDirection = new Vector2(0f, Mathf.Sign(direction.y));

        if (animator != null)
        {
            animator.SetFloat("lastMoveX", lastMoveDirection.x);
            animator.SetFloat("lastMoveY", lastMoveDirection.y);
            animator.SetBool("isWalking", false);
            animator.SetFloat("moveX", 0f);
            animator.SetFloat("moveY", 0f);
        }

        if (Mathf.Abs(lastMoveDirection.x) > 0.01f && spriteRenderer != null)
            spriteRenderer.flipX = lastMoveDirection.x < 0;
    }

    public void UpdateAnimation(Vector2 movement)
    {
        if (isAttacking)
            return;

        bool isWalking = movement.sqrMagnitude > 0.01f;

        float moveX = 0f;
        float moveY = 0f;

        if (isWalking)
        {
            if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
            {
                moveX = Mathf.Sign(movement.x);
                moveY = 0f;
                lastMoveDirection = new Vector2(moveX, 0f);
            }
            else
            {
                moveX = 0f;
                moveY = Mathf.Sign(movement.y);
                lastMoveDirection = new Vector2(0f, moveY);
            }
        }

        if (animator != null)
        {
            animator.SetBool("isWalking", isWalking);
            animator.SetFloat("moveX", moveX);
            animator.SetFloat("moveY", moveY);
            animator.SetFloat("lastMoveX", lastMoveDirection.x);
            animator.SetFloat("lastMoveY", lastMoveDirection.y);
        }

        if (Mathf.Abs(lastMoveDirection.x) > 0.01f && spriteRenderer != null)
            spriteRenderer.flipX = lastMoveDirection.x < 0;
    }
}