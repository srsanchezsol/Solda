using UnityEngine;

public class CurrencyPickup : MonoBehaviour
{
    public int value = 1;
    public string currencyName = "Gem";

    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;

        collected = true;

        if (PlayerWallet.Instance != null)
        {
            PlayerWallet.Instance.AddCurrency(value);
        }

        Debug.Log("Picked up " + currencyName + " +" + value);

        Destroy(gameObject);
    }
}