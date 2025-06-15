using UnityEngine;

public class UpgradeResetter : MonoBehaviour
{
    [SerializeField] private UpgradeData[] upgradesToReset;

    private void Awake()
    {
        foreach (var upgrade in upgradesToReset)
        {
            upgrade.ResetLevel();
        }
    }
}
