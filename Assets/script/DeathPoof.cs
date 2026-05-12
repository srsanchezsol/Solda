using UnityEngine;

public class DeathPoof : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        float time = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, time);
    }
}