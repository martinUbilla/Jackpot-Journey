using UnityEngine;

public class HealPickUpObject : MonoBehaviour, IpickupObject
{
    [SerializeField] int healAmount;


    public void OnPickUp(Character character)
    {
        
      
            character.Heal(healAmount);
            if (character.currentHp > character.maxHp)
            {
                character.currentHp = character.maxHp;
            }
        }
}

