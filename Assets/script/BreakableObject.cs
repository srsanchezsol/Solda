using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [Header("Break Effect")]
    public GameObject breakEffect;

    [Header("Loot")]
    public GameObject blueGemPrefab;
    [Range(0f, 1f)] public float dropChance = 1f;

    private bool isBroken = false;

    public void Break()
    {
        if (isBroken) return;
        isBroken = true;

        if (breakEffect != null)
        {
            Instantiate(breakEffect, transform.position, Quaternion.identity);
        }

        if (blueGemPrefab != null && Random.value <= dropChance)
        {
            Instantiate(blueGemPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}