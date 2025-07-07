using Unity.VisualScripting;
using UnityEngine;

public class EnemyCard0 : MonoBehaviour, IEnemy
{
    [SerializeField] int hp = 999;
    [SerializeField] int damage = 1;
    Transform targetDestination;
    Character targetCharacter;
    [SerializeField] float speed = 3f;
    GameObject targetGameObject;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    [SerializeField] int experience_reward = 400;
    [SerializeField] GameObject coinPrefab;
    [SerializeField] float dropOffset = 0.5f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void SetTarget(GameObject target)
    {
        targetGameObject = target;  
        targetDestination = target.transform;
    }

    private void FixedUpdate()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Vector2 direction = (targetDestination.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        // Voltear sprite hacia la izquierda o derecha
        if (direction.x < -0.01f)
        {
            spriteRenderer.flipX = true; // Mira a la izquierda
        }
        else if (direction.x > 0.01f)
        {
            spriteRenderer.flipX = false; // Mira a la derecha
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
    public void TakeDamage(int attackDamage)
    {
        hp -= attackDamage;
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