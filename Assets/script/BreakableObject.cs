using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [Header("Break Effect")]
    public GameObject breakEffect;
    public AudioClip breakSound;

    [Header("Loot")]
    public GameObject blueGemPrefab;
    [Range(0f, 1f)] public float dropChance = 1f;

    private bool isBroken = false;

    public void Break()
    {
        if (isBroken) return;
        isBroken = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        if (breakEffect != null)
        {
            Instantiate(breakEffect, transform.position, Quaternion.identity);
        }

        if (breakSound != null)
        {
            AudioSource.PlayClipAtPoint(breakSound, transform.position);
        }

        if (blueGemPrefab != null && Random.value <= dropChance)
        {
            Instantiate(blueGemPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}