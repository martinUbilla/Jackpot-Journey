using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade System/Dash Upgrade")]
public class DashUpgrade : UpgradeData
{
    [SerializeField] private float cooldownReductionPerLevel = 0.2f;
    [SerializeField] private float minCooldown = 0.5f;
    [SerializeField] private float dashCooldown = 2f;
  
    public override void Apply(GameObject player)
    {
        currentLevel++;
        DashAbility dash = player.GetComponent<DashAbility>();

        if (dash == null)
        {
            dash = player.AddComponent<DashAbility>();
        }

        // Incrementar nivel (si estás usando currentLevel en UpgradeData)
        

        // Aplicar reducción de cooldown
        float newCooldown = dash.GetCooldown() - cooldownReductionPerLevel;
        newCooldown = Mathf.Max(newCooldown, minCooldown);
        dash.SetCooldown(newCooldown);

        cooldownDuration = newCooldown;
        FindFirstObjectByType<Level>().RefreshCooldownUI(this);
        dash.SetUpgradeData(this);

        Debug.Log($"[DashUpgrade] Nivel: {currentLevel}, Nuevo cooldown: {newCooldown}");
    }
    public override void ResetLevel()
    {
        currentLevel = 0;
    }
}
