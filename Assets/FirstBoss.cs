using UnityEngine;

public class BossEnemy : MonoBehaviour, IEnemy
{
    [SerializeField] int hp = 1500;
    [SerializeField] int damage = 2;
    [SerializeField] int experienceReward = 1500;

    [SerializeField] float speed = 2f;
    [SerializeField] float rayoRange = 8f;
    [SerializeField] float explosionRange = 5f;
    [SerializeField] float meleeRange = 2f;
    [SerializeField] float attackCooldown = 2f;

    // Nuevos campos para los ataques especiales
    [SerializeField] GameObject rayoProjectilePrefab;
    [SerializeField] GameObject explosionProjectilePrefab;
    [SerializeField] Transform firePoint; // Punto desde donde salen los proyectiles
    [SerializeField] int rayoDamage = 3;
    [SerializeField] int explosionDamage = 4;

    private float nextAttackTime;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    Transform targetDestination;
    GameObject targetGameObject;
    Character targetCharacter;
    [SerializeField] GameObject coinPrefab;
    [SerializeField] float dropOffset = 0.5f;

    private enum BossAttackState { Rayo, Explosion, Melee }
    private BossAttackState currentState = BossAttackState.Rayo;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Si no se asignó firePoint, usar la posición del boss
        if (firePoint == null)
        {
            firePoint = transform;
        }
    }

    public void SetTarget(GameObject target)
    {
        targetGameObject = target;
        targetDestination = target.transform;
    }

    private void FixedUpdate()
    {
        if (targetDestination == null) return;

        MoveTowardsTarget();

        float distance = Vector2.Distance(transform.position, targetDestination.position);
        if (Time.time >= nextAttackTime)
        {
            TryAttack(distance);
        }
    }

    private void MoveTowardsTarget()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Vector2 direction = (targetDestination.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        // Flip sprite
        if (direction.x < -0.01f)
            spriteRenderer.flipX = false;
        else if (direction.x > 0.01f)
            spriteRenderer.flipX = true;
    }

    private void TryAttack(float distance)
    {
        switch (currentState)
        {
            case BossAttackState.Rayo:
                if (distance <= rayoRange)
                {
                    RayoAttack();
                    currentState = BossAttackState.Explosion;
                    nextAttackTime = Time.time + attackCooldown;
                }
                break;

            case BossAttackState.Explosion:
                if (distance <= explosionRange)
                {
                    ExplosionAttack();
                    currentState = BossAttackState.Melee;
                    nextAttackTime = Time.time + attackCooldown;
                }
                break;

            case BossAttackState.Melee:
                if (distance <= meleeRange)
                {
                    MeleeAttack();
                    currentState = BossAttackState.Rayo;
                    nextAttackTime = Time.time + attackCooldown;
                }
                break;
        }
    }

    private void RayoAttack()
    {
        if (rayoProjectilePrefab == null || targetDestination == null) return;

        // Calcular dirección hacia el jugador
        Vector2 direction = (targetDestination.position - firePoint.position).normalized;

        // Crear el proyectil de rayo
        GameObject rayoProjectile = Instantiate(rayoProjectilePrefab, firePoint.position, Quaternion.identity);

        // Configurar el proyectil
        RayoProjectile rayoScript = rayoProjectile.GetComponentInChildren<RayoProjectile>();
        if (rayoScript != null)
        {
            rayoScript.SetDirection(direction);
            rayoScript.SetDamage(rayoDamage);
        }

        // Rotar el proyectil para que apunte en la dirección correcta
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rayoProjectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Debug.Log("Boss ejecutó ataque de rayo!");
    }

    private void ExplosionAttack()
    {
        if (explosionProjectilePrefab == null || targetDestination == null) return;

        // Calcular dirección hacia el jugador
        Vector2 direction = (targetDestination.position - firePoint.position).normalized;

        // Crear el proyectil explosivo
        GameObject explosionProjectile = Instantiate(explosionProjectilePrefab, firePoint.position, Quaternion.identity);

        // Configurar el proyectil
        ExplosionProjectile explosionScript = explosionProjectile.GetComponentInChildren<ExplosionProjectile>();
        if (explosionScript != null)
        {
            explosionScript.SetDirection(direction);
            explosionScript.SetDamage(explosionDamage);
        }

        // Rotar el proyectil para que apunte en la dirección correcta
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        explosionProjectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Debug.Log("Boss ejecutó ataque explosivo!");
    }

    private void MeleeAttack()
    {
        // Ataque cuerpo a cuerpo (el original)
        if (targetCharacter == null && targetGameObject != null)
        {
            targetCharacter = targetGameObject.GetComponent<Character>();
        }

        if (targetCharacter != null)
        {
            targetCharacter.TakeDamage(damage);
            Debug.Log("Boss ejecutó ataque cuerpo a cuerpo!");
        }
    }

    // Método mantenido para compatibilidad
    public void Attack()
    {
        MeleeAttack();
    }

    public void TakeDamage(int damageAmount)
    {
        hp -= damageAmount;
        if (hp <= 0)
        {
            if (targetGameObject != null)
            {
                Level level = targetGameObject.GetComponent<Level>();
                if (level != null)
                {
                    ScoreManager.Instance.AddScore(100); // Aï¿½adir puntaje al morir
                    level.addExperience(experienceReward);
                }
            }
            ScoreManager.Instance.AddScore(300);
            DropItem();
            Destroy(gameObject);

        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject == targetGameObject)
        {
            if (targetCharacter == null)
                targetCharacter = targetGameObject.GetComponent<Character>();

            if (targetCharacter != null)
                targetCharacter.TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, rayoRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }

    private void DropItem()
    {
        // Genera el objeto en la posiciï¿½n del jefe con un pequeï¿½o desplazamiento
        Vector3 dropPosition = transform.position + new Vector3(Random.Range(-dropOffset, dropOffset), Random.Range(-dropOffset, dropOffset), 0);

        // Dropea una moneda
        Instantiate(coinPrefab, dropPosition, Quaternion.identity);
        Debug.Log("¡Droppé una moneda!");

          
        
        

    }
}