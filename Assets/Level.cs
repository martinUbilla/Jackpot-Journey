using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Level : MonoBehaviour
{
    int level = 1;
    int experience = 0;
    [SerializeField] Character character;
    [SerializeField] ExperienceBar experienceBar;
    [SerializeField] AutoAttack arma;
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject weaponParent;
    [SerializeField] UpgradePanelManager upgradePanel;
    [SerializeField] List<UpgradeData> upgrades;
    public List<UpgradeData> selectedUpgrades;
    [SerializeField] public List<UpgradeData> acquiredUpgrades;
    [SerializeField] private List<UpgradeData> downgrades;
    [SerializeField] private Transform upgradeUIContainer;
    [SerializeField] private UpgradeIconUI upgradeIconPrefab;

    private List<UpgradeIconUI> activeIcons = new List<UpgradeIconUI>();

    private bool isChoosingUpgrade = false;


    int TO_LEVEL_UP
    {
        get
        {
            return level * 1000;
        }
    }
    private void Start()
    {
        experienceBar.UpdateExperienceSlider(experience, TO_LEVEL_UP);
        experienceBar.SetLevelText(level);
    }
    public void addExperience(int amount)
    {
        experience += amount;
        CheckLevelUp();
        experienceBar.UpdateExperienceSlider(experience, TO_LEVEL_UP);
    }

    private void CheckLevelUp()
    {
        if (experience >= TO_LEVEL_UP )
        {
            isChoosingUpgrade = true;
            if (selectedUpgrades == null) { selectedUpgrades = new List<UpgradeData>(); }
            selectedUpgrades.Clear();
            selectedUpgrades.AddRange(GetUpgrades(4));
            upgradePanel.OpenPanel(selectedUpgrades);
            experience -= TO_LEVEL_UP;
            level += 1;
            HealToFull();
            IncreaseDamage();
            experienceBar.SetLevelText(level);
            if (level > 6)
            {
                GetComponent<PlayerMove>().enabled = false;
                winPanel.SetActive(true);
                weaponParent.SetActive(false);
                character.maxHp = 1000000000;
                HealToFull(); 
            }
        }
    }

    private void IncreaseDamage()
    {
        arma.subirDamage(100);
    }

    private void HealToFull()
    {
        character.Heal(character.maxHp);
    }

    public List<UpgradeData> GetUpgrades(int count)
    {
        List<UpgradeData> finalUpgrades = new List<UpgradeData>();

        // 1. Mezclar y seleccionar 2 mejoras normales (no negativas)
        List<UpgradeData> availableUpgrades = new List<UpgradeData>(upgrades);
        for (int i = 0; i < availableUpgrades.Count; i++)
        {
            int rand = UnityEngine.Random.Range(i, availableUpgrades.Count);
            (availableUpgrades[i], availableUpgrades[rand]) = (availableUpgrades[rand], availableUpgrades[i]);
        }

        int upgradeCount = Mathf.Min(2, availableUpgrades.Count);
        for (int i = 0; i < upgradeCount; i++)
        {
            finalUpgrades.Add(availableUpgrades[i]);
        }

        // 2. Mezclar y seleccionar 1 downgrade
        if (downgrades.Count > 0)
        {
            List<UpgradeData> availableDowngrades = new List<UpgradeData>(downgrades);
            for (int i = 0; i < availableDowngrades.Count; i++)
            {
                int rand = UnityEngine.Random.Range(i, availableDowngrades.Count);
                (availableDowngrades[i], availableDowngrades[rand]) = (availableDowngrades[rand], availableDowngrades[i]);
            }

            finalUpgrades.Add(availableDowngrades[0]);
        }

        // 3. Mezclar las 3 cartas finales para que el downgrade no esté siempre en el mismo lugar
        for (int i = 0; i < finalUpgrades.Count; i++)
        {
            int rand = UnityEngine.Random.Range(i, finalUpgrades.Count);
            (finalUpgrades[i], finalUpgrades[rand]) = (finalUpgrades[rand], finalUpgrades[i]);
        }

        return finalUpgrades;
    }




    public void Upgrade(int selectedUpgradeID)
    {
        
        UpgradeData upgradeData = selectedUpgrades[selectedUpgradeID];

        if (upgradeData == null) return;

        if (acquiredUpgrades == null)
            acquiredUpgrades = new List<UpgradeData>();

        if (!upgradeData.IsNegative && !acquiredUpgrades.Contains(upgradeData))
        {
            acquiredUpgrades.Add(upgradeData);
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Debug.Log("Aplicando mejora: " + upgradeData.Name);
            upgradeData.Apply(player);
        }
        if (!upgradeData.IsNegative)
        {
            ShowUpgradeIcon(upgradeData);
        }
        else
        {
            UpgradeIconUI icon = Instantiate(upgradeIconPrefab, upgradeUIContainer);
            icon.Initialize(upgradeData.icon, upgradeData.currentLevel, upgradeData); // nivel 1 por defecto
            Destroy(icon.gameObject, 3f); // downgrade: desaparecer en 3 segundos
        }

    }


    public void ResetUpgradeState()
        {
        isChoosingUpgrade = false;
        }
    private void ShowUpgradeIcon(UpgradeData upgradeData)
    {
        // Revisar si ya existe un ícono para esta mejora
        foreach (var icon in activeIcons)
        {
            if (icon.upgradeData == upgradeData)
            {
                icon.SetLevel(upgradeData.currentLevel);
                return;
            }
        }

        // Si no existe, crear uno nuevo
        UpgradeIconUI iconUI = Instantiate(upgradeIconPrefab, upgradeUIContainer);
        iconUI.Initialize(upgradeData.icon, upgradeData.currentLevel, upgradeData);
        activeIcons.Add(iconUI);
    }
    public void TriggerCooldown(UpgradeData upgradeData)
    {
        foreach (var icon in activeIcons)
        {
            if (icon.upgradeData == upgradeData)
            {
                icon.TriggerCooldown(); // ?? esto inicia el cooldown visual
            }
        }
    }
    public void RefreshUpgradeUI(UpgradeData targetUpgrade)
    {
        foreach (var icon in activeIcons)
        {
            if (icon.upgradeData == targetUpgrade)
            {
                icon.SetLevel(targetUpgrade.currentLevel);
               
            }
        }
    }
    public void RefreshCooldownUI(UpgradeData upgradeData)
    {
        foreach (var icon in activeIcons)
        {
            if (icon.upgradeData == upgradeData)
            {
                icon.SetCooldown(upgradeData.cooldownDuration);
            }
        }
    }





}
