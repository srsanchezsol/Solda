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

        animator.SetBool("isWalking", isWalking);
        animator.SetFloat("moveX", moveX);
        animator.SetFloat("moveY", moveY);
        animator.SetFloat("lastMoveX", lastMoveDirection.x);
        animator.SetFloat("lastMoveY", lastMoveDirection.y);

        if (Mathf.Abs(lastMoveDirection.x) > 0.01f)
        {
            spriteRenderer.flipX = lastMoveDirection.x < 0;
        }
    }
}