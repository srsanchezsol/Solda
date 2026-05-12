using UnityEngine;
using TMPro;
using Solana.Unity.SDK;
using Solana.Unity.Wallet;

public class WalletConnectUI : MonoBehaviour
{
    public TMP_Text walletText;

    private void OnEnable()
    {
        Web3.OnLogin += OnWalletConnected;
        Web3.OnLogout += OnWalletDisconnected;
    }

    private void OnDisable()
    {
        Web3.OnLogin -= OnWalletConnected;
        Web3.OnLogout -= OnWalletDisconnected;
    }

    public void ConnectWallet()
    {
        if (Web3.Instance == null)
        {
            Debug.LogError("Web3 instance not found in scene.");
            return;
        }

        Web3.Instance.LoginWithWalletAdapter();
    }

    private void OnWalletConnected(Account account)
    {
        if (account == null)
        {
            Debug.LogWarning("Wallet connection returned null account.");
            return;
        }

        string pubKey = account.PublicKey;
        Debug.Log("Wallet connected: " + pubKey);

        if (walletText != null)
            walletText.text = "Wallet: " + Shorten(pubKey);
    }

    private void OnWalletDisconnected()
    {
        if (walletText != null)
            walletText.text = "Wallet: Not connected";
    }

    private string Shorten(string s)
    {
        if (string.IsNullOrEmpty(s) || s.Length < 10) return s;
        return s.Substring(0, 4) + "..." + s.Substring(s.Length - 4);
    }
}