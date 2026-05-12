using UnityEngine;

public class SimplePatrol : MonoBehaviour
{
    [SerializeField] float speed = 0.4f;
    [SerializeField] float patrolDistance = 2f;

    Vector2 startPosition;
    bool movingRight = true;
    SpriteRenderer sr;

    void Start()
    {
        startPosition = transform.position;
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float move = speed * Time.deltaTime;

        if (movingRight)
        {
            transform.Translate(Vector2.right * move);

            if (transform.position.x >= startPosition.x + patrolDistance)
            {
                movingRight = false;
                sr.flipX = true;
            }
        }
        else
        {
            transform.Translate(Vector2.left * move);

            if (transform.position.x <= startPosition.x - patrolDistance)
            {
                movingRight = true;
                sr.flipX = false;
            }
        }
    }
}