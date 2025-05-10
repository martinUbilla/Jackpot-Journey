using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class Character : MonoBehaviour
{
    public int maxHp = 1000;
    public int currentHp = 1000;
    [SerializeField] StatusBar hpBar;
    private bool isDead;

    public void TakeDamage(int damage)
    {
        if (isDead == true)
        {
            return;
        }
        currentHp -= damage;
        if(currentHp < 1)
        {
            GetComponent<CharacterGameOver>().GameOver();
            isDead = true;
        }
        hpBar.SetState(currentHp, maxHp);
    }
}
