using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradePanelManager : MonoBehaviour
{
    [SerializeField] GameObject panel;
    PauseManager pauseManager;
    [SerializeField] List<UpgradeButton> upgradesButton;
    
    private void Awake()
    {
        pauseManager = GetComponent<PauseManager>();
    }

    public void OpenPanel(List<UpgradeData> upgradeDatas)
    {
        Clean();
        pauseManager.PauseGame();
        panel.SetActive(true);

        for (int i = 0; i < upgradeDatas.Count; i++)
        {
            upgradesButton[i].gameObject.SetActive(true);
            upgradesButton[i].Set(upgradeDatas[i], this, i);
        }
    }

    private void Start()
    {
        for (int i = 0; i < upgradesButton.Count; i++)
        {
            upgradesButton[i].gameObject.SetActive(false);
        }
    }

    // Método llamado cuando se selecciona una carta
    public void CardSelected(int buttonIndex)
    {
        Debug.Log($"=== CARTA SELECCIONADA: {buttonIndex} ===");

        // Deshabilitar todos los botones
        DisableAllButtons();

        // Iniciar el proceso de delay y upgrade
        StartCoroutine(DelayedUpgrade(buttonIndex));
    }

    // Método para deshabilitar todos los botones (evitar clics múltiples)
    public void DisableAllButtons()
    {
        for (int i = 0; i < upgradesButton.Count; i++)
        {
            upgradesButton[i].GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
    }

    private System.Collections.IEnumerator DelayedUpgrade(int buttonIndex)
    {
        Debug.Log("=== ESPERANDO 2 SEGUNDOS ===");

        // Temporalmente despausar para que funcione la corrutina
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 1f;

        yield return new WaitForSeconds(2f);

        // Volver a pausar
        Time.timeScale = originalTimeScale;

        Debug.Log("=== APLICANDO MEJORA DESPUÉS DEL DELAY ===");

        Upgrade(buttonIndex);
    }

    public void Upgrade(int pressedButtonID)
    {
        Debug.Log($"Aplicando mejora del botón: {pressedButtonID}");

        // Aplicar la mejora
        GameManager.instance.playerTransform.GetComponent<Level>().Upgrade(pressedButtonID);

        // Cerrar el panel
        ClosePanel();
    }

    public void Clean()
    {
        for (int i = 0; i < upgradesButton.Count; i++)
        {
            upgradesButton[i].Clean();
        }
    }

    public void ClosePanel()
    {
        // Reactivar botones antes de cerrar
        for (int i = 0; i < upgradesButton.Count; i++)
        {
            upgradesButton[i].GetComponent<UnityEngine.UI.Button>().interactable = true;
            upgradesButton[i].gameObject.SetActive(false);
        }

        pauseManager.UnPauseGame();
        panel.SetActive(false);
        GameManager.instance.playerTransform.GetComponent<Level>().ResetUpgradeState();

        Debug.Log("Panel cerrado");
    }
    
}