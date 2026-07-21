using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public float damage;
    public float range;
    public float cooldown;
}
