using UnityEngine;

public class MonedaProyectil : MonoBehaviour
{
    [SerializeField] int damage = 20;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Character>().TakeDamage(damage);
            Destroy(gameObject);
        }

        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
        else { }
    }
}