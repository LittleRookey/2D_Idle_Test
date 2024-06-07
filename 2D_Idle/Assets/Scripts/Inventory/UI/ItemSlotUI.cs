using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace Litkey.InventorySystem
{
    public class ItemSlotUI : MonoBehaviour
    {
        [SerializeField] private Image slotBG;
        [SerializeField] private Image iconImage;
        [SerializeField] private Image useBG;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private TextMeshProUGUI useText;

        [SerializeField] private Image equippedImage;
        [SerializeField] private Image highlightImage;
        [SerializeField] private Image itemNameBG;

        [SerializeField] private float highlightAlpha;
        [SerializeField] private EquipmentRarityColor eRarityColor;

        public UnityEvent OnSlotClick;


        private void SlotClicked()
        {
            useBG.gameObject.SetActive(true);
            highlightImage.gameObject.SetActive(true);
            itemNameBG.gameObject.SetActive(true);
        }
        public void SetSlot(Item item, UnityAction clickAction=null)
        {
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
                useBG.gameObject.SetActive(false);

                useText.SetText($"ÀåÂø");

                equippedImage.gameObject.SetActive(false);
                
            }
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
            useBG.gameObject.SetActive(false);

            equippedImage.gameObject.SetActive(false);
        }
    }
}