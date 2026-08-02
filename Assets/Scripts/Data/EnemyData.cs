using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    [Min(1f)] public float maxHealth = 55f;
    [Min(0f)] public float damage = 6f;
    [Min(0.05f)] public float attackCooldown = 1.35f;
    [Min(0.1f)] public float attackRange = 1.35f;
    [Min(0.1f)] public float moveSpeed = 3.2f;
}
