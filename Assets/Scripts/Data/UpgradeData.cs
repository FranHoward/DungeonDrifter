using UnityEngine;

public enum StatType { Damage, MoveSpeed, AttackRange }

[CreateAssetMenu(fileName = "Upgrade", menuName = "Game/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string title;
    public StatType stat;
    public float multiplier = 1.2f;
}
