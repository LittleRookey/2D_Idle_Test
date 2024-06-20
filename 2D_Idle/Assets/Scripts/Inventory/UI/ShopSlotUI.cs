using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Litkey.InventorySystem;
using Litkey.Utility;
using UnityEngine.Events;

public class ShopSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private TextMeshProUGUI priceText;

    [SerializeField] private Image titleBG;
    [SerializeField] private Image iconImage;
    [SerializeField] private RectTransform selectedGO;
    [SerializeField] private RectTransform soldGO;
    [SerializeField] private Button slotButton;
    [SerializeField] private DOTweenAnimation onSelectedAnim;
    [SerializeField] private EquipmentRarityColor rarityColor;
    
    List<Tween> slotTweens;

    private bool _isSold;
    public bool IsSold => _isSold;
    public bool IsSelected => selectedGO.gameObject.activeInHierarchy;

    private readonly string _onOpen = "OnOpen";
    private readonly string _onSelected = "OnSelected";

    public void SetShopSlotUI(ItemData item, int itemCount, bool isSold=false, UnityAction OnSlotSelected=null)
    {
        _isSold = isSold;
        
        titleText.SetText($"[{item.rarity}] {item.Name}");
        countText.SetText($"x{itemCount}");
        priceText.SetText($"{item.SellPrice * itemCount}G");

        titleBG.color = rarityColor.GetColor(item.rarity);
        iconImage.sprite = item.IconSprite;

        slotButton.enabled = true;

        selectedGO.gameObject.SetActive(false);
        if (_isSold)
            soldGO.gameObject.SetActive(true);
        slotButton.onClick.AddListener(OnSlotSelected);
    }


    private void RunAnim(string inputString)
    {
        if (slotTweens == null)
        {
            slotTweens = onSelectedAnim.GetTweens();
        }
        var tween = slotTweens.Find((Tween tween) => tween.stringId.Equals(inputString));
        if (tween != null)
        {
            tween.Restart();
        }
    }

    public void SelectSlot()
    {
        RunAnim(_onSelected);
        selectedGO.gameObject.SetActive(true);
    }

    public void DeselectSlot()
    {
        selectedGO.gameObject.SetActive(false);
    }

    public void ClearSlot()
    {
        _isSold = false;

        //titleBG.color = rarityColor.GetColor(item.rarity);
        iconImage.sprite = null;

        selectedGO.gameObject.SetActive(false);
        
        soldGO.gameObject.SetActive(false);
        slotButton.onClick.RemoveAllListeners();
        slotButton.enabled = true;
    }

    public void OnSlotDisplayed()
    {
        RunAnim(_onOpen);
    }

    public void SetSold()
    {
        selectedGO.gameObject.SetActive(false);
        soldGO.gameObject.SetActive(true);
        slotButton.enabled = false;

    }


}
