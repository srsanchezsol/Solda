using UnityEngine;
using TMPro;
using System.Collections;

public class TypewriterDialogue : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;

    [TextArea(3,5)]
    public string message;

    public float typingSpeed = 0.03f;

    private bool isTyping = false;
    private bool textFinished = false;

    void OnEnable()
    {
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        dialogueText.text = "";
        isTyping = true;
        textFinished = false;

        foreach (char letter in message)
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;
        textFinished = true;
    }

    void Update()
    {
        if (!gameObject.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = message;
                isTyping = false;
                textFinished = true;
            }
            else if (textFinished)
            {
                CloseDialogue();
            }
        }
    }

    void CloseDialogue()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}