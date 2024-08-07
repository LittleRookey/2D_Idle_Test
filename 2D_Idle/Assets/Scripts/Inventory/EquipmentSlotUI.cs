using Litkey.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour
{
    [SerializeField] private EquipmentSlot equipmentSlot;
    [SerializeField] private Image icon;
    [SerializeField] private Image backImage;

    public EquipmentItem EquippedItem
    {
        get
        {
            if (!equipmentSlot.IsEquipped) return null;
            return equipmentSlot.EquippedItem;
        }
    }
    private void Awake()
    {
        Debug.Log("장비창 UI 이벤트함수 등록 완료");
        equipmentSlot.OnEquip.AddListener(OnEquipped);
        equipmentSlot.OnUnEquip.AddListener(OnUnEquipped);
    }

    private void OnEnable()
    {
        if (equipmentSlot.IsEquipped)
        {
            OnEquipped(equipmentSlot.EquippedItem);
        }
    }
    private void OnEquipped(EquipmentItem equipItem)
    {
        if (equipItem == null)
        {
            Debug.LogError("EquipItem is null");
            return;
        }
        Debug.Log($"equipItem: {equipItem.EquipmentData.Name}");
        icon.gameObject.SetActive(true);
        icon.sprite = equipItem.EquipmentData.IconSprite;
        backImage.gameObject.SetActive(false);
    }

    private void OnUnEquipped(EquipmentItem equipItem)
    {
        icon.sprite = null;
        icon.gameObject.SetActive(false);
        backImage.gameObject.SetActive(true);
    }
}
