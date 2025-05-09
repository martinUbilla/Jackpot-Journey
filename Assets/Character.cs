using UnityEngine;

public class Character : MonoBehaviour
{
    public int maxHp = 1000;
    public int currentHp = 1000;
    [SerializeField] StatusBar hpBar;
    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        if(currentHp < 1)
        {
            Destroy(gameObject);
        }
        hpBar.SetState(currentHp, maxHp);
    }
}
