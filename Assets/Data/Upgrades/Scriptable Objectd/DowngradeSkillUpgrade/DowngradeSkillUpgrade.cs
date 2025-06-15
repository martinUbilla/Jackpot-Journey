using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Upgrade System/Negative/DowngradeSkill")]
public class DowngradeSkillUpgrade : UpgradeData
{
    public override bool IsNegative => true;

    public override void Apply(GameObject player)
    {
        Debug.Log("Intentando reducir nivel de una habilidad...");

        // Obtener todas las mejoras activas del jugador
        Level level = player.GetComponent<Level>();

        if (level == null || level.acquiredUpgrades == null || level.acquiredUpgrades.Count == 0)
        {
            Debug.Log("No hay mejoras adquiridas");
            return;
        }

        // Filtrar las que tienen nivel 2 o más
        List<UpgradeData> downgradeCandidates = level.acquiredUpgrades.FindAll(up => up.currentLevel >= 2);

        if (downgradeCandidates.Count == 0)
        {
            Debug.Log("No hay habilidades con nivel 2 o más para reducir");
            return;
        }

        // Elegir una aleatoria
        int index = Random.Range(0, downgradeCandidates.Count);
        UpgradeData toDowngrade = downgradeCandidates[index];

        toDowngrade.currentLevel--;
        FindFirstObjectByType<Level>().RefreshUpgradeUI(toDowngrade);
        Debug.Log($"Se redujo el nivel de la mejora: {toDowngrade.Name} (nuevo nivel: {toDowngrade.currentLevel})");
    }
}
