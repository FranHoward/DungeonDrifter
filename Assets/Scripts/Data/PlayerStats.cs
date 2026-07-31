using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float damageMultiplier = 1f;
    [SerializeField] private float speedMultiplier = 1f;
    [SerializeField] private float rangeMultiplier = 1f;
    [SerializeField] private float cooldownMultiplier = 1f;

    public float DamageMultiplier => damageMultiplier;
    public float SpeedMultiplier => speedMultiplier;
    public float RangeMultiplier => rangeMultiplier;
    public float CooldownMultiplier => cooldownMultiplier;

    public event Action OnStatsChanged;

    public void Apply(UpgradeData upgrade)
    {
        if (upgrade == null)
            return;

        switch (upgrade.stat)
        {
            case StatType.Damage:
                damageMultiplier *= upgrade.multiplier;
                break;
            case StatType.MoveSpeed:
                speedMultiplier *= upgrade.multiplier;
                break;
            case StatType.AttackRange:
                rangeMultiplier *= upgrade.multiplier;
                break;
            case StatType.AttackCooldown:
                cooldownMultiplier *= upgrade.multiplier;
                break;
        }

        OnStatsChanged?.Invoke();
        Debug.Log(
            $"Upgrade applied: {upgrade.title}. " +
            $"Damage x{damageMultiplier:0.##}, Speed x{speedMultiplier:0.##}, " +
            $"Range x{rangeMultiplier:0.##}, Cooldown x{cooldownMultiplier:0.##}");
    }
}
