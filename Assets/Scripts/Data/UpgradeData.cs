using UnityEngine;

public enum StatType
{
    Damage,
    MoveSpeed,
    AttackRange,
    AttackCooldown
}

[CreateAssetMenu(fileName = "Upgrade", menuName = "Game/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string title;
    [TextArea] public string description;
    public StatType stat;
    public float multiplier = 1.2f;
}
