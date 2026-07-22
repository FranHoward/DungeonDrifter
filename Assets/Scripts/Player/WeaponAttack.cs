using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    [SerializeField] private WeaponData weapon;
    [SerializeField] private LayerMask enemyLayer;
    private float lastAttackTime;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastAttackTime + weapon.cooldown)
        {
            lastAttackTime = Time.time;
            var center = transform.position + transform.forward * weapon.range * 0.5f;
            foreach (var hit in Physics.OverlapSphere(center, weapon.range, enemyLayer))
            {
                Debug.Log($"{weapon.weaponName} 造成 {weapon.damage} 伤害给 {hit.name}");
                if (hit.TryGetComponent<Health>(out var health))
                    health.TakeDamage(weapon.damage);
            }
        }
    }
}
