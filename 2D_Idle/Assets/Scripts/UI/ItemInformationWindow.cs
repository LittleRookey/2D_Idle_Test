using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using Redcode.Pools;
using Litkey.InventorySystem;
using UnityEngine.Events;

public class ItemInformationWindow : MonoBehaviour
{
    public static ItemInformationWindow Instance;
    [SerializeField] private RectTransform orientation; // 아이템창과 주위배경
    [SerializeField] private DraggableUI onlyWindow; // 아이템창만

    [SerializeField] private Image rankTopImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image iconBG;
    [SerializeField] private Image[] Stars;

    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI itemRankText;
    [Header("옵션들")]
    [SerializeField] private ItemInfoBasicOption itemBasicOptionPrefab;
    [SerializeField] private RectTransform itemBasicOptionParent;
    [SerializeField] private TextMeshProUGUI itemExplanationText;
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;
    [SerializeField] private Button button4;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button exitBackground;

    [Header("References")]
    [SerializeField] private EquipmentRarityColor rarityColor;
    [SerializeField] private Inventory inventory;
    [SerializeField] private InventoryUI inventoryUI;

    private Pool<ItemInfoBasicOption> itemInfoPool;

    private List<ItemInfoBasicOption> itemInfoStored;
    public bool DisableOnStart;

    
    UnityEvent OnClosedEvent = new();
    private void Awake()
    {
        Instance = this;
        itemInfoStored = new List<ItemInfoBasicOption>();
        itemInfoPool = Pool.Create<ItemInfoBasicOption>(itemBasicOptionPrefab);
        itemInfoPool.SetContainer(itemBasicOptionParent);
        ClearItemAndCloseInformationWindow();
    }

    public void ShowItemInfo(Item item, bool enableButton, UnityAction OnClosed = null)
    {
        if (item is ResourceGetterItem resourceGetterItem) ShowEquipmentItemInfo(resourceGetterItem, enableButton, OnClosed);
        else if (item is EquipmentItem equipmentItem) ShowEquipmentItemInfo(equipmentItem, enableButton, OnClosed);
        else if (item is IUsableItem && item is CountableItem countableItem) ShowUsableItemInfo(countableItem, enableButton, OnClosed);
        else if (item is CountableItem countableItems) ShowCountableItemInfo(countableItems, OnClosed);
        orientation.gameObject.SetActive(true);
    }

    /// <summary>
    /// 아이템이 인벤토리에 없는곳에서 부름
    /// </summary>
    /// <param name="itemData"></param>
    /// <param name="enableButton"></param>
    /// <param name="OnClosed"></param>
    public void ShowItemInfo(ItemData itemData, UnityAction OnClosed = null)
    {
        if (itemData is ResourceGetterItemData resourceGetterItemData) ShowEquipmentItemInfo(resourceGetterItemData, OnClosed);
        else if (itemData is EquipmentItemData equipmentItemData) ShowEquipmentItemInfo(equipmentItemData, OnClosed);
        else if (itemData is CountableItemData countableItems) ShowCountableItemInfo(countableItems, OnClosed);
        orientation.gameObject.SetActive(true);
    }

    private void DisableAllStarts()
    {
        for (int i = 0; i < Stars.Length; i++)
        {
            Stars[i].gameObject.SetActive(false);
        }
    }

    public void ClearItemAndCloseInformationWindow()
    {
        // 초기화 하기
        for (int i = 0; i < itemInfoStored.Count; i++)
        {
            itemInfoPool.Take(itemInfoStored[i]);
        }
        itemInfoStored.Clear();
        // 버튼 초기화
        ResetButtons();
        OnClosedEvent?.Invoke();

        // 버튼 초기화
        ResetButtons();
        OnClosedEvent.RemoveAllListeners();
        onlyWindow.ResetPosition();
        exitBackground.gameObject.SetActive(false);
        orientation.gameObject.SetActive(false);
    }

    private void EnableStars(int enableStarCount)
    {
        DisableAllStarts();
        for (int i = 0; i < enableStarCount; i++)
        {
            Stars[i].gameObject.SetActive(true);
        }
    }

    private void ResetButtons()
    {
        button1.onClick.RemoveAllListeners();
        button2.onClick.RemoveAllListeners();
        button3.onClick.RemoveAllListeners();
        button4.onClick.RemoveAllListeners();

        button1.gameObject.SetActive(false);
        button2.gameObject.SetActive(false);
        button3.gameObject.SetActive(false);
        button4.gameObject.SetActive(false);
    }

    private void ShowEquipmentItemInfo(EquipmentItemData equipmentData, UnityAction OnClosed=null)
    {

        OnClosedEvent.AddListener(OnClosed);
        exitBackground.gameObject.SetActive(true);

        iconImage.sprite = equipmentData.IconSprite;
        iconBG.color = rarityColor.GetColor(equipmentData.rarity);
        itemNameText.SetText(equipmentData.Name); // TODO 후에 강화수치까지 표시하기

        itemTypeText.SetText($"장비 / {equipmentData.Parts.ToString()}");

        itemRankText.SetText(equipmentData.rarity.ToString());

        rankTopImage.color = rarityColor.GetColor(equipmentData.rarity);

        // 별 개수
        EnableStars(1);

        // 아이템 옵션보이기
        var itemInfo = itemInfoPool.Get();
        itemInfo.SetOption(eItemOptionType.기본, equipmentData.GetStats());
        itemInfoStored.Add(itemInfo);

        itemInfo.transform.SetAsFirstSibling();

        itemExplanationText.SetText(equipmentData.Tooltip);
        itemExplanationText.transform.SetAsLastSibling();
    }

    private void ShowEquipmentItemInfo(EquipmentItem equipment, bool enableButton, UnityAction OnClosed = null)
    {

        // 재 설정
        var equipmentData = equipment.EquipmentData;

        OnClosedEvent.AddListener(OnClosed);
        exitBackground.gameObject.SetActive(true);

        iconImage.sprite = equipmentData.IconSprite;
        iconBG.color = rarityColor.GetColor(equipmentData.rarity);
        itemNameText.SetText(equipmentData.Name); // TODO 후에 강화수치까지 표시하기

        itemTypeText.SetText($"장비 / {equipmentData.Parts.ToString()}");

        itemRankText.SetText(equipmentData.rarity.ToString());

        rankTopImage.color = rarityColor.GetColor(equipmentData.rarity);

        // 별 개수
        EnableStars(1);

        // 아이템 옵션보이기
        var itemInfo = itemInfoPool.Get();
        itemInfo.SetOption(eItemOptionType.기본, equipmentData.GetStats());
        itemInfoStored.Add(itemInfo);

        itemInfo.transform.SetAsFirstSibling();

        itemExplanationText.SetText(equipmentData.Tooltip);
        itemExplanationText.transform.SetAsLastSibling();

        // 버튼들 설정
        if (!enableButton) return;

        button1.gameObject.SetActive(true);
        int itemIndex = inventory.FindItemInInventory(equipment.ID);

        button1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("판매");
        button1.onClick.AddListener(() =>
        {
            inventory.SellItem(itemIndex, 1);
            ClearItemAndCloseInformationWindow();
        });

        button3.gameObject.SetActive(true);

        button3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("강화");
        button3.onClick.AddListener(() =>
        {
            // 강화창 열기
            inventoryUI.OpenItemUpgradeWindow(equipment);

            ClearItemAndCloseInformationWindow();
        });

        button4.gameObject.SetActive(true);
        if (inventory.IsEquipped(equipment))
        {
            button4.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("해제");
        }
        else
        {
            button4.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("장착");
        }
        button4.onClick.AddListener(() =>
        {
            inventoryUI.UseOrEquipItem(itemIndex);
            ClearItemAndCloseInformationWindow();
        });
    }

    private void ShowEquipmentItemInfo(ResourceGetterItemData equipmentData, UnityAction OnClosed = null)
    {

        // 재 설정

        OnClosedEvent.AddListener(OnClosed);
        exitBackground.gameObject.SetActive(true);

        iconImage.sprite = equipmentData.IconSprite;
        iconBG.color = rarityColor.GetColor(equipmentData.rarity);

        itemNameText.SetText(equipmentData.Name); // TODO 후에 강화수치까지 표시하기

        itemTypeText.SetText($"장비 / {equipmentData.Parts.ToString()}");

        itemRankText.SetText(equipmentData.rarity.ToString());

        rankTopImage.color = rarityColor.GetColor(equipmentData.rarity);

        // 별 개수
        EnableStars(1);

        // 아이템 옵션보이기
        var itemInfo = itemInfoPool.Get();
        itemInfo.SetOption(eItemOptionType.기본, equipmentData.MaxDurability);
        itemInfoStored.Add(itemInfo);

        itemInfo.transform.SetAsFirstSibling();

        itemExplanationText.SetText(equipmentData.Tooltip);
        itemExplanationText.transform.SetAsLastSibling();

    }

    private void ShowEquipmentItemInfo(ResourceGetterItem equipment, bool enableButton, UnityAction OnClosed = null)
    {

        // 재 설정
        var equipmentData = equipment.EquipmentData;

        OnClosedEvent.AddListener(OnClosed);
        exitBackground.gameObject.SetActive(true);

        iconImage.sprite = equipmentData.IconSprite;
        iconBG.color = rarityColor.GetColor(equipmentData.rarity);

        itemNameText.SetText(equipmentData.Name); // TODO 후에 강화수치까지 표시하기

        itemTypeText.SetText($"장비 / {equipmentData.Parts.ToString()}");

        itemRankText.SetText(equipmentData.rarity.ToString());

        rankTopImage.color = rarityColor.GetColor(equipmentData.rarity);

        // 별 개수
        EnableStars(1);

        // 아이템 옵션보이기
        var itemInfo = itemInfoPool.Get();
        itemInfo.SetOption(eItemOptionType.기본, equipmentData.MaxDurability);
        itemInfoStored.Add(itemInfo);

        itemInfo.transform.SetAsFirstSibling();

        itemExplanationText.SetText(equipmentData.Tooltip);
        itemExplanationText.transform.SetAsLastSibling();

        // 버튼들 설정
        if (!enableButton) return;

        
        button1.gameObject.SetActive(true);
        int itemIndex = inventory.FindItemInInventory(equipment.ID);

        button1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("판매");
        button1.onClick.AddListener(() =>
        {
            inventory.SellItem(itemIndex, 1);
            ClearItemAndCloseInformationWindow();
        });

        button4.gameObject.SetActive(true);
        if (inventory.IsEquipped(equipment))
        {
            button4.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("해제");
        }
        else
        {
            button4.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("장착");
        }
        button4.onClick.AddListener(() =>
        {
            //inventory.EquipItem(equipment);
            inventoryUI.UseOrEquipItem(itemIndex);
            ClearItemAndCloseInformationWindow();
        });
    }

    private void ShowCountableItemInfo(CountableItemData countableItemData, UnityAction OnClosed = null)
    {
        // 재 설정

        OnClosedEvent.AddListener(OnClosed);
        exitBackground.gameObject.SetActive(true);

        iconImage.sprite = countableItemData.IconSprite;
        iconBG.color = rarityColor.GetColor(countableItemData.rarity);
        itemNameText.SetText(countableItemData.Name); // TODO 후에 강화수치까지 표시하기

        itemTypeText.SetText($"재료");

        itemRankText.SetText(countableItemData.rarity.ToString());

        rankTopImage.color = rarityColor.GetColor(countableItemData.rarity);

        // 별 개수
        EnableStars(0);

        itemExplanationText.SetText(countableItemData.Tooltip);
        itemExplanationText.transform.SetAsLastSibling();
    }

    private void ShowCountableItemInfo(CountableItem countableItem, UnityAction OnClosed = null)
    {
        // 재 설정
        var countableItemData = countableItem.CountableData;

        OnClosedEvent.AddListener(OnClosed);
        exitBackground.gameObject.SetActive(true);

        iconImage.sprite = countableItemData.IconSprite;
        iconBG.color = rarityColor.GetColor(countableItemData.rarity);
        itemNameText.SetText(countableItemData.Name); // TODO 후에 강화수치까지 표시하기

        itemTypeText.SetText($"재료");

        itemRankText.SetText(countableItemData.rarity.ToString());

        rankTopImage.color = rarityColor.GetColor(countableItemData.rarity);

        // 별 개수
        EnableStars(0);

        itemExplanationText.SetText(countableItemData.Tooltip);
        itemExplanationText.transform.SetAsLastSibling();

    }

    private void ShowUsableItemInfo(CountableItem countableItem, bool enableButton, UnityAction OnClosed = null)
    {
        // 초기화 하기

        // 재 설정
        var countableItemData = countableItem.CountableData;

        OnClosedEvent.AddListener(OnClosed);
        exitBackground.gameObject.SetActive(true);

        iconImage.sprite = countableItemData.IconSprite;
        iconBG.color = rarityColor.GetColor(countableItemData.rarity);
        itemNameText.SetText(countableItemData.Name); // TODO 후에 강화수치까지 표시하기

        itemTypeText.SetText($"재료");

        itemRankText.SetText(countableItemData.rarity.ToString());

        rankTopImage.color = rarityColor.GetColor(countableItemData.rarity);

        // 별 개수
        EnableStars(0);

        //var itemInfo = itemInfoPool.Get();
        //itemInfo.SetOption(eItemOptionType.기본, countableItem.CountableData.MaxDurability);
        //itemInfoStored.Add(itemInfo);


        //itemInfo.transform.SetAsFirstSibling();

        itemExplanationText.SetText(countableItemData.Tooltip);
        itemExplanationText.transform.SetAsLastSibling();


        if (!enableButton) return;

        button1.gameObject.SetActive(true);
        int itemIndex = inventory.FindItemInInventory(countableItem.ID);

        button1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("판매");
        button1.onClick.AddListener(() =>
        {
            inventory.SellItem(itemIndex, countableItem.Amount);
            ClearItemAndCloseInformationWindow();
        });

        button4.gameObject.SetActive(true);

        button4.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("사용");

        button4.onClick.AddListener(() =>
        {
            inventoryUI.UseOrEquipItem(inventory.FindItemInInventory(countableItem.ID));
        });

    }

}
