using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade System/CardShooter Upgrade")]
public class CardShooterUpgrade : UpgradeData
{
    [SerializeField] private float intervalReductionPerLevel = 0.15f;
    [SerializeField] private float minInterval = 0.2f;
    [SerializeField] private int damagePerLevel = 5;
    public GameObject cardShooterPrefab;

    public override void Apply(GameObject player)
    {
        if (currentLevel == 0)
        {
            currentLevel++;
        }

        CardShooter shooter = player.GetComponentInChildren<CardShooter>();

        // Si no existe aún, instanciarlo
        if (shooter == null)
        {
            GameObject instance = Instantiate(cardShooterPrefab, player.transform);
            instance.transform.localPosition = Vector3.zero;
            shooter = instance.GetComponent<CardShooter>();
        }

        // Calcular nuevo intervalo
        float newInterval = shooter.GetInterval() - intervalReductionPerLevel;
        newInterval = Mathf.Max(newInterval, minInterval);
        
        shooter.SetInterval(newInterval);
        shooter.SetProjectileDamage(currentLevel * damagePerLevel);
        Debug.Log($"[CardShooterUpgrade] Nivel {currentLevel}, Nuevo intervalo: {newInterval}");
    }
    
    public override void ResetLevel()
    {
        currentLevel = 0;
    }
}
