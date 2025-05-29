using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade System/Aura Upgrade")]
public class AuraUpgrade : UpgradeData
{
    [SerializeField] private GameObject auraDiePrefab;
    [SerializeField] private int numberOfDice = 3;
    [SerializeField] private float orbitRadius = 1.5f;
    [SerializeField] private float rotationSpeed = 90f;

    public override void Apply(GameObject player)
    {
        for (int i = 0; i < numberOfDice; i++)
        {
            GameObject die = GameObject.Instantiate(auraDiePrefab, player.transform);
            AuraOrbit orbit = die.GetComponent<AuraOrbit>();
            orbit.center = player.transform;
            orbit.radius = orbitRadius;
            orbit.speed = rotationSpeed;
            orbit.angleOffset = (360f / numberOfDice) * i;
        }
    }
}
