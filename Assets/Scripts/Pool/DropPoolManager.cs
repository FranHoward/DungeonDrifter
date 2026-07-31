using System.Collections.Generic;
using UnityEngine;

public class DropPoolManager : MonoBehaviour
{
    private const int DefaultPoolSize = 5;
    private static DropPoolManager instance;
    private readonly Dictionary<GameObject, ObjectPool> pools = new();

    public static DropPoolManager Instance
    {
        get
        {
            if (instance == null)
            {
                var managerObject = new GameObject(nameof(DropPoolManager));
                instance = managerObject.AddComponent<DropPoolManager>();
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void Spawn(DropTable.DropEntry entry, Vector3 position)
    {
        if (entry == null || entry.prefab == null)
            return;

        ObjectPool pool = GetOrCreatePool(entry.prefab);
        GameObject item = pool.Get(position + Vector3.up * 0.35f);
        if (item == null)
            return;

        if (!item.TryGetComponent(out PickupItem pickup))
        {
            Debug.LogError($"{entry.prefab.name} needs a PickupItem component.");
            pool.Return(item);
            return;
        }

        pickup.Initialize(entry.type, entry.amount, entry.weapon, pool);
    }

    private ObjectPool GetOrCreatePool(GameObject prefab)
    {
        if (pools.TryGetValue(prefab, out ObjectPool existingPool))
            return existingPool;

        var poolObject = new GameObject($"{prefab.name} Pool");
        poolObject.transform.SetParent(transform);
        ObjectPool newPool = poolObject.AddComponent<ObjectPool>();
        newPool.Configure(prefab, DefaultPoolSize);
        pools.Add(prefab, newPool);
        return newPool;
    }
}
