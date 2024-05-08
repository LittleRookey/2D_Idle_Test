using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class EquipmentUpgradeSlot : MonoBehaviour
{
    [SerializeField] private RectTransform badge;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemExplanationText;
    [SerializeField] private TextMeshProUGUI goldUpgradeAmountText;
    [SerializeField] private TextMeshProUGUI badgeText;
    [SerializeField] private Button upgradeButton;

    private bool isSetup;
    public void Setup(EquipmentTier eTier, UnityAction<EquipmentTier> ac = null)
    {
        icon.sprite = eTier.equipmentSprite[0];
        badge.gameObject.SetActive(true);
        badgeText.SetText("x1");
        itemNameText.SetText($"{eTier.equipmentName} +{eTier.totalUpgradeLevel}");
        itemExplanationText.SetText(eTier.itemExplanation);
        goldUpgradeAmountText.SetText(eTier.requiredGold.ToString("N0"));

        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(() => ac(eTier));
        isSetup = true;
    }

    public void UpdateEquipmentTier(EquipmentTier eTier)
    {
        itemNameText.SetText($"{eTier.equipmentName} +{eTier.totalUpgradeLevel}");
        itemExplanationText.SetText(eTier.itemExplanation);
        goldUpgradeAmountText.SetText(eTier.requiredGold.ToString("N0"));
    }

    
    public bool IsEmpty()
    {
        return !isSetup;
    }
}
