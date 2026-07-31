using System;
using UnityEngine;

public enum DropType
{
    None,
    Coin,
    Health,
    Weapon
}

[CreateAssetMenu(fileName = "NewDropTable", menuName = "Dungeon Drifter/Drop Table")]
public class DropTable : ScriptableObject
{
    [Serializable]
    public class DropEntry
    {
        public DropType type;
        public GameObject prefab;
        [Min(0f)] public float weight = 1f;
        [Min(0f)] public float amount = 1f;
        public WeaponData weapon;
    }

    [SerializeField] private DropEntry[] entries = Array.Empty<DropEntry>();
    [Min(0f)] [SerializeField] private float noDropWeight;

    public bool TryRoll(out DropEntry selectedDrop)
    {
        selectedDrop = null;
        float totalWeight = noDropWeight;

        foreach (DropEntry entry in entries)
        {
            if (IsValid(entry))
                totalWeight += entry.weight;
        }

        if (totalWeight <= 0f)
            return false;

        float roll = UnityEngine.Random.Range(0f, totalWeight);
        if (roll < noDropWeight)
            return false;

        roll -= noDropWeight;
        foreach (DropEntry entry in entries)
        {
            if (!IsValid(entry))
                continue;

            if (roll < entry.weight)
            {
                selectedDrop = entry;
                return true;
            }

            roll -= entry.weight;
        }

        return false;
    }

    private static bool IsValid(DropEntry entry)
    {
        return entry != null
            && entry.type != DropType.None
            && entry.prefab != null
            && entry.weight > 0f;
    }
}
