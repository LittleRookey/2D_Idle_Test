using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Litkey.InventorySystem;
using Litkey.Utility;
using DG.Tweening;
using AssetKits.ParticleImage;
using UnityEngine.Events;

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

    public void ShowUpgradeWindow(EquipmentItem equipmentItem, UnityAction OnUpgrade=null)
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
        afterUpgradeLevelText.SetText($"{equipmentItem.CurrentUpgrade+1}강");

        string statText = string.Empty;
        var finalStat = equipmentItem.GetFinalStats(OperatorType.plus);
        foreach (var stats in finalStat)
        {
            statText += $"{stats.Key} +{stats.Value}\n";
        }
        beforeStat.SetText(statText);
        var afterModifiers = equipmentItem.GetNextUpgradeStat();

        foreach (var stat in afterModifiers)
        {
            if (!finalStat.ContainsKey(stat.statType))
            {
                finalStat.Add(stat.statType, 0f);
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
        
        var requirement = equipmentItem.GetNextUpgradeRequirements();

        if (requirement != null)
        {
            string requirementString = string.Empty;
            for (int i = 0; i < requirement.requiredItems.Count; i++)
            {
                requirementString += $"{requirement.requiredItems[i].item.Name} {requirement.requiredItems[i].quantity}개\n";
            }
            requirementString += $"{requirement.goldCost} 골드";
            requirementText.SetText(requirementString);
        }

        UpgradeButton.gameObject.SetActive(true);
        UpgradeButton.onClick.RemoveAllListeners();
        UpgradeButton.onClick.AddListener(() =>
        {
            bool meetsAllRequirement = true;
            var requirements = equipmentItem.GetNextUpgradeRequirements();
            // 강화 재료가 충분하지 않을때 
            for (int i = 0; i < requirements.requiredItems.Count; i++)
            {
                if (!inventory.ContainsItem(requirements.requiredItems[i].item, requirements.requiredItems[i].quantity))
                {
                    meetsAllRequirement = false;
                }
            }
            // 강화 비용이 충분하지 않을떄
            if (!ResourceManager.Instance.HasGold(requirements.goldCost))
            {
                meetsAllRequirement = false;
            }

            if (meetsAllRequirement)
            {
                // 강화 재료들과 골드를 사용
                for (int i = 0; i < requirements.requiredItems.Count; i++)
                {
                    inventory.UseItem(requirements.requiredItems[i].item, requirements.requiredItems[i].quantity);
                }
                ResourceManager.Instance.UseGold(requirements.goldCost);
                // 장비를 잠시 뺐다가 다시 낀다.
                bool wasEquipped = false;
                if (inventory.IsEquipped(equipmentItem))
                {
                    wasEquipped = true;
                    inventory.EquipEquipment(equipmentItem);
                }
                if (equipmentItem.Upgrade())
                {
                    UpdateUpgradeStatus(equipmentItem);
                    successParticle.Play();
                    OnUpgrade?.Invoke();
                }
                if (wasEquipped)
                {
                    // 만약 장비가 장착돼있었으면 한번 뺏었으니까 다시 장착하기
                    inventory.EquipEquipment(equipmentItem);
                }
                inventory.Save();
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
        var afterModifiers = equipmentItem.GetNextUpgradeStat();

        foreach (var stat in afterModifiers)
        {
            if (!finalStat.ContainsKey(stat.statType))
            {
                finalStat.Add(stat.statType, 0f);
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

        var requirement = equipmentItem.GetNextUpgradeRequirements();

        if (requirement != null)
        {
            string requirementString = string.Empty;
            for (int i = 0; i < requirement.requiredItems.Count; i++)
            {
                requirementString += $"{requirement.requiredItems[i].item.Name} {requirement.requiredItems[i].quantity}개\n";
            }
            requirementString += $"{requirement.goldCost} 골드";
            requirementText.SetText(requirementString);
        }

        UpgradeButton.gameObject.SetActive(true);
       

        if (equipmentItem.CurrentUpgrade >= equipmentItem.EquipmentData.UpgradeData.MaxLevel)
        {
            arrow.gameObject.SetActive(false);
            afterUpgradeLevelText.gameObject.SetActive(false);
            requirementBG.gameObject.SetActive(false);
            UpgradeButton.gameObject.SetActive(false);
            afterStatBG.gameObject.SetActive(false);
        }
    }
}
