using UnityEngine;
using System.Collections;

public class SwordPickup : MonoBehaviour
{
    public GameObject dialogueBox;
    public PlayerController playerController;
    public SpriteRenderer playerSpriteRenderer;
    public Animator playerAnimator;

    [Header("Pickup Pose")]
    public Sprite swordPickupSprite;
    public float pickupDuration = 1.0f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip fanfareSfx;

    [Header("World Sword")]
    public GameObject swordWorldObject;

    [Header("Pickup Sword Visual")]
    public SpriteRenderer heldSwordRenderer;

    private bool playerInRange = false;
    private bool pickedUp = false;

    public static SwordPickup currentPickup;

    private Sprite originalSprite;
    private Collider2D pickupCollider;
    private SpriteRenderer worldSwordSpriteRenderer;
    private Collider2D worldSwordCollider;

    void Start()
    {
        if (heldSwordRenderer != null)
            heldSwordRenderer.enabled = false;

        pickupCollider = GetComponent<Collider2D>();

        if (swordWorldObject != null)
        {
            worldSwordSpriteRenderer = swordWorldObject.GetComponent<SpriteRenderer>();
            worldSwordCollider = swordWorldObject.GetComponent<Collider2D>();
        }
        else
        {
            worldSwordSpriteRenderer = GetComponent<SpriteRenderer>();
            worldSwordCollider = GetComponent<Collider2D>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pickedUp) return;
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
        currentPickup = this;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;

        if (currentPickup == this)
            currentPickup = null;
    }

    public void TryPickup()
    {
        if (pickedUp || !playerInRange) return;

        StartCoroutine(PickupRoutine());
    }

    IEnumerator PickupRoutine()
    {
        pickedUp = true;
        playerInRange = false;

        if (currentPickup == this)
            currentPickup = null;

        if (pickupCollider != null)
            pickupCollider.enabled = false;

        if (worldSwordCollider != null)
            worldSwordCollider.enabled = false;

        if (worldSwordSpriteRenderer != null)
            worldSwordSpriteRenderer.enabled = false;

        if (playerController != null)
            playerController.enabled = false;

        if (playerSpriteRenderer != null)
            originalSprite = playerSpriteRenderer.sprite;

        if (playerAnimator != null)
            playerAnimator.enabled = false;

        if (playerSpriteRenderer != null && swordPickupSprite != null)
            playerSpriteRenderer.sprite = swordPickupSprite;

        if (heldSwordRenderer != null)
            heldSwordRenderer.enabled = true;

        if (dialogueBox != null)
            dialogueBox.SetActive(true);

        if (audioSource != null && fanfareSfx != null)
            audioSource.PlayOneShot(fanfareSfx);

        yield return new WaitForSeconds(pickupDuration);

        if (playerController != null)
            playerController.GetSword();

        if (heldSwordRenderer != null)
            heldSwordRenderer.enabled = false;

        if (playerSpriteRenderer != null && originalSprite != null)
            playerSpriteRenderer.sprite = originalSprite;

        if (playerAnimator != null)
            playerAnimator.enabled = true;

        if (dialogueBox != null)
            dialogueBox.SetActive(false);

        if (playerController != null)
            playerController.enabled = true;

        if (swordWorldObject != null && swordWorldObject != gameObject)
            Destroy(swordWorldObject);

        Destroy(gameObject);
    }
}