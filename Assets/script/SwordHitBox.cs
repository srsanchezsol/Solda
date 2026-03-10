using UnityEngine;

public class SwordHitBox : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        BreakableObject breakable = other.GetComponent<BreakableObject>();
        if (breakable != null)
        {
            breakable.Break();
        }

        RatHealth rat = other.GetComponent<RatHealth>();
        if (rat != null)
        {
            Vector2 hitDirection = (rat.transform.position - transform.position).normalized;
            rat.TakeDamage(damage, hitDirection);
        }
    }
}