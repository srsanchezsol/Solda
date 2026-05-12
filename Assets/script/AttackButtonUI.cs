using UnityEngine;
using UnityEngine.UI;

public class AttackButtonUI : MonoBehaviour
{
    public Image buttonImage;
    public PlayerController player;

    void Start()
    {
        if (buttonImage == null)
            buttonImage = GetComponent<Image>();
    }

    void Update()
    {
        if (player == null || buttonImage == null) return;

        if (player.hasSword)
        {
            buttonImage.color = Color.white;
        }
        else
        {
            buttonImage.color = new Color(1f, 1f, 1f, 0.4f); // semi transparente
        }
    }
}