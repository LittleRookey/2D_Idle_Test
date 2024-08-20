using Litkey.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageProgress
{
    public int previousLevel;
    public float previousExp;

    public int previousGold;

    public List<RewardItem> gainedItems;

    public StageProgress()
    {
        gainedItems = new List<RewardItem>();
    }

    public void AddItem(ItemData itemData, int count)
    {
        gainedItems.Add(new RewardItem() { itemData = itemData, count = count });
    }

    public void ClearStageProgress()
    {
        previousLevel = 0;
        previousExp = 0f;
        previousGold = 0;

        gainedItems.Clear();
    }
}
