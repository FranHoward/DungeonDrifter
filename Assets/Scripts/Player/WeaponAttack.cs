using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    [SerializeField] private WeaponData weapon;
    [SerializeField] private LayerMask enemyLayer;
    private float lastAttackTime;
    private PlayerStats stats;

    public WeaponData CurrentWeapon => weapon;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    public void Equip(WeaponData newWeapon)
    {
        if (newWeapon == null)
            return;

        weapon = newWeapon;
        Debug.Log($"Equipped {weapon.weaponName}.");
    }

    private void Update()
    {
        float cooldownMultiplier = stats != null ? stats.CooldownMultiplier : 1f;
        float finalCooldown = weapon != null ? weapon.cooldown * cooldownMultiplier : 0f;

        if (weapon != null
            && Input.GetKeyDown(KeyCode.Space)
            && Time.time >= lastAttackTime + finalCooldown)
        {
            lastAttackTime = Time.time;
            float damageMultiplier = stats != null ? stats.DamageMultiplier : 1f;
            float rangeMultiplier = stats != null ? stats.RangeMultiplier : 1f;
            float finalDamage = weapon.damage * damageMultiplier;
            float finalRange = weapon.range * rangeMultiplier;
            var center = transform.position + transform.forward * finalRange * 0.5f;

            foreach (var hit in Physics.OverlapSphere(center, finalRange, enemyLayer))
            {
                Debug.Log($"{weapon.weaponName} deals {finalDamage:0.##} damage to {hit.name}");
                if (hit.TryGetComponent<Health>(out var health))
                    health.TakeDamage(finalDamage);
            }
        }
    }
}
