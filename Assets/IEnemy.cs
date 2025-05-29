using UnityEngine;

public interface IEnemy
{
    void SetTarget(GameObject target);
    void TakeDamage(int attackDamage);

    void Attack();
}
