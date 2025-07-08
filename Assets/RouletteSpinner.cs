using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RouletteSpinner : MonoBehaviour
{
    [Header("Configuraci�n de la Ruleta")]
    public Transform wheel; // El objeto que rota (tu ruleta)
    public float spinDuration = 3f;
    public int numberOfSpins = 4; // Siempre 4 giros completos

    [Header("Efectos y UI")]
    public RouletteEffect[] effects; // Arreglo de efectos
    public Character playerStats; // Referencia al jugador
    public TextMeshProUGUI resultText;
    public Image resultIcon;

    [Header("Configuraci�n Avanzada")]
    public AnimationCurve spinCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Curva de animaci�n personalizable
    public bool useEaseOutCubic = true; // Si prefieres usar la funci�n cubic

    private bool isSpinning = false;
    private float currentRotation = 0f;
    private Coroutine spinCoroutine; // Para poder cancelar la coroutine si es necesario

    public void StartSpin()
    {
        // CONDICIONALES PARA PERMITIR O NO LA RULETA
        if (isSpinning)
        {
            Debug.Log("La ruleta ya est� girando");
            return;
        }

        // Verificar si el jugador tiene suficientes monedas
        if (playerStats != null && !playerStats.SpendCoins(5)) // Cambia el 3 por el costo que quieras
        {
            if (resultText != null)
                resultText.text = "�No tienes suficientes monedas!";
            Debug.Log("No tienes monedas suficientes para girar");
            return;
        }

        // Verificar si el jugador est� vivo
        if (playerStats != null && playerStats.currentHp <= 0)
        {
            if (resultText != null)
                resultText.text = "�No puedes usar la ruleta estando muerto!";
            Debug.Log("El jugador est� muerto");
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
        // Detener la animaci�n si el objeto se desactiva
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
        Debug.Log("Iniciando animaci�n de ruleta...");

        // Configuraci�n b�sica
        int numberOfSections = effects.Length;
        float anglePerSection = 360f / numberOfSections; // Para 5 sectores = 72� cada uno

        // Seleccionar resultado ANTES de la animaci�n
        int selectedIndex = Random.Range(0, numberOfSections);

        // Calcular posici�n inicial normalizada
        float currentNormalizedAngle = currentRotation % 360f;
        if (currentNormalizedAngle < 0) currentNormalizedAngle += 360f;

        // CORRECCI�N PARA GIRO HORARIO
        // Como la ruleta gira en sentido horario, pero Unity rota en antihorario con valores positivos,
        // necesitamos invertir la l�gica
        float targetSectorCenter = selectedIndex * anglePerSection;

        // Para giro horario, invertimos la rotaci�n
        targetSectorCenter = -targetSectorCenter;

        // Agregar una peque�a variaci�n aleatoria dentro del sector (�15�, m�s peque�a que antes)
        float randomVariation = Random.Range(-15f, 15f);
        float finalTargetAngle = targetSectorCenter + randomVariation;

        // Calcular cu�ntos grados necesitamos rotar desde la posici�n actual
        float rotationNeeded = finalTargetAngle - currentNormalizedAngle;

        // Si necesitamos rotar "hacia atr�s", agregar una vuelta completa
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

        // Mostrar que est� girando
        if (resultText != null)
            StartCoroutine(ShowTextWithDelay(resultText, "Girando ...", 1f));

        Debug.Log($"=== DEBUG RULETA ===");
        Debug.Log($"Efecto seleccionado: {effects[selectedIndex].name} (�ndice {selectedIndex})");
        Debug.Log($"�ngulo actual: {currentNormalizedAngle}�");
        Debug.Log($"�ngulo objetivo del sector: {targetSectorCenter}�");
        Debug.Log($"�ngulo objetivo final: {finalTargetAngle}�");
        Debug.Log($"Rotaci�n total: {totalRotation}�");
        Debug.Log($"�ngulo final absoluto: {targetAngle}�");

        // Animaci�n principal
        while (elapsedTime < spinDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float t = elapsedTime / spinDuration;
            t = Mathf.Clamp01(t);

            // Usar curva de animaci�n
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
                Debug.LogError("�El objeto wheel es null durante la animaci�n!");
                isSpinning = false;
                yield break;
            }

            yield return null;
        }

        // Asegurar que termina en la posici�n exacta
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
            StartCoroutine(ShowTextWithDelay(resultText, $"�{selectedEffect.name}!", 1f));

        // Aplicar efecto al jugador
        selectedEffect.Apply(playerStats);

        // Debug final para verificar
        float finalNormalizedAngle = targetAngle % 360f;
        if (finalNormalizedAngle < 0) finalNormalizedAngle += 360f;

        int calculatedIndex = CalculateResultFromAngle(finalNormalizedAngle);

        Debug.Log($"=== RESULTADO ===");
        Debug.Log($"�ngulo final normalizado: {finalNormalizedAngle}�");
        Debug.Log($"�ndice seleccionado: {selectedIndex}");
        Debug.Log($"�ndice calculado: {calculatedIndex}");
        Debug.Log($"Efecto aplicado: {selectedEffect.name}");

        if (selectedIndex != calculatedIndex)
        {
            Debug.LogWarning("�Los �ndices no coinciden!");
        }

        isSpinning = false;
    }

    // M�todo para calcular el resultado basado en el �ngulo final
    private int CalculateResultFromAngle(float normalizedAngle)
    {
        int numberOfSections = effects.Length;
        float anglePerSection = 360f / numberOfSections;

        // Ajustar el �ngulo para que el centro del sector 0 est� en 0�
        float adjustedAngle = normalizedAngle + (anglePerSection / 2f);
        if (adjustedAngle >= 360f) adjustedAngle -= 360f;

        // Calcular en qu� sector est� la flecha
        int sectorIndex = Mathf.FloorToInt(adjustedAngle / anglePerSection);

        // Asegurar que est� en rango
        return sectorIndex % numberOfSections;
    }

    private float EaseOutCubic(float t)
    {
        t--;
        return t * t * t + 1;
    }

    // M�todos adicionales �tiles
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

    // Resetear rotaci�n si es necesario
    public void ResetRotation()
    {
        if (!isSpinning)
        {
            currentRotation = 0f;
            wheel.localEulerAngles = Vector3.zero;
        }
    }
    private IEnumerator ShowTextWithDelay(TextMeshProUGUI textComponent, string message, float displayDuration)
    {
        // Mostrar el texto
        textComponent.text = message;
        textComponent.gameObject.SetActive(true);

        // Esperar el tiempo especificado
        yield return new WaitForSeconds(displayDuration);

        // Ocultar el texto
        textComponent.text = "";
        textComponent.gameObject.SetActive(false);
    }
}