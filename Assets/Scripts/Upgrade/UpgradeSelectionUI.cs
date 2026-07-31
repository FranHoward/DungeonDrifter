using UnityEngine;

public class UpgradeSelectionUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private UpgradeCardUI[] cards;
    [SerializeField] private PlayerStats playerStats;

    private bool isOpen;

    public void Show(UpgradeData[] choices)
    {
        if (isOpen || choices == null || choices.Length < cards.Length)
            return;

        if (playerStats == null)
            playerStats = FindFirstObjectByType<PlayerStats>();

        if (playerStats == null)
        {
            Debug.LogError("Upgrade UI could not find PlayerStats.");
            return;
        }

        isOpen = true;
        panel.SetActive(true);
        Time.timeScale = 0f;

        for (int i = 0; i < cards.Length; i++)
            cards[i].Setup(choices[i], SelectUpgrade);
    }

    private void SelectUpgrade(UpgradeData upgrade)
    {
        if (!isOpen)
            return;

        playerStats.Apply(upgrade);
        isOpen = false;
        panel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        if (isOpen)
            Time.timeScale = 1f;
    }
}
