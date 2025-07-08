using UnityEngine;

[System.Serializable]
public class RouletteEffect
{
    public string name;
    public Sprite icon;
    public EffectType type;
    public int intValue;
    public float floatValue;
    [SerializeField] AutoAttack ataque;

    public enum EffectType
    {
        IncreaseHealth,
        IncreaseAttack,
        IncreaseAttackSpeed,
        InstantDamage,
        ReduceAttackSpeed,
        ConfuseScreen,
        // Puedes agregar m�s aqu�
    }

    public void Apply(Character stats)
    {
        switch (type)
        {
            case EffectType.IncreaseHealth:
                stats.IncreaseHealth(intValue);
                break;
            case EffectType.IncreaseAttack:
                ataque.subirDamage(intValue);
                break;
            case EffectType.IncreaseAttackSpeed:
                ataque.subirCDAtaque(floatValue);
                break;
            case EffectType.InstantDamage:
                stats.TakeDamage(intValue);
                break;
            case EffectType.ReduceAttackSpeed:
                if (ataque.getCd() >= 0.5f) { 
                ataque.bajarCDAtaque(floatValue);
                }
                break;
            case EffectType.ConfuseScreen:
                Debug.Log("Pantalla confusa (por implementar)");
                break;
        }
    }
}
