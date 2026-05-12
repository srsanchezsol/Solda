using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    public static PlayerWallet Instance;

    public int totalCurrency = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddCurrency(int amount)
    {
        totalCurrency += amount;
        Debug.Log("Total Currency: " + totalCurrency);
    }

    public bool SpendCurrency(int amount)
    {
        if (totalCurrency < amount)
            return false;

        totalCurrency -= amount;
        return true;
    }

    public void ResetCurrency()
    {
        totalCurrency = 0;
    }
}