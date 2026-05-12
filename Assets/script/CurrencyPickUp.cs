using UnityEngine;

public class CurrencyPickup : MonoBehaviour
{
    public int value = 1;
    public string currencyName = "Gem";

    [Header("Audio")]
    public AudioClip pickupSound;

    [Header("Pickup Delay")]
    public float pickupDelay = 0.15f;

    private bool collected = false;
    private bool canPickup = false;

    void Start()
    {
        Invoke(nameof(EnablePickup), pickupDelay);
    }

    void EnablePickup()
    {
        canPickup = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !canPickup) return;
        if (!other.CompareTag("Player")) return;

        collected = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        if (PlayerWallet.Instance != null)
        {
            PlayerWallet.Instance.AddCurrency(value);
        }

        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        Debug.Log("Picked up " + currencyName + " +" + value);

        Destroy(gameObject);
    }
}