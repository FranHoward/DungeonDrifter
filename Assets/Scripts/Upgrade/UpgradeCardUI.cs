using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Button button;

    public void Setup(UpgradeData upgrade, Action<UpgradeData> onSelected)
    {
        titleText.text = upgrade.title;
        descriptionText.text = upgrade.description;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onSelected(upgrade));
    }
}
