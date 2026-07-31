using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSize = 10;
    private readonly Queue<GameObject> pool = new();
    private bool initialized;

    private void Awake()
    {
        Initialize();
    }

    public void Configure(GameObject pooledPrefab, int size)
    {
        prefab = pooledPrefab;
        initialSize = Mathf.Max(0, size);
        Initialize();
    }

    private void Initialize()
    {
        if (initialized || prefab == null)
            return;

        initialized = true;
        for (int i = 0; i < initialSize; i++)
            pool.Enqueue(CreateNew());
    }

    private GameObject CreateNew()
    {
        var obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        return obj;
    }

    public GameObject Get(Vector3 pos)
    {
        Initialize();
        if (prefab == null)
            return null;

        var obj = pool.Count > 0 ? pool.Dequeue() : CreateNew();
        obj.transform.position = pos;
        obj.transform.rotation = Quaternion.identity;
        obj.SetActive(true);
        return obj;
    }

    public void Return(GameObject obj)
    {
        if (obj == null || !obj.activeSelf)
            return;

        obj.SetActive(false);
        obj.transform.SetParent(transform);
        pool.Enqueue(obj);
    }
}
