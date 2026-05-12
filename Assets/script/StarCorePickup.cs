using UnityEngine;

public class StarCorePickup : MonoBehaviour
{
    [Header("Pickup")]
    public int amount = 1;

    [Header("Audio")]
    public AudioClip pickupSfx;
    [Range(0f, 1f)] public float pickupVolume = 1f;

    [Header("Effect")]
    public GameObject pickupEffect;

    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        if (!other.CompareTag("Player")) return;

        collected = true;

        StarCoreInventory inventory = other.GetComponent<StarCoreInventory>();

        if (inventory != null)
        {
            inventory.AddStarCore(amount);
        }

        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        }

        if (pickupSfx != null)
        {
            GameObject tempAudio = new GameObject("TempPickupAudio");

            tempAudio.transform.position = transform.position;

            AudioSource audioSource = tempAudio.AddComponent<AudioSource>();

            audioSource.clip = pickupSfx;
            audioSource.volume = pickupVolume;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;

            audioSource.Play();

            Destroy(tempAudio, pickupSfx.length + 0.1f);
        }

        gameObject.SetActive(false);

        Destroy(gameObject, 0.05f);
    }
}