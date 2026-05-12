using UnityEngine;
using System.Collections;

public class SwordAttack : MonoBehaviour
{
    [Header("Attack visuals / hitboxes")]
    public GameObject attackRight;
    public GameObject attackLeft;
    public GameObject attackUp;
    public GameObject attackDown;

    [Header("Timing")]
    public float attackDuration = 0.12f;

    private PlayerState playerState;
    private Animator animator;
    private bool isAttacking = false;
    private Coroutine attackCoroutine;

    void Awake()
    {
        playerState = GetComponent<PlayerState>();
        animator = GetComponent<Animator>();

        DisableAllAttacks();
    }

    void Update()
    {
        if (playerState == null || !playerState.hasSword)
            return;

        if (isAttacking)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            attackCoroutine = StartCoroutine(DoAttack());
        }
    }

    IEnumerator DoAttack()
    {
        isAttacking = true;

        if (animator != null)
            animator.SetBool("isAttacking", true);

        DisableAllAttacks();

        GameObject selectedAttack = GetAttackByDirection();

        if (selectedAttack != null)
            selectedAttack.SetActive(true);

        yield return new WaitForSeconds(attackDuration);

        DisableAllAttacks();

        if (animator != null)
            animator.SetBool("isAttacking", false);

        isAttacking = false;
        attackCoroutine = null;
    }

    GameObject GetAttackByDirection()
    {
        if (animator == null)
            return attackDown;

        float lastMoveX = animator.GetFloat("lastMoveX");
        float lastMoveY = animator.GetFloat("lastMoveY");

        // prioridad vertical primero
        if (lastMoveY > 0.1f)
            return attackUp;

        if (lastMoveY < -0.1f)
            return attackDown;

        if (lastMoveX < -0.1f)
            return attackLeft;

        return attackRight;
    }

    void DisableAllAttacks()
    {
        if (attackRight != null) attackRight.SetActive(false);
        if (attackLeft != null) attackLeft.SetActive(false);
        if (attackUp != null) attackUp.SetActive(false);
        if (attackDown != null) attackDown.SetActive(false);
    }
}