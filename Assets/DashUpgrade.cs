using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade System/Dash Upgrade")]
public class DashUpgrade : UpgradeData
{
    public override void Apply(GameObject player)
    {
        if (player.GetComponent<DashAbility>() == null)
        {
            player.AddComponent<DashAbility>();
        }
    }
}
    