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

    public virtual void Apply(GameObject player)
    {
        Debug.Log($"Aplicando mejora: {Name}");

        if (upgrade != null)
        {
            Debug.Log("Instanciando upgrade prefab...");

            if (player.GetComponentInChildren<CardShooter>() == null)
            {
                GameObject instance = Instantiate(upgrade, player.transform);
                instance.transform.localPosition = Vector3.zero;
            }
        }
    }
}


