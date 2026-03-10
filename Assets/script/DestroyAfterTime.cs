using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    public float destroyTime = 0.6f;

    void Awake()
    {
        Destroy(gameObject, destroyTime);
    }
}