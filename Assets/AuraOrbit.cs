using UnityEngine;

public class AuraOrbit : MonoBehaviour
{
    public Transform center;          // Referencia al jugador
    public float radius = 1.5f;       // Distancia desde el centro
    public float speed = 90f;         // Velocidad de rotación (grados por segundo)
    public float angleOffset;         // Ángulo inicial único para cada dado

    private float currentAngle;

    private void Start()
    {
        currentAngle = angleOffset;
    }

    private void Update()
    {
        currentAngle += speed * Time.deltaTime;
        float radians = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0) * radius;
        transform.position = center.position + offset;
    }
}
