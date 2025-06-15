using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum UpgradeType
{
    WeaponUpgrade,
    ItemUpgrade,
    WeaponUnlock,
    ItemUnlock
}
[CreateAssetMenu] 
public class UpgradeData : ScriptableObject
{
    public GameObject upgrade; 
    public UpgradeType UpgradeType;
    public string Name;
    public Sprite icon;
    [HideInInspector] public int currentLevel = 0;
    
    [HideInInspector] public float cooldownDuration = 0f;
    [HideInInspector] public float cooldownRemaining = 0f;

    public virtual bool IsNegative => false;

    public virtual void Apply(GameObject player)
    {
        CardShooter shooter = player.GetComponentInChildren<CardShooter>();
        if (currentLevel == 0)
        {
            currentLevel++;
        }
        Debug.Log($"Aplicando mejora: {Name}");

        if (upgrade != null)
        {

            Debug.Log("Instanciando upgrade prefab...");

            if (player.GetComponentInChildren<CardShooter>() == null)
            {
                GameObject instance = Instantiate(upgrade, player.transform);
                instance.transform.localPosition = Vector3.zero;
            }
            else
            {
                currentLevel++;
                shooter.GetComponentInChildren<CardShooter>().cardSpeed += 0.2f; // cada nivel +1 velocidad
                shooter.SetInterval(Mathf.Max(0.3f, 1.5f - 0.1f * currentLevel)) ; // reduce intervalo hasta un mínimo
                shooter.SetProjectileDamage( shooter.cardDamage * currentLevel);
                Debug.Log($"[Upgrade] CardShooter mejorado ? Velocidad: {shooter.cardSpeed}, Intervalo: {shooter.shootInterval}");
            }
        }
    }
    public virtual void ResetLevel()
    {
        currentLevel = 0;
    }

}


