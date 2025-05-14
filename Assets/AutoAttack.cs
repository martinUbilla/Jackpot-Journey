using System.Collections;
using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 2f;     // Rango de ataque
    [SerializeField] private float attackCooldown = 0.5f; // Tiempo entre ataques
    [SerializeField] private int damage = 10;             // Daño infligido por ataque
    [SerializeField] private LayerMask enemyLayer;        // Capa de enemigos
    [SerializeField] GameObject spriteWhipLeft;
    [SerializeField] GameObject spriteWhipRight;
    private float nextAttackTime = 0f;
    PlayerMove playerMove;
    private void Awake()
    {
        playerMove = GetComponentInParent<PlayerMove>();
    }
    void Update()
    {
        // Verificar si es el momento de atacar
        if (Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void Attack()
    {
        // Detectar enemigos en el rango usando OverlapCircle
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

        // Si hay al menos un enemigo en el rango
        if (enemiesInRange.Length > 0)
        {
            // Atacar al primer enemigo detectado
            Collider2D enemy = enemiesInRange[0];
            if (enemy != null)
            {
                // Obtener el componente de salud del enemigo y aplicar daño
                EnemyCard0 enemyHealth = enemy.GetComponent<EnemyCard0>();
               
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                    Debug.Log("Atacando al enemigo: " + enemy.name);
                }
            }
        }
        if(playerMove.lastHorizontalVector > 0)
        {
            spriteWhipRight.SetActive(true);
        }
        else
        {
            spriteWhipLeft.SetActive(true);
        }
    }

    // Visualización del rango en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
