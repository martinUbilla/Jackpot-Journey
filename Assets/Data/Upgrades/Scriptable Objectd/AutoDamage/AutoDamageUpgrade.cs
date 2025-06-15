using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade System/Negative/AutoDamage")]
public class AutoDamageUpgrade : UpgradeData
{
    public int damage = 30;

    public override bool IsNegative => true;

    public override void Apply(GameObject player)
    {
        Debug.Log("AutoDamage recibido");
        var character = player.GetComponent<Character>(); // Usa tu script de vida
        if (character != null)
        {
            character.TakeDamage(damage);
        }
    }
}
