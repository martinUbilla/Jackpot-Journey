using UnityEngine;

public class RayoProjectile : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] int damage = 3;
    [SerializeField] float lifetime = 5f;
    [SerializeField] GameObject hitEffect; // Efecto visual al impactar (opcional)

    private Vector2 direction;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Destruir el proyectil después del tiempo de vida
        Destroy(gameObject, lifetime);
    }

    private void Start()
    {
        // Mover el proyectil en la dirección establecida
        rb.linearVelocity = direction * speed;
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verificar si impacta con el jugador
        if (other.CompareTag("Player"))
        {
            Character character = other.GetComponent<Character>();
            if (character != null)
            {
                character.TakeDamage(damage);
            }

            // Efecto visual al impactar (opcional)
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            // Destruir el proyectil
            Destroy(gameObject);
        }

        // Destruir si impacta con paredes u obstáculos
       
    }
}