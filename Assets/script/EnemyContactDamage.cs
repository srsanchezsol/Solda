using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    public int contactDamage = 1;
    public float damageCooldown = 1f;

    private float lastDamageTime;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (Time.time < lastDamageTime + damageCooldown)
            return;

        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            Vector2 hitDirection = (collision.transform.position - transform.position).normalized;
            playerHealth.TakeDamage(contactDamage, hitDirection);
            lastDamageTime = Time.time;
        }
    }
}