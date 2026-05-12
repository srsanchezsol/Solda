using UnityEngine;

public class NPCTrigger : MonoBehaviour
{
    private NPCInteraction npcInteraction;

    void Awake()
    {
        npcInteraction = GetComponent<NPCInteraction>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
            player.SetCurrentNPC(npcInteraction);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
            player.ClearCurrentNPC(npcInteraction);
    }
}