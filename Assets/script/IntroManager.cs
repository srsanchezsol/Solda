using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroManager : MonoBehaviour
{
    [Header("UI")]
    public Image image;

    [Header("Sprites")]
    public Sprite first;
    public Sprite second;

    [Header("Timing")]
    public float firstDuration = 2f;
    public float secondDuration = 2f;

    [Header("Scene")]
    public string nextSceneName = "solda backup";

    [Header("Skip Options")]
    public bool allowTapToSkip = true;

    private bool isSkipping = false;
    private Coroutine introCoroutine;

    void Start()
    {
        introCoroutine = StartCoroutine(IntroSequence());
    }

    void Update()
    {
        if (!allowTapToSkip || isSkipping)
            return;

        // Touch en Android
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            SkipIntro();
        }

        // Click en PC/editor para pruebas
        if (Input.GetMouseButtonDown(0))
        {
            SkipIntro();
        }
    }

    IEnumerator IntroSequence()
    {
        // Primera imagen
        image.sprite = first;
        yield return new WaitForSeconds(firstDuration);

        if (isSkipping) yield break;

        // Segunda imagen
        image.sprite = second;
        yield return new WaitForSeconds(secondDuration);

        if (isSkipping) yield break;

        LoadNextScene();
    }

    public void SkipIntro()
    {
        if (isSkipping)
            return;

        isSkipping = true;

        if (introCoroutine != null)
            StopCoroutine(introCoroutine);

        LoadNextScene();
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}