using System.IO;
using UnityEditor;
using UnityEngine;

public static class DropSystemSetup
{
    private const string GeneratedFolder = "Assets/Generated/DropSystem";
    private const string DropTablePath = GeneratedFolder + "/DefaultDropTable.asset";
    private const string EnemyPrefabPath = "Assets/Prefabs/Characters/Enemy.prefab";
    private const string PlayerPrefabPath = "Assets/Prefabs/Characters/Player.prefab";

    [InitializeOnLoadMethod]
    private static void SetupAutomatically()
    {
        EditorApplication.delayCall += () =>
        {
            if (AssetDatabase.LoadAssetAtPath<DropTable>(DropTablePath) == null)
                Setup();
        };
    }

    [MenuItem("Tools/Dungeon Drifter/Setup Drop System")]
    public static void Setup()
    {
        EnsureFolder("Assets/Generated");
        EnsureFolder(GeneratedFolder);

        GameObject coinPrefab = CreatePickupPrefab(
            "CoinPickup", PrimitiveType.Cylinder, new Color(1f, 0.72f, 0.05f));
        GameObject healthPrefab = CreatePickupPrefab(
            "HealthPickup", PrimitiveType.Sphere, new Color(0.15f, 0.9f, 0.25f));
        GameObject weaponPrefab = CreatePickupPrefab(
            "WeaponPickup", PrimitiveType.Cube, new Color(0.2f, 0.55f, 1f));

        DropTable dropTable = AssetDatabase.LoadAssetAtPath<DropTable>(DropTablePath);
        if (dropTable == null)
        {
            dropTable = ScriptableObject.CreateInstance<DropTable>();
            AssetDatabase.CreateAsset(dropTable, DropTablePath);
        }

        WeaponData sword = AssetDatabase.LoadAssetAtPath<WeaponData>("Assets/Data/Sword.asset");
        WeaponData bow = AssetDatabase.LoadAssetAtPath<WeaponData>("Assets/Data/Bow.asset");
        WeaponData hammer = AssetDatabase.LoadAssetAtPath<WeaponData>("Assets/Data/Hammer.asset");
        ConfigureDropTable(
            dropTable, coinPrefab, healthPrefab, weaponPrefab, sword, bow, hammer);
        ConfigurePlayerPrefab();
        ConfigureEnemyPrefab(dropTable);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Drop system ready: kill an enemy, walk over its drop, and the item is used.");
    }

    private static GameObject CreatePickupPrefab(
        string name,
        PrimitiveType primitiveType,
        Color color)
    {
        string prefabPath = $"{GeneratedFolder}/{name}.prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (existing != null)
            return existing;

        GameObject pickup = GameObject.CreatePrimitive(primitiveType);
        pickup.name = name;
        pickup.transform.localScale = primitiveType == PrimitiveType.Cylinder
            ? new Vector3(0.6f, 0.12f, 0.6f)
            : Vector3.one * 0.6f;

        Collider collider = pickup.GetComponent<Collider>();
        collider.isTrigger = true;

        Rigidbody body = pickup.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;

        pickup.AddComponent<PickupItem>();

        var material = new Material(Shader.Find("Universal Render Pipeline/Lit"))
        {
            color = color
        };
        string materialPath = $"{GeneratedFolder}/{name}.mat";
        AssetDatabase.CreateAsset(material, materialPath);
        pickup.GetComponent<Renderer>().sharedMaterial = material;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(pickup, prefabPath);
        Object.DestroyImmediate(pickup);
        return prefab;
    }

    private static void ConfigureDropTable(
        DropTable table,
        GameObject coin,
        GameObject health,
        GameObject weaponPrefab,
        WeaponData sword,
        WeaponData bow,
        WeaponData hammer)
    {
        var serializedTable = new SerializedObject(table);
        SerializedProperty entries = serializedTable.FindProperty("entries");
        entries.arraySize = 5;

        SetEntry(entries.GetArrayElementAtIndex(0), DropType.Coin, coin, 50f, 1f, null);
        SetEntry(entries.GetArrayElementAtIndex(1), DropType.Health, health, 35f, 25f, null);
        SetEntry(entries.GetArrayElementAtIndex(2), DropType.Weapon, weaponPrefab, 6.75f, 1f, sword);
        SetEntry(entries.GetArrayElementAtIndex(3), DropType.Weapon, weaponPrefab, 6f, 1f, bow);
        SetEntry(entries.GetArrayElementAtIndex(4), DropType.Weapon, weaponPrefab, 2.25f, 1f, hammer);
        serializedTable.FindProperty("noDropWeight").floatValue = 0f;
        serializedTable.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(table);
    }

    private static void SetEntry(
        SerializedProperty entry,
        DropType type,
        GameObject prefab,
        float weight,
        float amount,
        WeaponData weapon)
    {
        entry.FindPropertyRelative("type").enumValueIndex = (int)type;
        entry.FindPropertyRelative("prefab").objectReferenceValue = prefab;
        entry.FindPropertyRelative("weight").floatValue = weight;
        entry.FindPropertyRelative("amount").floatValue = amount;
        entry.FindPropertyRelative("weapon").objectReferenceValue = weapon;
    }

    private static void ConfigurePlayerPrefab()
    {
        GameObject root = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
        try
        {
            if (root.GetComponent<PlayerInventory>() == null)
                root.AddComponent<PlayerInventory>();

            PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ConfigureEnemyPrefab(DropTable table)
    {
        GameObject root = PrefabUtility.LoadPrefabContents(EnemyPrefabPath);
        try
        {
            EnemyDrop enemyDrop = root.GetComponent<EnemyDrop>();
            if (enemyDrop == null)
                enemyDrop = root.AddComponent<EnemyDrop>();

            var serializedDrop = new SerializedObject(enemyDrop);
            serializedDrop.FindProperty("dropTable").objectReferenceValue = table;
            serializedDrop.ApplyModifiedPropertiesWithoutUndo();
            PrefabUtility.SaveAsPrefabAsset(root, EnemyPrefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
        string folderName = Path.GetFileName(path);
        AssetDatabase.CreateFolder(parent, folderName);
    }
}
