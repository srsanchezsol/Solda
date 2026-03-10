using UnityEngine;
using System.Collections;

public class RatMovement : MonoBehaviour
{
    public float speed = 1.5f;
    public float distance = 1.5f;
    public float waitTime = 0.5f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool movingToTarget = true;

    private Animator animator;

    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + Vector3.up * distance; // solo vertical
        animator = GetComponent<Animator>();

        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        while (true)
        {
            Vector3 destination = movingToTarget ? targetPosition : startPosition;

            if (animator != null)
            {
                animator.SetBool("IsWalking", true);

                float moveY = destination.y > transform.position.y ? 1f : -1f;
                animator.SetFloat("MoveY", moveY);
            }

            while (Vector3.Distance(transform.position, destination) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    destination,
                    speed * Time.deltaTime
                );

                yield return null;
            }

            if (animator != null)
            {
                animator.SetBool("IsWalking", false);
            }

            yield return new WaitForSeconds(waitTime);

            movingToTarget = !movingToTarget;
        }
    }
}