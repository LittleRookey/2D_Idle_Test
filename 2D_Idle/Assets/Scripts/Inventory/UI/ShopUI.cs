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
            shop.Initialize();
            for (int i = 0; i < shop._productCounts; i++)
            {
                if (!shopSlots.ContainsKey(i)) shopSlots.Add(i, null);
            }

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

    public void SetShop(Shop shop)
    {
        if (this.shop != null)
        {
            shop.OnItemBought.RemoveListener(SetSold);
            shop.OnBagUpdated.RemoveListener(UpdateTotalPrice);
            shop.OnItemAdded.RemoveListener(AddShopSlotUI);
            shop.OnClearBag.RemoveListener(UpdateTotalPriceToZero);
            ClearAllSlots();
        }
        this.shop = shop;
        shop.OnItemBought.AddListener(SetSold);
        shop.OnBagUpdated.AddListener(UpdateTotalPrice);
        shop.OnItemAdded.AddListener(AddShopSlotUI);
        shop.OnClearBag.AddListener(UpdateTotalPriceToZero);
        shop.Initialize();
        shop.RefreshShop();
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
        totalPriceText.SetText($"รั ฑÝพื: {totalPrice.ToString("N0")}G");
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
            shopSlot.SetShopSlotUI(product.Item, product.Count, false, () => UpdateSlotState(index));
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
        for (int i = 0; i < shop._productCounts; i++)
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
        foreach (int slotIndex in shopSlots.Keys)
        {
            shopSlots[slotIndex].ClearSlot();
            shopSlotPool.Take(shopSlots[slotIndex]);
            shopSlots[slotIndex] = null;

        }
    }


}
