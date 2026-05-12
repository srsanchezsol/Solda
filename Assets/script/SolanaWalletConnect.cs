using UnityEngine;

public class SolanaWalletConnect : MonoBehaviour
{
    public void ConnectWallet()
    {
        string phantomURL = "https://phantom.app/ul/browse/https://solana.com";

        Application.OpenURL(phantomURL);
    }
}