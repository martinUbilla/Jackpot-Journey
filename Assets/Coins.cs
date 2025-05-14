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
}
