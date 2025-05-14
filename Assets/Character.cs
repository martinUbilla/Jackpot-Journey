using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class Character : MonoBehaviour
{
    public int maxHp = 1000;
    public int currentHp = 1000;
    [SerializeField] StatusBar hpBar;
    [HideInInspector] public Coins coins;
    private bool isDead;

    private void Awake()
    {
        coins = GetComponent<Coins>();  

    }
    private void Start()
    {
        hpBar.SetState(currentHp, maxHp);  
    }
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
 


   public void Heal(int amount)
    {
        currentHp += amount;
        if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }
        hpBar.SetState(currentHp, maxHp);
    }

}


