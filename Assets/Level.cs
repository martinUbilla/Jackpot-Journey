using System;
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
}
