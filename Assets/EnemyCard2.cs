using UnityEngine;

public class EnemyCard2 : MonoBehaviour
{
    [SerializeField] int hp = 999;
    [SerializeField] int damage = 1;
    [SerializeField] Transform targetDestination;
    Character targetCharacter;
    [SerializeField] float speed = 3f;
    GameObject targetGameObject;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        targetGameObject = targetDestination.gameObject;
    }

    private void FixedUpdate()
    {
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

    private void Attack() { 
   if (targetCharacter == null)
        {
            targetCharacter = targetGameObject.GetComponent<Character>();
        }
    targetCharacter.TakeDamage(damage);
    }
    public void TakeDamage(int damage){
    hp -= damage;
    if (hp < 1){
        Destroy(gameObject);
        }
    }
}
