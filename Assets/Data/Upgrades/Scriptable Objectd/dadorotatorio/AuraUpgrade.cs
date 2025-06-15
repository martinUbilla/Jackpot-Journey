using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Upgrade System/Aura Upgrade")]
public class AuraUpgrade : UpgradeData
{
    [SerializeField] private GameObject auraDiePrefab;
    [SerializeField] private int baseNumberOfDice = 1;
    [SerializeField] private float orbitRadius = 1.5f;
    [SerializeField] private float baseRotationSpeed = 90f;
    [SerializeField] private int baseDamage = 10;
    [SerializeField] private float damageIncrement = 5f;
    [SerializeField] private float speedIncrement = 30f;

 
    private List<GameObject> activeDice = new List<GameObject>();

    public override void Apply(GameObject player)
    {
        currentLevel++;
        Debug.Log("?? Aura nivel: " + currentLevel);

        // Destruir dados anteriores
        foreach (GameObject die in activeDice)
        {
            if (die != null) GameObject.Destroy(die);
        }
        activeDice.Clear();

        int currentDice = baseNumberOfDice + (currentLevel - 1); // aumenta dados
        float currentSpeed = baseRotationSpeed + speedIncrement * (currentLevel - 1);
        int currentDamage = baseDamage + Mathf.RoundToInt(damageIncrement * (currentLevel - 1));

        Debug.Log("dados " + currentDice);
        Debug.Log("speed" + currentSpeed);
        Debug.Log("damage " + currentDamage);
        for (int i = 0; i < currentDice; i++)
        {
            GameObject die = GameObject.Instantiate(auraDiePrefab, player.transform);
            activeDice.Add(die);

            // Configurar órbita
            AuraOrbit orbit = die.GetComponent<AuraOrbit>();
            orbit.center = player.transform;
            orbit.radius = orbitRadius;
            orbit.speed = currentSpeed;
            orbit.angleOffset = (360f / currentDice) * i;

            // Configurar daño
            AuraDamage dmg = die.GetComponent<AuraDamage>();
            dmg.SetDamage(currentDamage);
        }
        cooldownDuration = 0f; // Si no tiene cooldown
        cooldownRemaining = 0f;
       

    }

    public override void ResetLevel()
    {
        currentLevel = 0;
    }
}
