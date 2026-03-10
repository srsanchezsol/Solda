using UnityEngine;

public class LootBounce : MonoBehaviour
{
    public float upwardForce = 2.5f;
    public float sidewaysForce = 1.2f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            float randomX = Random.Range(-sidewaysForce, sidewaysForce);
            Vector2 bounce = new Vector2(randomX, upwardForce);
            rb.linearVelocity = bounce;
        }
    }
}