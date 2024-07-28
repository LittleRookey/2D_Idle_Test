using Litkey.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowItemInfoOnShop : MonoBehaviour
{
    private ShopSlotUI shopSlotUI;
    [SerializeField] private Button slotButton;
    private void Awake()
    {
        shopSlotUI = GetComponent<ShopSlotUI>();
        if (slotButton == null) slotButton = GetComponent<Button>();

        slotButton.onClick.AddListener(() =>
        {
            ItemInformationWindow.Instance.ShowItemInfo(shopSlotUI.Product, () =>
            {
                //shopSlotUI.DeselectSlot();
            });
        });
    }
}
