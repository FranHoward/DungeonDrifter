using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int coins;

    public int Coins => coins;
    public event Action OnItemCollected;

    public void AddCoins(int amount)
    {
        coins += Mathf.Max(0, amount);
        Debug.Log($"Picked up {amount} coin(s). Total coins: {coins}.");
        OnItemCollected?.Invoke();
    }

    public bool EquipWeapon(WeaponData weapon)
    {
        if (weapon == null || !TryGetComponent(out WeaponAttack weaponAttack))
            return false;

        weaponAttack.Equip(weapon);
        OnItemCollected?.Invoke();
        return true;
    }
}
