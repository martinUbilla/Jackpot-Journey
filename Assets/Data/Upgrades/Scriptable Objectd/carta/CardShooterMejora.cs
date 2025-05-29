using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade System/CardShooter Upgrade")]
public class CardShooterUpgrade : UpgradeData
{
    public GameObject cardShooterPrefab;

    public override void Apply(GameObject player)
    {
        if (player.GetComponentInChildren<CardShooter>() == null)
        {
            GameObject instance = Instantiate(cardShooterPrefab, player.transform);
            instance.transform.localPosition = Vector3.zero;
        }
    }
}
