using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PickupItem : MonoBehaviour
{
    private DropType type;
    private float amount;
    private WeaponData weapon;
    private ObjectPool ownerPool;
    private bool collected;

    public void Initialize(
        DropType dropType,
        float dropAmount,
        WeaponData weaponData,
        ObjectPool pool)
    {
        type = dropType;
        amount = dropAmount;
        weapon = weaponData;
        ownerPool = pool;
        collected = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected || !other.CompareTag("Player"))
            return;

        Health health = other.GetComponentInParent<Health>();
        PlayerInventory inventory = other.GetComponentInParent<PlayerInventory>();
        bool used = false;

        switch (type)
        {
            case DropType.Health:
                if (health != null && health.CurrentHealth < health.MaxHealth)
                {
                    health.Heal(amount);
                    Debug.Log($"Picked up {amount} health.");
                    used = true;
                }
                break;

            case DropType.Coin:
                if (inventory != null)
                {
                    inventory.AddCoins(Mathf.Max(1, Mathf.RoundToInt(amount)));
                    used = true;
                }
                break;

            case DropType.Weapon:
                used = inventory != null && inventory.EquipWeapon(weapon);
                break;
        }

        if (used)
            ReturnToPool();
    }

    private void ReturnToPool()
    {
        collected = true;
        if (ownerPool != null)
            ownerPool.Return(gameObject);
        else
            gameObject.SetActive(false);
    }
}
