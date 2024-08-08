using Litkey.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ShowItemInformationOnClick : MonoBehaviour
{
    private ItemSlotUI itemSlotUI;
    private Button slotButton;

    private EquipmentSlotUI equipmentSlotUI;

    public bool onlyDisplayPurpose;
    private void Awake()
    {
        itemSlotUI = GetComponent<ItemSlotUI>();
        slotButton = GetComponent<Button>();
        if (itemSlotUI != null)
        {
            slotButton.onClick.AddListener(() =>
            {
                itemSlotUI.SelectSlot();
                ItemInformationWindow.Instance.ShowItemInfo(itemSlotUI.EquippedItem, !onlyDisplayPurpose, () =>
                {
                    itemSlotUI.ResetClickState();
                });
            });
        }

        equipmentSlotUI = GetComponent<EquipmentSlotUI>();
        if (equipmentSlotUI != null)
        {
            slotButton.onClick.AddListener(() =>
            {
                //itemSlotUI.SelectSlot();
                if (equipmentSlotUI.EquippedItem == null) return;
                ItemInformationWindow.Instance.ShowItemInfo(equipmentSlotUI.EquippedItem, !onlyDisplayPurpose, () =>
                {
                    
                });
            });
        }
    }
    
}
