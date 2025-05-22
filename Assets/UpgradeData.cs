using UnityEngine;

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
    public UpgradeType UpgradeType;
    public string Name;
    public Sprite icon;
}
