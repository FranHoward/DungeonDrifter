using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float damageMult = 1f;
    public float speedMult = 1f;
    public float rangeMult = 1f;

    public void Apply(UpgradeData u)
    {
        switch(u.stat)
        {
            case StatType.Damage: damageMult *= u.multiplier; break;
            case StatType.MoveSpeed: speedMult *= u.multiplier; break;
            case StatType.AttackRange: rangeMult *= u.multiplier; break;
        }
    }
}
