using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeIconUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image cooldownOverlay;

    private float cooldownDuration;
    private float cooldownRemaining;
    private bool hasCooldown;
    public UpgradeData upgradeData;
    public void Initialize(Sprite sprite, int level, UpgradeData reference)
    {
        if (icon != null && sprite != null)
            icon.sprite = sprite;

        if (levelText != null)
            levelText.text = $"Lv.{level}";

        upgradeData = reference;

        cooldownDuration = reference.cooldownDuration;       // ?? Este valor se copia aquí
        cooldownRemaining = reference.cooldownRemaining;     // ?? este puede estar en 0 al inicio
        hasCooldown = cooldownDuration > 0f;

        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = hasCooldown ? 1f : 0f;
    }





    public void TriggerCooldown()
    {
        if (!hasCooldown || cooldownDuration <= 0f) return;

        cooldownRemaining = cooldownDuration;

        Debug.Log("?? Cooldown visual iniciado: " + cooldownDuration);

        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 1f;
    }



    private void Update()
    {
        if (hasCooldown && cooldownRemaining > 0f)
        {
            cooldownRemaining -= Time.deltaTime;

            if (cooldownOverlay != null)
            {
                cooldownOverlay.fillAmount = cooldownRemaining / cooldownDuration;
            }
        }
    }


    public void SetLevel(int newLevel)
    {
        if (levelText != null)
            levelText.text = $"Lv.{newLevel}";
    }

    public void SetCooldown(float newCooldown)
    {
        cooldownDuration = newCooldown;
        cooldownRemaining = 0f; // opcional, reinicia el visual
    }

}
