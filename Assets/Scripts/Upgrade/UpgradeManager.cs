using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    private static UpgradeManager instance;

    [SerializeField] private int killsPerUpgrade = 3;
    [SerializeField] private UpgradeData[] availableUpgrades;
    [SerializeField] private UpgradeSelectionUI selectionUI;

    private int killCount;

    public static UpgradeManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType<UpgradeManager>();
            return instance;
        }
    }

    public int KillCount => killCount;
    public int KillsUntilUpgrade =>
        Mathf.Max(0, killsPerUpgrade - killCount % killsPerUpgrade);

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void RegisterKill()
    {
        killCount++;
        Debug.Log($"Enemy defeated: {killCount}. Upgrade in {KillsUntilUpgrade} kill(s).");

        if (killsPerUpgrade > 0 && killCount % killsPerUpgrade == 0)
            OfferUpgrade();
    }

    private void OfferUpgrade()
    {
        UpgradeData[] choices = GetUniqueChoices(3);
        if (choices.Length < 3)
        {
            Debug.LogError("The upgrade manager needs at least three valid upgrades.");
            return;
        }

        if (selectionUI == null)
            selectionUI = FindFirstObjectByType<UpgradeSelectionUI>(FindObjectsInactive.Include);

        if (selectionUI == null)
        {
            Debug.LogError("No UpgradeSelectionUI was found.");
            return;
        }

        selectionUI.Show(choices);
    }

    private UpgradeData[] GetUniqueChoices(int count)
    {
        var candidates = new List<UpgradeData>();
        foreach (UpgradeData upgrade in availableUpgrades)
        {
            if (upgrade != null)
                candidates.Add(upgrade);
        }

        for (int i = 0; i < candidates.Count; i++)
        {
            int randomIndex = Random.Range(i, candidates.Count);
            (candidates[i], candidates[randomIndex]) =
                (candidates[randomIndex], candidates[i]);
        }

        int resultCount = Mathf.Min(count, candidates.Count);
        return candidates.GetRange(0, resultCount).ToArray();
    }
}
