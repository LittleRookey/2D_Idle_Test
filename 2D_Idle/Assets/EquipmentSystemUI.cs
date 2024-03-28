using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentSystemUI : MonoBehaviour
{
    [SerializeField] private GameObject orientation;
    [SerializeField] private RectTransform upgradeSlotParent;
    [SerializeField] private TextMeshProUGUI totalGoldText;

    [SerializeField] private EquipmentUpgradeSlot equipmentUpgradePrefab;
    [SerializeField] private EquipmentSystem eSystem;

    Dictionary<EquipmentTier, bool> eHasSlot;

    private void Awake()
    {
        eHasSlot = new Dictionary<EquipmentTier, bool>();
    }

    public void Turn()
    {
        if (orientation.activeInHierarchy) orientation.SetActive(false);
        else orientation.SetActive(true);

        if (orientation.activeInHierarchy)
        {
            SetupEquipments(eSystem.weapon);
            SetupEquipments(eSystem.topArmor);

            // 골드량 업데이트
            UpdateTotalGold();
        } else
        {
            for (int i = 0; i < upgradeSlotParent.childCount; i++)
            {
                upgradeSlotParent.GetChild(i).gameObject.SetActive(false);
            }
        }

    }

    public void SetupEquipments(EquipmentTier eTier)
    {
        var slot = GetEmptySlot();
        slot.gameObject.SetActive(true);
        slot.Setup(eTier, (eTier) =>
        {

            if (eSystem.UpgradeWeapon(eTier))
            {
                slot.UpdateEquipmentTier(eTier);
                UpdateTotalGold();
            }
        });
    }


    private EquipmentUpgradeSlot GetEmptySlot()
    {
        for (int i = 0; i < upgradeSlotParent.childCount; i++)
        {
            if (!upgradeSlotParent.GetChild(i).gameObject.activeInHierarchy)
            {
                return upgradeSlotParent.GetChild(i).GetComponent<EquipmentUpgradeSlot>();
            }
        }
        // 남은 빈 슬롯이 없다면
        return Instantiate(equipmentUpgradePrefab, upgradeSlotParent.transform);
    }

    private void UpdateTotalGold()
    {
        totalGoldText.SetText(ResourceManager.Instance.Gold.ToString("N0"));
    }
}
