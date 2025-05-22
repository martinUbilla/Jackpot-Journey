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
    List<UpgradeData> selectedUpgrades;
    [SerializeField] List<UpgradeData> acquiredUpgrades;
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
        if (experience >= TO_LEVEL_UP)
        {
            if(selectedUpgrades == null) { selectedUpgrades = new List<UpgradeData>(); }
            selectedUpgrades.Clear();
            selectedUpgrades.AddRange(GetUpgrades(4));
            upgradePanel.OpenPanel(selectedUpgrades);
            experience -= TO_LEVEL_UP;
            level += 1;
            HealToFull();
            IncreaseDamage();
            experienceBar.SetLevelText(level);
            if (level > 2)
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
        List<UpgradeData> upgradeList = new List<UpgradeData>();

        if(count > upgrades.Count)
        {
            count = upgrades.Count;
        }

        for (int i = 0; i < count; i++)
        {
            upgradeList.Add(upgrades[UnityEngine.Random.Range(0,upgrades.Count)]);

        }
        return upgradeList;
    }

    public void Upgrade(int selectedUpgradeID)
    {
        UpgradeData upgradeData = selectedUpgrades[selectedUpgradeID];

        if (acquiredUpgrades == null){ acquiredUpgrades = new List<UpgradeData>(); }

        acquiredUpgrades.Add(upgradeData);
        upgrades.Remove(upgradeData);
    }
}
