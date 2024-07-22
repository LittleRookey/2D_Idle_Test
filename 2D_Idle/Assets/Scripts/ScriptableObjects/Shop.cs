using Litkey.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Utility;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Litkey/Shop/RandomShop")]
public class Shop : ScriptableObject
{
    [Header("Actual product to sell")]
    [SerializeField] protected Inventory _inventory;
    [field: SerializeField, ReadOnly] public List<Product> Products { protected set; get; }
    [Header("Inspector product")]
    public Product[] thingsToSell;
    public WeightedRandomPicker<Product> shopContainer; // 정보저장 
    //public int baseProductCounts = 6;
    public int _productCounts => shopProductCount;

    public int shopProductCount = 6;
    protected Dictionary<int, bool> bag;

    [field: SerializeField] public int currentPriceTotal { private set; get; } = 0;

    [HideInInspector] public UnityEvent OnClearBag;
    [HideInInspector] public UnityEvent<int> OnItemBought;
    [HideInInspector] public UnityEvent<int, int> OnBagUpdated; // index와 total price를 이벤트로 보냄
    [HideInInspector] public UnityEvent<Product> OnItemAdded;


    public virtual void Initialize()
    {
        currentPriceTotal = 0;
        shopContainer = new WeightedRandomPicker<Product>();
        for (int i = 0; i < thingsToSell.Length; i++)
        {
            shopContainer.Add(thingsToSell[i], thingsToSell[i].Weight);
        }

        if (bag == null)
        {
            bag = new Dictionary<int, bool>();
            for (int i = 0; i < shopProductCount; i++)
            {
                bag.Add(i, false);
            }
        }
        if (Products == null)
        {
            Products = new List<Product>();
        }

    }

    [Button("RefreshShop")]
    public virtual void RefreshShop()
    {
        Debug.Log("Shop Refreshed");
        if (bag == null)
        {
            bag = new Dictionary<int, bool>();
            for (int i = 0; i < shopProductCount; i++)
            {
                bag.Add(i, false);
            }
        }
        if (Products == null)
        {
            Products = new List<Product>();
        }
        if (shopContainer == null)
        {
            shopContainer = new WeightedRandomPicker<Product>();
            for (int i = 0; i < thingsToSell.Length; i++)
            {
                shopContainer.Add(thingsToSell[i], thingsToSell[i].Weight);
            }
        }

        Products.Clear();

        for (int i = 0; i < _productCounts; i++)
        {
            var item = shopContainer.GetRandomPick().Clone();
            Debug.Log("Products added");
            Products.Add(item);
            OnItemAdded?.Invoke(item);
        }
    }

    public void AddToBag(int index)
    {
        if (!bag[index])
        {
            bag[index] = true;
            currentPriceTotal += Products[index].TotalPrice;
            OnBagUpdated?.Invoke(index, currentPriceTotal);
        }
    }

    public void RemoveFromBag(int index)
    {
        if (bag[index])
        {
            bag[index] = false;
            currentPriceTotal -= Products[index].TotalPrice;
            currentPriceTotal = Mathf.Clamp(currentPriceTotal, 0, int.MaxValue);
            OnBagUpdated?.Invoke(index, currentPriceTotal);
        }
    }

    protected void ClearBag()
    {
        for (int i = 0; i < _productCounts; i++)
        {
            bag[i] = false;
        }
        OnClearBag?.Invoke();
        currentPriceTotal = 0;
    }

    public virtual bool BuyItems()
    {
        if (bag == null) return false;
        if (currentPriceTotal <= 0) return false;
        if (!ResourceManager.Instance.HasGold(currentPriceTotal)) return false;

        ResourceManager.Instance.UseGold(currentPriceTotal);

        foreach (var entry in bag)
        {
            int index = entry.Key;
            bool isInBag = entry.Value;

            if (isInBag)
            {
                Products[index].Sell();
                var soldItem = Products[index].Item.CreateItem();
                if (soldItem is CountableItem countableItem)
                {
                    countableItem.SetAmount(Products[index].Count);
                    _inventory.AddToInventory(countableItem);
                } 
                else
                {
                    _inventory.AddToInventory(soldItem);
                }

                OnItemBought?.Invoke(index);
            }
        }

        ClearBag();
        return true;
    }

    public void AddToShopContainer()
    {
        for (int i = 0; i < thingsToSell.Length; i++)
        {
            shopContainer.Add(thingsToSell[i], thingsToSell[i].Weight);
        }
    }

    public System.Collections.ObjectModel.ReadOnlyDictionary<Product, double> GetPercentageTable()
    {
        return shopContainer.GetNormalizedItemDictReadonly();
    }
}

[System.Serializable]
public class Product
{
    public ItemData Item;
    public int Count = 1;
    public bool IsSold;
    public int Weight;
    public int TotalPrice => Item.SellPrice * Count;

    public Product() { }

    public Product(Product product)
    {
        Item = product.Item;
        Count = product.Count;
        IsSold = false;
        Weight = product.Weight;
    }

    public void Sell()
    {
        if (IsSold) Debug.LogError($"There is no mroe product {Item.Name} left in Shop");

        IsSold = true;
        
    }

    public Product Clone()
    {
        return new Product(this);
    }
}
