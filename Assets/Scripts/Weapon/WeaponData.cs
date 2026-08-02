using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    [Min(0f)]
    public float damage;
    [Min(0.1f)]
    public float range;
    [Min(0.05f)]
    public float cooldown;
}
