using UnityEngine;

public class EnemyBola8 : MonoBehaviour
{
    [SerializeField] int hp = 999;
    [SerializeField] int damage = 1;
    [SerializeField] Transform targetDestination;
    [SerializeField] float speed = 3f;
    [SerializeField] float chargeSpeed = 8f;
    [SerializeField] float attackRange = 5f;
    [SerializeField] float chargeDuration = 1f;
    [SerializeField] float chargeCooldown = 2f;

    Character targetCharacter;
    GameObject targetGameObject;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private Vector2 chargeDirection;
    private float chargeTime;
    private float cooldownTime;
    private bool isCharging = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        targetGameObject = targetDestination.gameObject;
    }

    private void FixedUpdate()
    {
        if (isCharging)
        {
            ChargeAttack();
            return;
        }

        cooldownTime -= Time.fixedDeltaTime;
        Vector2 direction = (targetDestination.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        if (Vector2.Distance(transform.position, targetDestination.position) < attackRange && cooldownTime <= 0)
        {
            StartCharge(direction);
        }

        // Voltear sprite hacia la izquierda o derecha
        if (direction.x < -0.01f)
        {
            spriteRenderer.flipX = true;
        }
        else if (direction.x > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
    }

    private void StartCharge(Vector2 direction)
    {
        chargeDirection = direction;
        isCharging = true;
        chargeTime = chargeDuration;
    }

    private void ChargeAttack()
    {
        chargeTime -= Time.fixedDeltaTime;
        rb.linearVelocity = chargeDirection * chargeSpeed;

        if (chargeTime <= 0)
        {
            isCharging = false;
            cooldownTime = chargeCooldown;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject == targetGameObject)
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (targetCharacter == null)
        {
            targetCharacter = targetGameObject.GetComponent<Character>();
        }
        targetCharacter.TakeDamage(damage);
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp < 1)
        {
            Destroy(gameObject);
        }
    }
}

