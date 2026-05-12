using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    public Image fadeImage;
    public float fadeSpeed = 1.5f;

    private void Awake()
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
    }

    public IEnumerator FadeOut()
    {
        yield return StartCoroutine(FadeTo(1f));
    }

    public IEnumerator FadeIn()
    {
        yield return StartCoroutine(FadeTo(0f));
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        if (fadeImage == null)
            yield break;

        Color c = fadeImage.color;

        while (!Mathf.Approximately(c.a, targetAlpha))
        {
            c.a = Mathf.MoveTowards(c.a, targetAlpha, fadeSpeed * Time.deltaTime);
            fadeImage.color = c;
            yield return null;
        }
    }
}