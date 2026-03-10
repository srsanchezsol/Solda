using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    public static PlayerWallet Instance;

    public int totalCurrency = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddCurrency(int amount)
    {
        totalCurrency += amount;
        Debug.Log("Total Currency: " + totalCurrency);
    }
}