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

    private void Awake()
    {
        equipmentSlot.OnEquip.AddListener(OnEquipped);
        equipmentSlot.OnUnEquip.AddListener(OnUnEquipped);
    }

    private void OnEquipped(EquipmentItem equipItem)
    {
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
