using UnityEngine;

public class StarCoreInventory : MonoBehaviour
{
    [Header("Star Core Count")]
    public int starCores = 0;

    public void AddStarCore(int amount)
    {
        starCores += amount;
        Debug.Log("Star Cores collected: " + starCores);
    }
}