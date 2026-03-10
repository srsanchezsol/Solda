using UnityEngine;
using UnityEngine;
using TMPro;

public class CurrencyHUD : MonoBehaviour
{
    public TextMeshProUGUI currencyText;

    void Update()
    {
        if (PlayerWallet.Instance == null)
        {
            Debug.Log("No PlayerWallet Instance found");
            return;
        }

        if (currencyText == null)
        {
            Debug.Log("Currency Text is not assigned");
            return;
        }

        Debug.Log("HUD reading: " + PlayerWallet.Instance.totalCurrency);
        currencyText.text = PlayerWallet.Instance.totalCurrency.ToString("000");
    }
}