using UnityEngine;

public class TolyDialogue : MonoBehaviour
{
    public GameObject dialogueBox;
    private bool playerNear = false;

    void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            dialogueBox.SetActive(!dialogueBox.activeSelf);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
            dialogueBox.SetActive(false);
        }
    }
}