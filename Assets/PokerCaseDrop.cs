using UnityEngine;

public class PokerCaseDrop : MonoBehaviour
{
    [SerializeField] GameObject coinPrefab;       // Prefab de la moneda
    [SerializeField] GameObject heartPrefab;      // Prefab del corazón
    [SerializeField] float dropOffset = 0.5f;     // Desplazamiento del drop

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DropItem();
            Destroy(gameObject);  // Destruir el maletín después del drop
        }
    }

    private void DropItem()
    {
        int randomValue = Random.Range(0, 2);  // 0 o 1

        // Genera el objeto en la posición del maletín con un pequeño desplazamiento
        Vector3 dropPosition = transform.position + new Vector3(Random.Range(-dropOffset, dropOffset), Random.Range(-dropOffset, dropOffset), 0);

        if (randomValue == 0)
        {
            // Dropea una moneda
            Instantiate(coinPrefab, dropPosition, Quaternion.identity);
            Debug.Log("¡Droppé una moneda!");
        }
        else
        {
            // Dropea un corazón de vida
            Instantiate(heartPrefab, dropPosition, Quaternion.identity);
            Debug.Log("¡Droppé un corazón de vida!");
        }
    }
}
