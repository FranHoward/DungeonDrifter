using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    [SerializeField] private WeaponData weapon;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private EnemyHitFeedback hitFeedback;
    private float lastAttackTime;
    private PlayerStats stats;
    private readonly HashSet<Health> damagedEnemies = new HashSet<Health>();

    public WeaponData CurrentWeapon => weapon;
    public event Action<WeaponData> OnAttack;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        if (hitFeedback == null)
            hitFeedback = GetComponent<EnemyHitFeedback>();
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

            OnAttack?.Invoke(weapon);

            damagedEnemies.Clear();
            foreach (var hit in Physics.OverlapSphere(center, finalRange, enemyLayer))
            {
                Health health = hit.GetComponentInParent<Health>();
                if (health == null || health.IsDead || !damagedEnemies.Add(health))
                    continue;

                Vector3 hitPosition = hit.ClosestPoint(transform.position);
                Vector3 impactDirection = (hit.bounds.center - transform.position).normalized;

                Debug.Log($"{weapon.weaponName} deals {finalDamage:0.##} damage to {health.name}");
                health.TakeDamage(finalDamage);
                hitFeedback?.Play(hitPosition, impactDirection);
            }
        }
    }
}
