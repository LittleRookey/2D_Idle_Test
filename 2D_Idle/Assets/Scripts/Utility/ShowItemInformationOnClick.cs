using Litkey.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(ItemSlotUI))]
public class ShowItemInformationOnClick : MonoBehaviour
{
    private ItemSlotUI itemSlotUI;
    private Button slotButton;

    private void Awake()
    {
        itemSlotUI = GetComponent<ItemSlotUI>();
        slotButton = GetComponent<Button>();
        slotButton.onClick.AddListener(() =>
        {
            itemSlotUI.SelectSlot();
            ItemInformationWindow.Instance.ShowItemInfo(itemSlotUI.EquippedItem, true, () =>
            {
                itemSlotUI.ResetClickState();
            });
        });
    }
    
}
