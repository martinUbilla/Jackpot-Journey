using UnityEngine;

public class Tragamonedas : MonoBehaviour, IEnemy
{
    [SerializeField] int hp = 999;
    [SerializeField] int damage = 1;

    Transform targetDestination;
    GameObject targetGameObject;
    Character targetCharacter;

    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float detectionRange = 5f;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float fireCooldown = 2f;
    [SerializeField] Transform firePoint;
    [SerializeField] float speed = 3f;
    [SerializeField] int exp;
    private float nextFireTime;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    [SerializeField] GameObject coinPrefab;
    [SerializeField] float dropOffset = 0.5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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

        float distanceToPlayer = Vector2.Distance(transform.position, targetDestination.position);
        if (distanceToPlayer <= detectionRange && Time.time >= nextFireTime)
        {
            Attack();
            nextFireTime = Time.time + fireCooldown;
        }
    }

    private void MoveTowardsTarget()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Vector2 direction = (targetDestination.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        // Voltear sprite
        if (direction.x < -0.01f)
            spriteRenderer.flipX = true;
        else if (direction.x > 0.01f)
            spriteRenderer.flipX = false;
    }

    public void Attack()
    {
        if (firePoint == null || projectilePrefab == null || targetDestination == null) return;

        // Crear proyectil
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.transform.localScale = new Vector3(0.07f, 0.07f, 1f);

        // Direcci�n hacia el jugador
        Vector2 direction = (targetDestination.position - transform.position).normalized;

        // Asignar velocidad
        projectile.GetComponent<Rigidbody2D>().linearVelocity = direction * projectileSpeed;

        // Destruir despu�s de un tiempo
        Destroy(projectile, 2f);
    }

    public void TakeDamage(int damageAmount)
    {
        hp -= damageAmount;
        if (hp < 1)
        {
            targetGameObject.GetComponent<Level>().addExperience(exp);
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

            targetCharacter.TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
    private void DropItem()
    {
        // Genera el objeto en la posición del jefe con un pequeño desplazamiento
        Vector3 dropPosition = transform.position + new Vector3(Random.Range(-dropOffset, dropOffset), Random.Range(-dropOffset, dropOffset), 0);
        // Dropea una moneda
        Instantiate(coinPrefab, dropPosition, Quaternion.identity);
        Debug.Log("¡Droppé una moneda!");


    }
}
