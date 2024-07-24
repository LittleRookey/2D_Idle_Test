using Litkey.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowItemInfoOnShop : MonoBehaviour
{
    private ShopSlotUI shopSlotUI;
    private Button slotButton;
    private void Awake()
    {
        shopSlotUI = GetComponent<ShopSlotUI>();
        slotButton = GetComponent<Button>();
        slotButton.onClick.AddListener(() =>
        {
            ItemInformationWindow.Instance.ShowItemInfo(shopSlotUI.Product, () =>
            {
                shopSlotUI.DeselectSlot();
            });
        });
    }
}
