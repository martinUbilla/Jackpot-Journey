using System.Collections.Generic;
using UnityEngine;

public class UpgradeUIManager : MonoBehaviour
{
    [SerializeField] private Transform upgradeUIContainer;          // Contenedor en la UI
    [SerializeField] private UpgradeIconUI upgradeIconPrefab;       // Prefab del ícono

    private Dictionary<UpgradeData, UpgradeIconUI> activeIcons = new();

    public void ShowUpgradeIcon(UpgradeData upgradeData)
    {
        // Si ya existe un ícono de esta mejora, actualiza el nivel visual
        if (activeIcons.ContainsKey(upgradeData))
        {
            activeIcons[upgradeData].SetLevel(upgradeData.currentLevel);
        }
        else
        {
            // Instancia el nuevo ícono y guárdalo
            UpgradeIconUI icon = Instantiate(upgradeIconPrefab, upgradeUIContainer);
            icon.Initialize(upgradeData.icon, upgradeData.currentLevel, upgradeData);
            activeIcons.Add(upgradeData, icon);
        }
    }

    public void TriggerCooldown(UpgradeData upgradeData)
    {
        if (activeIcons.ContainsKey(upgradeData))
        {
            activeIcons[upgradeData].TriggerCooldown();
        }
    }
    

}
