using System.Collections.Generic;
using UnityEngine;

public class SwordHitBox : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    private Vector2 attackDirection = Vector2.down;

    private readonly HashSet<IDamageable> hitEnemies = new HashSet<IDamageable>();
    private readonly HashSet<BreakableObject> hitBreakables = new HashSet<BreakableObject>();

    private void OnEnable()
    {
        hitEnemies.Clear();
        hitBreakables.Clear();
    }

    public void SetAttackDirection(Vector2 direction)
    {
        if (direction != Vector2.zero)
            attackDirection = direction;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHit(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryHit(other);
    }

    private void TryHit(Collider2D other)
    {
        if (other == null)
            return;

        // 🔴 ENEMIGOS
        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            if (hitEnemies.Contains(damageable))
                return;

            hitEnemies.Add(damageable);
            damageable.TakeDamage(damage, attackDirection);
            return;
        }

        // 🔵 BREAKABLES (JARRONES)
        BreakableObject breakable = other.GetComponentInParent<BreakableObject>();
        if (breakable != null)
        {
            if (hitBreakables.Contains(breakable))
                return;

            hitBreakables.Add(breakable);
            breakable.Break();
        }
    }
}