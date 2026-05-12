using UnityEngine;

public class FloorSwitch : MonoBehaviour
{
    public Animator animator;
    public bool activated = false;

    public void Activate()
    {
        if (activated) return;

        activated = true;
        animator.SetTrigger("press");

        Debug.Log("Switch activated!");
    }
}