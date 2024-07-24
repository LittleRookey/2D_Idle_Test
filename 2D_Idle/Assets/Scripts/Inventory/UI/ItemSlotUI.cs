using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System;

namespace Litkey.InventorySystem
{
    public class ItemSlotUI : MonoBehaviour
    {
        [SerializeField] private Image slotBG;
        [SerializeField] private Image iconImage;
        [SerializeField] private Image selectedBG;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private Button slotButton;

        [SerializeField] private Image equippedImage;
        [SerializeField] private Image highlightImage;
        [SerializeField] private Image itemNameBG;

        [SerializeField] private float highlightAlpha;
        [SerializeField] private EquipmentRarityColor eRarityColor;

        private bool isDirty;
        public bool IsDirty => isDirty;

        public bool HasItem => iconImage != null;

        public Item EquippedItem { get; private set; }
        public void SetDirty()
        {
            isDirty = true;
        }

        public void ResetClickState()
        {
            selectedBG.gameObject.SetActive(false);
            highlightImage.gameObject.SetActive(false);
            itemNameBG.gameObject.SetActive(false);
        }

        public void SetSlot(Item item)
        {
            EquippedItem = item;
            if (item is EquipmentItem equipItem)
            {
                iconImage.sprite = equipItem.EquipmentData.IconSprite;
                var rarityColor = eRarityColor.GetColor(equipItem.EquipmentData.rarity);
                
                slotBG.color = rarityColor;

                itemNameText.color = rarityColor;
                itemNameText.SetText($"{TMProUtility.GetColorText($"[{equipItem.EquipmentData.rarity.ToString()}]", rarityColor)} {equipItem.EquipmentData.Name}");

                highlightImage.gameObject.SetActive(false);
                itemNameBG.gameObject.SetActive(false);
                amountText.gameObject.SetActive(false);
                selectedBG.gameObject.SetActive(false);

                equippedImage.gameObject.SetActive(false);
            }
            else if (item is CountableItem countableItem)
            {
                iconImage.sprite = countableItem.CountableData.IconSprite;
                var rarityColor = eRarityColor.GetColor(countableItem.CountableData.rarity);

                slotBG.color = rarityColor;

                itemNameText.color = rarityColor;
                itemNameText.SetText($"{TMProUtility.GetColorText($"[{countableItem.CountableData.rarity.ToString()}]", rarityColor)} {countableItem.CountableData.Name}");

                highlightImage.gameObject.SetActive(false);
                itemNameBG.gameObject.SetActive(false);
                selectedBG.gameObject.SetActive(false);
                amountText.gameObject.SetActive(true);

                amountText.SetText($"x{countableItem.Amount}");

                equippedImage.gameObject.SetActive(false);
            }
            
            
        }

        public void UpdateCount(int count)
        {
            amountText.SetText($"x{count}");
        }

        public void ClearSlot()
        {
            iconImage.sprite = null;
            //var rarityColor = eRarityColor.GetColor(equipItem.EquipmentData.rarity);

            slotBG.color = Color.black;

            itemNameText.color = Color.white;
            //itemNameText.SetText(equipItem.EquipmentData.Name);

            highlightImage.gameObject.SetActive(false);
            itemNameBG.gameObject.SetActive(false);
            amountText.gameObject.SetActive(false);
            selectedBG.gameObject.SetActive(false);
            EquippedItem = null;
            ResetClickState();
            SetUnEquipped();
        }
        public void SelectSlot()
        {
            selectedBG.gameObject.SetActive(true);
        }

        public void DeselectSlot() => selectedBG.gameObject.SetActive(false);

        public void SetEquipped()
        {
            equippedImage.gameObject.SetActive(true);
        }

        public void SetUnEquipped()
        {
            equippedImage.gameObject.SetActive(false);
        }
    }
}