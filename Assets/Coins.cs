using UnityEngine;

public class Coins : MonoBehaviour
{
    public int coinAcquired;
    [SerializeField] TMPro.TextMeshProUGUI coinsCountText;
    public void Add(int count)
    {
        coinAcquired += count;
        coinsCountText.text = "Dinero: " + coinAcquired.ToString();
    }
    public bool Spend(int count)
    {
        if (coinAcquired >= count)
        {
            coinAcquired -= count;
            coinsCountText.text = "Dinero: " + coinAcquired.ToString();
            return true;
        }
        else
        {
            Debug.Log("No hay suficiente dinero para gastar.");
            return false;
        }
    }
}
