using System.Collections.Generic;
using UnityEngine;



public class RouletteUIManager : MonoBehaviour
{
    public GameObject roulettePanel;
    private bool isOpen = false;
    public List<RouletteEffect> possibleEffects; // Lista de posibles resultados
    public Character playerStats;              // Referencia al jugador
    public TMPro.TextMeshProUGUI resultText;     // Texto para mostrar resultado
    public int spinCost = 3; // el costo en monedas


    void Start()
    {
        if (roulettePanel == null)
        {
            Debug.LogError("roulettePanel está NULL en Start");
        }
        else
        {
            Debug.Log("roulettePanel en Start está bien asignado");
        }
    }


   
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {

            if (roulettePanel.activeInHierarchy == false)
            {
                ToggleRoulettePanel();
            }
            else
            {
                CloseRoulette();
            }
        }
    }

    public void ToggleRoulettePanel()
    {
        Time.timeScale = 0f;
        Debug.Log("TAB PRESIONADO - roulettePanel es: " + roulettePanel);

        if (roulettePanel == null)
        {
            Debug.LogError("roulettePanel está NULL en Toggle");
            return;
        }

        isOpen = !isOpen;
        roulettePanel.SetActive(isOpen);
        Debug.Log("Panel activo: " + roulettePanel.activeSelf);
    }

    public void CloseRoulette()
    {
        Time.timeScale = 1f;
        isOpen = false;
        roulettePanel.SetActive(false);
    }

    public void SpinRoulette()
    {
        if (possibleEffects == null || possibleEffects.Count == 0)
        {
            Debug.LogWarning("No hay efectos en la ruleta.");
            return;
        }
        if (!playerStats.SpendCoins(spinCost))
        {
            resultText.text = "¡No tienes suficientes monedas!";
            Debug.Log("No tienes monedas suficientes para girar");
            return;
        }
        int index = Random.Range(0, possibleEffects.Count);
        RouletteEffect selected = possibleEffects[index];

        // Aplicar el efecto
        selected.Apply(playerStats);

        // Mostrar resultado
       
        Debug.Log("Efecto aplicado: "+selected.name);
    }

}


