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

    private float nextAttackTime;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    Transform targetDestination;
    GameObject targetGameObject;
    Character targetCharacter;

    private enum BossAttackState { Rayo, Explosion, Melee }
    private BossAttackState currentState = BossAttackState.Rayo;

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
                    Attack();
                    currentState = BossAttackState.Explosion;
                    nextAttackTime = Time.time + attackCooldown;
                }
                break;

            case BossAttackState.Explosion:
                if (distance <= explosionRange)
                {
                    Attack();
                    currentState = BossAttackState.Melee;
                    nextAttackTime = Time.time + attackCooldown;
                }
                break;

            case BossAttackState.Melee:
                if (distance <= meleeRange)
                {
                    Attack();
                    currentState = BossAttackState.Rayo;
                    nextAttackTime = Time.time + attackCooldown;
                }
                break;
        }
    }

    public void Attack()
    {
        if (targetCharacter == null && targetGameObject != null)
        {
            targetCharacter = targetGameObject.GetComponent<Character>();
        }

        if (targetCharacter != null)
        {
            targetCharacter.TakeDamage(damage);
        }
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
                    level.addExperience(experienceReward);
                }
            }
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
}
