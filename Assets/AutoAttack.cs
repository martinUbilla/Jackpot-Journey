using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 2f;     // Rango de ataque
    [SerializeField] private float attackCooldown = 0.5f; // Tiempo entre ataques
    [SerializeField] private int damage;             // Daño infligido por ataque
    [SerializeField] private LayerMask enemyLayer;        // Capa de enemigos
    [SerializeField] GameObject spriteWhipLeft;
    [SerializeField] GameObject spriteWhipRight;
    [SerializeField] private AudioClip attackSound;
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
                if (playerMove.lastHorizontalVector > 0)
                {
                    spriteWhipRight.SetActive(true);
                    SoundManager.Instance.PlaySound(attackSound);
               
                }
                else
                {
                    spriteWhipLeft.SetActive(true);
                    SoundManager.Instance.PlaySound(attackSound);

                }
                // Obtener el componente de salud del enemigo y aplicar daño
                IEnemy enemyHealth = enemy.GetComponent<IEnemy>();
               
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                    Debug.Log("Atacando al enemigo: " + enemy.name);
                }
            }
        }
       
    }

    // Visualización del rango en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    public void subirDamage(int amount)
    {
        damage += amount;
    }

    public void subirCDAtaque(float amount)
    {
        attackCooldown += amount;
    }

    public void bajarCDAtaque(float amount)
    {
        attackCooldown -= amount;
    }
    public float getCd()
    {
        return attackCooldown;
    }
}
