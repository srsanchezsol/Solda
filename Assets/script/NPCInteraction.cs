using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public GameObject dialogueBox;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
            player.SetCurrentNPC(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
            player.ClearCurrentNPC(this);
    }

    public void Interact()
    {
        if (dialogueBox == null) return;

        if (dialogueBox.activeSelf)
        {
            dialogueBox.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            dialogueBox.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public bool IsDialogueOpen()
    {
        return dialogueBox != null && dialogueBox.activeSelf;
    }
}