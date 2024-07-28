using Litkey.InventorySystem;
using Litkey.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Litkey/Shop/StaticShop")]
public class StaticShop : Shop
{
    [SerializeField] public bool isUnLimitedProduct; // 계속 살수있는 Product
    public override void Initialize()
    {
        base.Initialize();
    }

    public override void RefreshShop()
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

        ClearBag();
        Products.Clear();

        for (int i = 0; i < thingsToSell.Length; i++)
        {
            var item = thingsToSell[i].Clone();
            Debug.Log("Products added");
            Products.Add(item);
            OnItemAdded?.Invoke(item);
        }
    }
    public override bool BuyItems()
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
                if (!isUnLimitedProduct) Products[index].Sell();

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
}
