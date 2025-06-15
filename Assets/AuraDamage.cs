using UnityEngine;

public class AuraDamage : MonoBehaviour
{
    [SerializeField] public int damage = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            IEnemy enemy = collision.GetComponent<IEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
    public void SetDamage(int nDamage)
    {
        damage = nDamage;
    }
}
