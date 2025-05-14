using UnityEngine;

public class CoinPickUpObject : MonoBehaviour, IpickupObject
{
    [SerializeField] int count;
    public void OnPickUp(Character character)
    {
        character.coins.Add(count);
    }
}
