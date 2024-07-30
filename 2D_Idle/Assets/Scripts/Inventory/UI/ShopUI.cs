using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Redcode.Pools;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;
    [InlineEditor]
    [SerializeField] private Shop shop;

    [SerializeField] private RectTransform shopWindow;
    [SerializeField] private RectTransform shopSlotParent;
    [SerializeField] private ShopSlotUI shopSlotPrefab;
    [SerializeField] private TextMeshProUGUI totalPriceText;

    Pool<ShopSlotUI> shopSlotPool;
    //[SerializeField] private RectTransform 

    private Dictionary<int, ShopSlotUI> shopSlots;

    public UnityEvent OnShopWindowOpened;
    private void Awake()
    {
        Instance = this;

        shopSlotPool = Pool.Create<ShopSlotUI>(shopSlotPrefab, 6, shopSlotParent);
        shopSlots = new Dictionary<int, ShopSlotUI>();
        if (shop != null)
        {
            shop = null;

        }
    }

    private void OnEnable()
    {
        if (shop != null)
        {
            shop.OnItemBought.AddListener(SetSold);
            shop.OnBagUpdated.AddListener(UpdateTotalPrice);
            shop.OnItemAdded.AddListener(AddShopSlotUI);
            shop.OnClearBag.AddListener(UpdateTotalPriceToZero);
        }
    }

    private void OnDisable()
    {
        if (shop != null)
        {
            shop.OnItemBought.RemoveListener(SetSold);
            shop.OnBagUpdated.RemoveListener(UpdateTotalPrice);
            shop.OnItemAdded.RemoveListener(AddShopSlotUI);
            shop.OnClearBag.RemoveListener(UpdateTotalPriceToZero);

        }
    }

    private void Start()
    {
        //shop.RefreshShop();
    }

    public void SetShop(Shop newShop)
    {
        //오픈한게 같은 상점이면 아무것도안하기
        if (this.shop != null && this.shop.shopID.Equals(newShop.shopID)) return;

        if (this.shop != null)
        {
            ClearShop();
        }

        // this.shop becomes null
        // 만약 상점에 제품이 이미 있으면 그냥 업데이트
        this.shop = newShop;
        this.shop.OnItemBought.AddListener(SetSold);
        this.shop.OnBagUpdated.AddListener(UpdateTotalPrice);
        this.shop.OnItemAdded.AddListener(AddShopSlotUI);
        this.shop.OnClearBag.AddListener(UpdateTotalPriceToZero);
        if (newShop.HasProducts())
        {
            Debug.Log("Updated Shop");
            UpdateShop(this.shop);
        }
        // 상점에 제품이 없으면 새 아이템 Refresh하기
        else
        {
            this.shop.Initialize();
        }
        
        
    }

    private void UpdateShop(Shop shop)
    {
        // 샵 슬롯들이 비어잇어야함
        foreach (var product in shop.Products)
        {
            AddShopSlotUI(product);
        }
    }
    public void OpenShopWindow()
    {
        shopWindow.gameObject.SetActive(true);
        OnShopWindowOpened?.Invoke();
    }

    public void CloseShopWindow()
    {
        DeselectAllSlot();
        shopWindow.gameObject.SetActive(false);
    }

    private void UpdateTotalPriceToZero() => UpdateTotalPrice(-1, 0);

    private void UpdateTotalPrice(int index, int totalPrice)
    {
        Debug.Log($"Updated Total price {totalPrice}");
        totalPriceText.SetText($"총 금액: {totalPrice.ToString("N0")}G");
    }

    private void UpdateSlotState(int index)
    {
        var shopSlot = shopSlots[index];
        if (shopSlot.IsSelected)
        {
            shopSlot.DeselectSlot();
            shop.RemoveFromBag(index);
        } else
        {
            shopSlot.SelectSlot();
            shop.AddToBag(index);
        }
    }
    
    public void AddShopSlotUI(Product product)
    {
        int index = GetEmptyIndex();
        if (index != -1)
        {
            var shopSlot = shopSlotPool.Get();
            shopSlot.SetShopSlotUI(product.Item, product.Count, product.IsSold, () => UpdateSlotState(index));
            shopSlots[index] = shopSlot;
        }
        else
        {
            Debug.LogError($"Shop Slot UI is full at index: {index}");
        }
    }

    public void BuyAll() => shop.BuyItems();

    private int GetEmptyIndex()
    {
        for (int i = 0; i < 20; i++)
        {
            if (!shopSlots.ContainsKey(i)) shopSlots.Add(i, null);
            if (shopSlots[i] == null) return i;
        }

        return -1;
    }

    private void DeselectAllSlot()
    {
        foreach (var index in shopSlots.Keys)
        {
            var shopSlot = shopSlots[index];
            if (shopSlot != null && !shopSlot.IsSold)
            {
                shopSlot.DeselectSlot();
                shop.RemoveFromBag(index);
            }
        }
        
    }

    private void SetSold(int index)
    {
        if (shop is StaticShop staticShop)
        {
            if (!staticShop.isUnLimitedProduct)
            {
                shopSlots[index].SetSold();
            }
        }
        else
        {
            shopSlots[index].SetSold();
        }
        
        Debug.Log($"Slot {index} is Sold!");
    }

    private void ClearAllSlots()
    {
        if (shopSlots == null)
        {
            Debug.Log("shopSlots is null");
            return;
        }

        Debug.Log($"Number of slots: {shopSlots.Count}");

        // Create a list of keys to avoid modifying the dictionary while enumerating
        List<int> slotKeys = new List<int>(shopSlots.Keys);

        foreach (int slotIndex in slotKeys)
        {
            if (shopSlots[slotIndex] != null)
            {
                Debug.Log($"Clearing slot {slotIndex}");
                shopSlots[slotIndex].ClearSlot();
                shopSlotPool.Take(shopSlots[slotIndex]);
                shopSlots[slotIndex] = null;
            }
            else
            {
                Debug.Log($"Slot {slotIndex} is already null");
            }
        }

        Debug.Log("Finished clearing all slots");
    }

    private void ClearShop()
    {
        if (this.shop == null) return;

        this.shop.OnItemBought.RemoveListener(SetSold);
        this.shop.OnBagUpdated.RemoveListener(UpdateTotalPrice);
        this.shop.OnItemAdded.RemoveListener(AddShopSlotUI);
        this.shop.OnClearBag.RemoveListener(UpdateTotalPriceToZero);

        // 모든 슬롯 초기화
        ClearAllSlots();

        this.shop = null;

    }
}
