using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RouletteSpinner : MonoBehaviour
{
    [Header("Configuración de la Ruleta")]
    public Transform wheel; // El objeto que rota (tu ruleta)
    public float spinDuration = 3f;
    public int numberOfSpins = 4; // Siempre 4 giros completos

    [Header("Efectos y UI")]
    public RouletteEffect[] effects; // Arreglo de efectos
    public Character playerStats; // Referencia al jugador
    public TextMeshProUGUI resultText;
    public Image resultIcon;

    [Header("Configuración Avanzada")]
    public AnimationCurve spinCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Curva de animación personalizable
    public bool useEaseOutCubic = true; // Si prefieres usar la función cubic

    private bool isSpinning = false;
    private float currentRotation = 0f;
    private Coroutine spinCoroutine; // Para poder cancelar la coroutine si es necesario

    public void StartSpin()
    {
        // CONDICIONALES PARA PERMITIR O NO LA RULETA
        if (isSpinning)
        {
            Debug.Log("La ruleta ya está girando");
            return;
        }

        // Verificar si el jugador tiene suficientes monedas
        if (playerStats != null && !playerStats.SpendCoins(3)) // Cambia el 3 por el costo que quieras
        {
            if (resultText != null)
                resultText.text = "¡No tienes suficientes monedas!";
            Debug.Log("No tienes monedas suficientes para girar");
            return;
        }

        // Verificar si el jugador está vivo
        if (playerStats != null && playerStats.currentHp <= 0)
        {
            if (resultText != null)
                resultText.text = "¡No puedes usar la ruleta estando muerto!";
            Debug.Log("El jugador está muerto");
            return;
        }

        // Verificar si hay efectos configurados
        if (effects == null || effects.Length == 0)
        {
            Debug.LogError("No hay efectos configurados en la ruleta");
            return;
        }

        // Cancelar cualquier coroutine anterior
        if (spinCoroutine != null)
        {
            StopCoroutine(spinCoroutine);
        }

        spinCoroutine = StartCoroutine(SpinWheel());
    }

    private void OnDisable()
    {
        // Detener la animación si el objeto se desactiva
        if (spinCoroutine != null)
        {
            StopCoroutine(spinCoroutine);
            isSpinning = false;
        }
    }

    private IEnumerator SpinWheel()
    {
        isSpinning = true;

        // Debug inicial
        Debug.Log("Iniciando animación de ruleta...");

        // Configuración básica
        int numberOfSections = effects.Length;
        float anglePerSection = 360f / numberOfSections; // Para 5 sectores = 72° cada uno

        // Seleccionar resultado ANTES de la animación
        int selectedIndex = Random.Range(0, numberOfSections);

        // Calcular posición inicial normalizada
        float currentNormalizedAngle = currentRotation % 360f;
        if (currentNormalizedAngle < 0) currentNormalizedAngle += 360f;

        // CORRECCIÓN PARA GIRO HORARIO
        // Como la ruleta gira en sentido horario, pero Unity rota en antihorario con valores positivos,
        // necesitamos invertir la lógica
        float targetSectorCenter = selectedIndex * anglePerSection;

        // Para giro horario, invertimos la rotación
        targetSectorCenter = -targetSectorCenter;

        // Agregar una pequeña variación aleatoria dentro del sector (±15°, más pequeña que antes)
        float randomVariation = Random.Range(-15f, 15f);
        float finalTargetAngle = targetSectorCenter + randomVariation;

        // Calcular cuántos grados necesitamos rotar desde la posición actual
        float rotationNeeded = finalTargetAngle - currentNormalizedAngle;

        // Si necesitamos rotar "hacia atrás", agregar una vuelta completa
        if (rotationNeeded < 0)
        {
            rotationNeeded += 360f;
        }

        // Agregar los giros completos
        float totalRotation = (numberOfSpins * 360f) + rotationNeeded;

        // Valores iniciales
        float startAngle = currentRotation;
        float targetAngle = startAngle + totalRotation;
        float elapsedTime = 0f;

        // Mostrar que está girando
        if (resultText != null)
            resultText.text = "Girando...";

        Debug.Log($"=== DEBUG RULETA ===");
        Debug.Log($"Efecto seleccionado: {effects[selectedIndex].name} (índice {selectedIndex})");
        Debug.Log($"Ángulo actual: {currentNormalizedAngle}°");
        Debug.Log($"Ángulo objetivo del sector: {targetSectorCenter}°");
        Debug.Log($"Ángulo objetivo final: {finalTargetAngle}°");
        Debug.Log($"Rotación total: {totalRotation}°");
        Debug.Log($"Ángulo final absoluto: {targetAngle}°");

        // Animación principal
        while (elapsedTime < spinDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float t = elapsedTime / spinDuration;
            t = Mathf.Clamp01(t);

            // Usar curva de animación
            float easedT = useEaseOutCubic ? EaseOutCubic(t) : spinCurve.Evaluate(t);

            float currentAngle = Mathf.Lerp(startAngle, targetAngle, easedT);

            // Verificar que el objeto wheel sigue existiendo
            if (wheel != null)
            {
                wheel.localEulerAngles = new Vector3(0, 0, -currentAngle);
                currentRotation = currentAngle;
            }
            else
            {
                Debug.LogError("¡El objeto wheel es null durante la animación!");
                isSpinning = false;
                yield break;
            }

            yield return null;
        }

        // Asegurar que termina en la posición exacta
        if (wheel != null)
        {
            wheel.localEulerAngles = new Vector3(0, 0, -targetAngle);
            currentRotation = targetAngle;
        }

        // Usar el efecto seleccionado
        var selectedEffect = effects[selectedIndex];

        // Actualizar UI
        if (resultIcon != null)
            resultIcon.sprite = selectedEffect.icon;

        if (resultText != null)
            resultText.text = $"¡{selectedEffect.name}!";

        // Aplicar efecto al jugador
        selectedEffect.Apply(playerStats);

        // Debug final para verificar
        float finalNormalizedAngle = targetAngle % 360f;
        if (finalNormalizedAngle < 0) finalNormalizedAngle += 360f;

        int calculatedIndex = CalculateResultFromAngle(finalNormalizedAngle);

        Debug.Log($"=== RESULTADO ===");
        Debug.Log($"Ángulo final normalizado: {finalNormalizedAngle}°");
        Debug.Log($"Índice seleccionado: {selectedIndex}");
        Debug.Log($"Índice calculado: {calculatedIndex}");
        Debug.Log($"Efecto aplicado: {selectedEffect.name}");

        if (selectedIndex != calculatedIndex)
        {
            Debug.LogWarning("¡Los índices no coinciden!");
        }

        isSpinning = false;
    }

    // Método para calcular el resultado basado en el ángulo final
    private int CalculateResultFromAngle(float normalizedAngle)
    {
        int numberOfSections = effects.Length;
        float anglePerSection = 360f / numberOfSections;

        // Ajustar el ángulo para que el centro del sector 0 esté en 0°
        float adjustedAngle = normalizedAngle + (anglePerSection / 2f);
        if (adjustedAngle >= 360f) adjustedAngle -= 360f;

        // Calcular en qué sector está la flecha
        int sectorIndex = Mathf.FloorToInt(adjustedAngle / anglePerSection);

        // Asegurar que está en rango
        return sectorIndex % numberOfSections;
    }

    private float EaseOutCubic(float t)
    {
        t--;
        return t * t * t + 1;
    }

    // Métodos adicionales útiles
    public void SetSpinDuration(float duration)
    {
        spinDuration = Mathf.Max(0.5f, duration);
    }

    public void SetNumberOfSpins(int spins)
    {
        numberOfSpins = Mathf.Max(1, spins);
    }

    public bool IsSpinning()
    {
        return isSpinning;
    }

    // Resetear rotación si es necesario
    public void ResetRotation()
    {
        if (!isSpinning)
        {
            currentRotation = 0f;
            wheel.localEulerAngles = Vector3.zero;
        }
    }
}