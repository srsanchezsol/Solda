using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader instance;
    public Animator animator;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        animator.SetTrigger("FadeOut");
    }

    public void FadeOut()
    {
        animator.SetTrigger("FadeOut");
    }

    public void FadeIn()
    {
        animator.SetTrigger("FadeIn");
    }
}