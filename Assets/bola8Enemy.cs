using Unity.VisualScripting;
using UnityEngine;

public class EnemyBola8 : MonoBehaviour, IEnemy
{
    [SerializeField] int hp = 999;
    [SerializeField] int damage = 1;
    Transform targetDestination;
    [SerializeField] float speed = 3f;
    [SerializeField] float chargeSpeed = 8f;
    [SerializeField] float attackRange = 5f;
    [SerializeField] float chargeDuration = 1f;
    [SerializeField] float chargeCooldown = 2f;

    [SerializeField] private AudioClip attackSound;
    [SerializeField] int exp;
    Character targetCharacter;
    GameObject targetGameObject;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    [SerializeField] GameObject coinPrefab;
    [SerializeField] float dropOffset = 0.5f;
    [SerializeField] int experience_reward = 400;

    private Vector2 chargeDirection;
    private float chargeTime;
    private float cooldownTime;
    private bool isCharging = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cooldownTime = 4;
    }
    public void SetTarget(GameObject target)
    {
        targetGameObject = target;
        targetDestination = target.transform;
    }

    private void FixedUpdate()
    {

        if (targetDestination == null)
        {
            return;
        };

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
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

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
        SoundManager.Instance.PlaySound(attackSound);
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

    public void Attack()
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
            targetGameObject.GetComponent<Level>().addExperience(experience_reward);
            ScoreManager.Instance.AddScore(100); // Añadir puntaje al morir
            DropItem();
            Destroy(gameObject);


        }
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

