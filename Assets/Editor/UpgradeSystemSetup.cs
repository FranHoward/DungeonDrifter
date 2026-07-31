using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class UpgradeSystemSetup
{
    private const string GeneratedFolder = "Assets/Generated/UpgradeSystem";
    private const string PlayerPrefabPath = "Assets/Prefabs/Characters/Player.prefab";

    [InitializeOnLoadMethod]
    private static void SetupAutomatically()
    {
        EditorApplication.delayCall += () =>
        {
            if (AssetDatabase.LoadAssetAtPath<UpgradeData>(
                    GeneratedFolder + "/Damage20.asset") == null)
            {
                Setup();
            }
        };
    }

    [MenuItem("Tools/Dungeon Drifter/Setup Upgrade System")]
    public static void Setup()
    {
        EnsureFolder("Assets/Generated");
        EnsureFolder(GeneratedFolder);

        var upgrades = new List<UpgradeData>
        {
            CreateUpgrade("Damage20", "Sharpened Edge", "+20% attack damage",
                StatType.Damage, 1.2f),
            CreateUpgrade("Damage50", "Heavy Strikes", "+50% attack damage",
                StatType.Damage, 1.5f),
            CreateUpgrade("Speed20", "Swift Steps", "+20% movement speed",
                StatType.MoveSpeed, 1.2f),
            CreateUpgrade("Range25", "Long Reach", "+25% attack range",
                StatType.AttackRange, 1.25f),
            CreateUpgrade("Cooldown20", "Quick Hands", "20% shorter attack cooldown",
                StatType.AttackCooldown, 0.8f),
            CreateUpgrade("Cooldown40", "Battle Trance", "40% shorter attack cooldown",
                StatType.AttackCooldown, 0.6f)
        };

        ConfigurePlayerPrefab(upgrades.ToArray());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Upgrade system ready: every 3 enemy kills opens the three-card UI.");
    }

    private static UpgradeData CreateUpgrade(
        string fileName,
        string title,
        string description,
        StatType stat,
        float multiplier)
    {
        string path = $"{GeneratedFolder}/{fileName}.asset";
        UpgradeData upgrade = AssetDatabase.LoadAssetAtPath<UpgradeData>(path);
        if (upgrade == null)
        {
            upgrade = ScriptableObject.CreateInstance<UpgradeData>();
            AssetDatabase.CreateAsset(upgrade, path);
        }

        upgrade.title = title;
        upgrade.description = description;
        upgrade.stat = stat;
        upgrade.multiplier = multiplier;
        EditorUtility.SetDirty(upgrade);
        return upgrade;
    }

    private static void ConfigurePlayerPrefab(UpgradeData[] upgrades)
    {
        GameObject root = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
        try
        {
            PlayerStats stats = root.GetComponent<PlayerStats>();
            if (stats == null)
                stats = root.AddComponent<PlayerStats>();

            Attack duplicateAttack = root.GetComponent<Attack>();
            if (duplicateAttack != null)
                Object.DestroyImmediate(duplicateAttack);

            Transform oldSystem = root.transform.Find("UpgradeSystem");
            if (oldSystem != null)
                Object.DestroyImmediate(oldSystem.gameObject);

            GameObject systemObject = new GameObject("UpgradeSystem");
            systemObject.transform.SetParent(root.transform, false);
            UpgradeManager manager = systemObject.AddComponent<UpgradeManager>();

            UpgradeSelectionUI selectionUI =
                CreateUpgradeCanvas(systemObject.transform, stats);

            var serializedManager = new SerializedObject(manager);
            serializedManager.FindProperty("killsPerUpgrade").intValue = 3;
            SerializedProperty available =
                serializedManager.FindProperty("availableUpgrades");
            available.arraySize = upgrades.Length;
            for (int i = 0; i < upgrades.Length; i++)
                available.GetArrayElementAtIndex(i).objectReferenceValue = upgrades[i];
            serializedManager.FindProperty("selectionUI").objectReferenceValue =
                selectionUI;
            serializedManager.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static UpgradeSelectionUI CreateUpgradeCanvas(
        Transform parent,
        PlayerStats stats)
    {
        GameObject canvasObject = new GameObject(
            "UpgradeCanvas",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster),
            typeof(UpgradeSelectionUI));
        canvasObject.transform.SetParent(parent, false);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        GameObject panel = CreateUIObject(
            "UpgradePanel", canvasObject.transform, typeof(Image));
        Stretch(panel.GetComponent<RectTransform>());
        panel.GetComponent<Image>().color = new Color(0.025f, 0.035f, 0.07f, 0.94f);

        TMP_Text heading = CreateText(
            "Heading", panel.transform, "CHOOSE AN UPGRADE", 48,
            FontStyles.Bold, TextAlignmentOptions.Center);
        SetRect(heading.rectTransform, new Vector2(0.5f, 1f),
            new Vector2(0f, -95f), new Vector2(900f, 80f));

        TMP_Text subtitle = CreateText(
            "Subtitle", panel.transform,
            "Defeat enemies, grow stronger, keep drifting.", 24,
            FontStyles.Normal, TextAlignmentOptions.Center);
        subtitle.color = new Color(0.65f, 0.72f, 0.85f);
        SetRect(subtitle.rectTransform, new Vector2(0.5f, 1f),
            new Vector2(0f, -165f), new Vector2(900f, 50f));

        var cards = new UpgradeCardUI[3];
        float[] xPositions = { -420f, 0f, 420f };
        for (int i = 0; i < cards.Length; i++)
            cards[i] = CreateCard(panel.transform, i + 1, xPositions[i]);

        UpgradeSelectionUI selectionUI =
            canvasObject.GetComponent<UpgradeSelectionUI>();
        var serializedUI = new SerializedObject(selectionUI);
        serializedUI.FindProperty("panel").objectReferenceValue = panel;
        SerializedProperty cardArray = serializedUI.FindProperty("cards");
        cardArray.arraySize = cards.Length;
        for (int i = 0; i < cards.Length; i++)
            cardArray.GetArrayElementAtIndex(i).objectReferenceValue = cards[i];
        serializedUI.FindProperty("playerStats").objectReferenceValue = stats;
        serializedUI.ApplyModifiedPropertiesWithoutUndo();

        panel.SetActive(false);
        return selectionUI;
    }

    private static UpgradeCardUI CreateCard(
        Transform parent,
        int number,
        float xPosition)
    {
        GameObject card = CreateUIObject(
            $"UpgradeCard{number}", parent, typeof(Image), typeof(Button),
            typeof(UpgradeCardUI));
        RectTransform cardRect = card.GetComponent<RectTransform>();
        SetRect(cardRect, new Vector2(0.5f, 0.5f),
            new Vector2(xPosition, -35f), new Vector2(350f, 500f));

        Image image = card.GetComponent<Image>();
        image.color = new Color(0.1f, 0.14f, 0.25f, 1f);

        Button button = card.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.75f, 0.87f, 1f);
        colors.pressedColor = new Color(0.55f, 0.72f, 1f);
        button.colors = colors;

        TMP_Text title = CreateText(
            "Title", card.transform, "Upgrade", 34,
            FontStyles.Bold, TextAlignmentOptions.Center);
        title.color = new Color(1f, 0.82f, 0.28f);
        SetRect(title.rectTransform, new Vector2(0.5f, 1f),
            new Vector2(0f, -100f), new Vector2(310f, 100f));

        TMP_Text description = CreateText(
            "Description", card.transform, "Description", 26,
            FontStyles.Normal, TextAlignmentOptions.Center);
        description.color = new Color(0.9f, 0.93f, 1f);
        SetRect(description.rectTransform, new Vector2(0.5f, 0.5f),
            new Vector2(0f, -35f), new Vector2(290f, 180f));

        TMP_Text prompt = CreateText(
            "Prompt", card.transform, "CLICK TO SELECT", 20,
            FontStyles.Bold, TextAlignmentOptions.Center);
        prompt.color = new Color(0.48f, 0.76f, 1f);
        SetRect(prompt.rectTransform, new Vector2(0.5f, 0f),
            new Vector2(0f, 55f), new Vector2(280f, 40f));

        UpgradeCardUI cardUI = card.GetComponent<UpgradeCardUI>();
        var serializedCard = new SerializedObject(cardUI);
        serializedCard.FindProperty("titleText").objectReferenceValue = title;
        serializedCard.FindProperty("descriptionText").objectReferenceValue =
            description;
        serializedCard.FindProperty("button").objectReferenceValue = button;
        serializedCard.ApplyModifiedPropertiesWithoutUndo();
        return cardUI;
    }

    private static TMP_Text CreateText(
        string name,
        Transform parent,
        string text,
        float size,
        FontStyles style,
        TextAlignmentOptions alignment)
    {
        GameObject textObject = CreateUIObject(
            name, parent, typeof(TextMeshProUGUI));
        var label = textObject.GetComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = size;
        label.fontStyle = style;
        label.alignment = alignment;
        label.raycastTarget = false;
        return label;
    }

    private static GameObject CreateUIObject(
        string name,
        Transform parent,
        params System.Type[] components)
    {
        var types = new List<System.Type> { typeof(RectTransform) };
        types.AddRange(components);
        var gameObject = new GameObject(name, types.ToArray());
        gameObject.transform.SetParent(parent, false);
        return gameObject;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void SetRect(
        RectTransform rect,
        Vector2 anchor,
        Vector2 position,
        Vector2 size)
    {
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
        AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
    }
}
