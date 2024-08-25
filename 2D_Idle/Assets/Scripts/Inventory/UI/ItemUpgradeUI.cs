using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Litkey.InventorySystem;
using Litkey.Utility;
using DG.Tweening;
using AssetKits.ParticleImage;

public class ItemUpgradeUI : MonoBehaviour
{
    [SerializeField] private DOTweenAnimation glowAnim;
    [SerializeField] private Image itemFrame;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemUpgradeProbabilityText;
    [SerializeField] private TextMeshProUGUI beforeUpgradeLevelText;
    [SerializeField] private TextMeshProUGUI afterUpgradeLevelText;
    [SerializeField] private TextMeshProUGUI beforeStat;
    [SerializeField] private TextMeshProUGUI afterStat;
    [SerializeField] private Image beforeStatBG;
    [SerializeField] private Image afterStatBG;
    [SerializeField] private Image arrow;

    [SerializeField] private Image requirementBG;
    [SerializeField] private TextMeshProUGUI requirementText;
    [SerializeField] private Button UpgradeButton;
    [SerializeField] private EquipmentRarityColor rarityColor;

    [SerializeField] private Inventory inventory;
    [SerializeField] private ParticleImage successParticle;

    public void ShowUpgradeWindow(EquipmentItem equipmentItem)
    {
        glowAnim.DORestart();
        itemFrame.color = rarityColor.GetColor(equipmentItem.EquipmentData.rarity);
        itemIcon.sprite = equipmentItem.EquipmentData.IconSprite;
        itemNameText.SetText($"{equipmentItem.EquipmentData.Name} +{equipmentItem.CurrentUpgrade}강");
        itemUpgradeProbabilityText.SetText("100%");

        beforeUpgradeLevelText.SetText($"{equipmentItem.CurrentUpgrade}강");
        afterUpgradeLevelText.SetText($"{equipmentItem.CurrentUpgrade+1}강");

        string statText = string.Empty;
        var finalStat = equipmentItem.GetFinalStats(OperatorType.plus);
        foreach (var stats in finalStat)
        {
            statText += $"{stats.Key} +{stats.Value}\n";
        }
        beforeStat.SetText(statText);
        var afterModifiers = equipmentItem.EquipmentData.UpgradeData.GetUpgradeModifiers(equipmentItem.CurrentUpgrade);

        foreach (var stat in afterModifiers)
        {
            if (!finalStat.ContainsKey(stat.statType))
            {
                finalStat.Add(stat.statType, stat.value);
            }
            finalStat[stat.statType] += stat.value;
        }
        statText = string.Empty;
        foreach (var stats in finalStat)
        {
            statText += $"{stats.Key} +{stats.Value}\n";
        }
        afterStat.color = Color.green;
        afterStat.SetText(statText);

        requirementBG.gameObject.SetActive(true);
        
        var requirement = equipmentItem.EquipmentData.UpgradeData.GetUpgradeRequirements(equipmentItem.CurrentUpgrade);
        if (requirement != null)
        {
            requirementText.SetText($"{requirement.requiredItems[0].item.Name} {requirement.requiredItems[0].quantity}개" +
                $"\n{requirement.goldCost} 골드");
        }

        UpgradeButton.gameObject.SetActive(true);
        UpgradeButton.onClick.RemoveAllListeners();
        UpgradeButton.onClick.AddListener(() =>
        {
            if (inventory.ContainsItem(requirement.requiredItems[0].item, requirement.requiredItems[0].quantity) 
            && ResourceManager.Instance.HasGold(requirement.goldCost))
            {
                if (equipmentItem.Upgrade())
                {
                    UpdateUpgradeStatus(equipmentItem);
                    successParticle.Play();
                    inventory.Save();
                }
            }
            else
            {
                WarningMessageInvoker.Instance.ShowMessage("아이템이 충분하지 않습니다");
            }
        });

        if (equipmentItem.CurrentUpgrade >= equipmentItem.EquipmentData.UpgradeData.MaxLevel)
        {
            arrow.gameObject.SetActive(false);
            afterUpgradeLevelText.gameObject.SetActive(false);
            requirementBG.gameObject.SetActive(false);
            UpgradeButton.gameObject.SetActive(false);
            afterStatBG.gameObject.SetActive(false);
        }
    }
    public void UpdateUpgradeStatus(EquipmentItem equipmentItem)
    {
        arrow.gameObject.SetActive(true);
        afterUpgradeLevelText.gameObject.SetActive(true);
        afterStatBG.gameObject.SetActive(true);
        requirementBG.gameObject.SetActive(true);
        UpgradeButton.gameObject.SetActive(true);

        glowAnim.DORestart();
        itemFrame.color = rarityColor.GetColor(equipmentItem.EquipmentData.rarity);
        itemIcon.sprite = equipmentItem.EquipmentData.IconSprite;
        itemNameText.SetText($"{equipmentItem.EquipmentData.Name} +{equipmentItem.CurrentUpgrade}강");
        itemUpgradeProbabilityText.SetText("100%");

        beforeUpgradeLevelText.SetText($"{equipmentItem.CurrentUpgrade}강");
        afterUpgradeLevelText.SetText($"{equipmentItem.CurrentUpgrade + 1}강");

        string statText = string.Empty;
        var finalStat = equipmentItem.GetFinalStats(OperatorType.plus);
        foreach (var stats in finalStat)
        {
            statText += $"{stats.Key} +{stats.Value}\n";
        }
        beforeStat.SetText(statText);
        var afterModifiers = equipmentItem.EquipmentData.UpgradeData.GetUpgradeModifiers(equipmentItem.CurrentUpgrade);

        foreach (var stat in afterModifiers)
        {
            if (!finalStat.ContainsKey(stat.statType))
            {
                finalStat.Add(stat.statType, stat.value);
            }
            finalStat[stat.statType] += stat.value;
        }
        statText = string.Empty;
        foreach (var stats in finalStat)
        {
            statText += $"{stats.Key} +{stats.Value}\n";
        }
        afterStat.color = Color.green;
        afterStat.SetText(statText);

        requirementBG.gameObject.SetActive(true);

        var requirement = equipmentItem.EquipmentData.UpgradeData.GetUpgradeRequirements(equipmentItem.CurrentUpgrade);
        if (requirement != null)
        {
            requirementText.SetText($"{requirement.requiredItems[0].item.Name} {requirement.requiredItems[0].quantity}개" +
                $"\n{requirement.goldCost} 골드");
        }

        UpgradeButton.gameObject.SetActive(true);

        if (equipmentItem.CurrentUpgrade >= equipmentItem.EquipmentData.UpgradeData.MaxLevel)
        {
            arrow.gameObject.SetActive(false);
            afterUpgradeLevelText.gameObject.SetActive(false);
            afterStatBG.gameObject.SetActive(false);
            requirementBG.gameObject.SetActive(false);
            UpgradeButton.gameObject.SetActive(false);
        }
    }
}
