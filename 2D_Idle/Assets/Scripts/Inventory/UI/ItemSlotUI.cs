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
        [SerializeField] private TextMeshProUGUI upgradeText;
        [SerializeField] private Button slotButton;

        [SerializeField] private Image equippedImage;
        [SerializeField] private Image highlightImage;
        [SerializeField] private Image itemNameBG;

        [SerializeField] private float highlightAlpha;
        [SerializeField] private EquipmentRarityColor eRarityColor;
        [SerializeField] private Image glowImage;

        private bool isDirty;
        public bool IsDirty => isDirty;

        public bool HasItem => iconImage != null;

        private Color colorVariant = new Color(30f,30f,30f);
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
        
        public void SetSlot(ItemData itemData, int itemCount)
        {
            var item = itemData.CreateItem();

            EquippedItem = item;
            if (item is EquipmentItem equipItem)
            {
                upgradeText.gameObject.SetActive(true);
                
                iconImage.sprite = equipItem.EquipmentData.IconSprite;
                var rarityColor = eRarityColor.GetColor(equipItem.EquipmentData.rarity);

                slotBG.color = rarityColor;

                itemNameText.color = rarityColor;
                glowImage.color = rarityColor + colorVariant;

                itemNameText.SetText($"{TMProUtility.GetColorText($"[{equipItem.EquipmentData.rarity.ToString()}]", rarityColor)} {equipItem.EquipmentData.Name}");
                amountText.SetText($"x{itemCount}");
                if (equipItem.CurrentUpgrade <= 0)
                {
                    upgradeText.gameObject.SetActive(false);
                }
                upgradeText.SetText($"+{equipItem.CurrentUpgrade}");

                amountText.gameObject.SetActive(true);
                highlightImage.gameObject.SetActive(false);
                itemNameBG.gameObject.SetActive(false);
                selectedBG.gameObject.SetActive(false);
                glowImage.gameObject.SetActive(true);

                equippedImage.gameObject.SetActive(false);
            }
            else if (item is CountableItem countableItem)
            {
                countableItem.SetAmount(itemCount);
                iconImage.sprite = countableItem.CountableData.IconSprite;
                var rarityColor = eRarityColor.GetColor(countableItem.CountableData.rarity);

                slotBG.color = rarityColor;

                itemNameText.color = rarityColor;
                itemNameText.SetText($"{TMProUtility.GetColorText($"[{countableItem.CountableData.rarity.ToString()}]", rarityColor)} {countableItem.CountableData.Name}");
                glowImage.color = rarityColor + colorVariant;

                upgradeText.gameObject.SetActive(false);
                highlightImage.gameObject.SetActive(false);
                itemNameBG.gameObject.SetActive(false);
                selectedBG.gameObject.SetActive(false);
                amountText.gameObject.SetActive(true);
                glowImage.gameObject.SetActive(true);

                amountText.SetText($"x{countableItem.Amount}");

                equippedImage.gameObject.SetActive(false);

            }
        }

        public void SetSlot(ItemData itemData, int itemCount, bool displayXInCount)
        {
            var item = itemData.CreateItem();

            EquippedItem = item;
            if (item is EquipmentItem equipItem)
            {
                upgradeText.gameObject.SetActive(true);

                iconImage.sprite = equipItem.EquipmentData.IconSprite;
                var rarityColor = eRarityColor.GetColor(equipItem.EquipmentData.rarity);

                slotBG.color = rarityColor;

                itemNameText.color = rarityColor;
                glowImage.color = rarityColor + colorVariant;

                itemNameText.SetText($"{TMProUtility.GetColorText($"[{equipItem.EquipmentData.rarity.ToString()}]", rarityColor)} {equipItem.EquipmentData.Name}");
                if (equipItem.CurrentUpgrade <= 0)
                {
                    upgradeText.gameObject.SetActive(false);
                }
                upgradeText.SetText($"+{equipItem.CurrentUpgrade}");

                highlightImage.gameObject.SetActive(false);
                itemNameBG.gameObject.SetActive(false);
                amountText.gameObject.SetActive(false);
                selectedBG.gameObject.SetActive(false);
                glowImage.gameObject.SetActive(true);

                equippedImage.gameObject.SetActive(false);
            }
            else if (item is CountableItem countableItem)
            {
                countableItem.SetAmount(itemCount);
                iconImage.sprite = countableItem.CountableData.IconSprite;
                var rarityColor = eRarityColor.GetColor(countableItem.CountableData.rarity);

                slotBG.color = rarityColor;

                itemNameText.color = rarityColor;
                itemNameText.SetText($"{TMProUtility.GetColorText($"[{countableItem.CountableData.rarity.ToString()}]", rarityColor)} {countableItem.CountableData.Name}");
                glowImage.color = rarityColor + colorVariant;

                highlightImage.gameObject.SetActive(false);
                itemNameBG.gameObject.SetActive(false);
                selectedBG.gameObject.SetActive(false);
                amountText.gameObject.SetActive(true);
                glowImage.gameObject.SetActive(true);
                upgradeText.gameObject.SetActive(false);

                amountText.SetText($"x{countableItem.Amount}");

                equippedImage.gameObject.SetActive(false);

            }
        }


        public void SetSlot(Item item)
        {
            EquippedItem = item;
            if (item is EquipmentItem equipItem)
            {
                upgradeText.gameObject.SetActive(true);
                iconImage.sprite = equipItem.EquipmentData.IconSprite;
                var rarityColor = eRarityColor.GetColor(equipItem.EquipmentData.rarity);
                
                slotBG.color = rarityColor;

                itemNameText.color = rarityColor;
                glowImage.color = rarityColor + colorVariant;

                itemNameText.SetText($"{TMProUtility.GetColorText($"[{equipItem.EquipmentData.rarity.ToString()}]", rarityColor)} {equipItem.EquipmentData.Name}");
                if (equipItem.CurrentUpgrade <= 0)
                {
                    upgradeText.gameObject.SetActive(false);
                }
                upgradeText.SetText($"+{equipItem.CurrentUpgrade}");

                highlightImage.gameObject.SetActive(false);
                itemNameBG.gameObject.SetActive(false);
                amountText.gameObject.SetActive(false);
                selectedBG.gameObject.SetActive(false);
                glowImage.gameObject.SetActive(true);

                equippedImage.gameObject.SetActive(false);
            }
            else if (item is CountableItem countableItem)
            {
                iconImage.sprite = countableItem.CountableData.IconSprite;
                var rarityColor = eRarityColor.GetColor(countableItem.CountableData.rarity);

                slotBG.color = rarityColor;

                itemNameText.color = rarityColor;
                itemNameText.SetText($"{TMProUtility.GetColorText($"[{countableItem.CountableData.rarity.ToString()}]", rarityColor)} {countableItem.CountableData.Name}");
                glowImage.color = rarityColor + colorVariant;

                upgradeText.gameObject.SetActive(false);
                highlightImage.gameObject.SetActive(false);
                itemNameBG.gameObject.SetActive(false);
                selectedBG.gameObject.SetActive(false);
                amountText.gameObject.SetActive(true);
                glowImage.gameObject.SetActive(true);

                amountText.SetText($"x{countableItem.Amount}");

                equippedImage.gameObject.SetActive(false);

            }
            
            
        }

        public void UpdateCount(int count)
        {
            amountText.SetText($"x{count}");
        }

        public void UpdateUpgrade(int currentUpgrade)
        {
            upgradeText.SetText($"+{currentUpgrade}");
            upgradeText.gameObject.SetActive(true);
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
            highlightImage.gameObject.SetActive(true);
        }

        public void DeselectSlot() => highlightImage.gameObject.SetActive(false);

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