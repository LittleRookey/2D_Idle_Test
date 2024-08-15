using Litkey.InventorySystem;
using Litkey.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

[InlineEditor]
[CreateAssetMenu(menuName = "Litkey/Loots/RankedLootTable")]
public class RankedLootTable : LootTable
{

    
    [SerializeField] protected RankDrop[] _rankedLootTable;
    [System.Serializable]
    public class RankDrop
    {
        public ItemRarity rankName;
        [Range(0f, 100f)]
        public float rankDropRate;
        public ItemDrop[] items;

        public void NormalizeItemDropRates()
        {
            float total = items.Sum(item => item.dropRate);
            if (total != 100f)
            {
                float multiplier = 100f / total;
                foreach (var item in items)
                {
                    item.dropRate *= multiplier;
                }
            }
        }

    }
    private void OnValidate()
    {
        foreach (var rank in _rankedLootTable)
        {
            rank.NormalizeItemDropRates();
        }
    }

    public Item GetSingleItem()
    {
        // First, select a rank based on rank drop rates
        float totalRankRate = _rankedLootTable.Sum(rank => rank.rankDropRate);
        float randomRankValue = UnityEngine.Random.Range(0f, totalRankRate);
        float currentRankSum = 0f;
        RankDrop selectedRank = null;

        foreach (var rank in _rankedLootTable)
        {
            currentRankSum += rank.rankDropRate;
            if (randomRankValue <= currentRankSum)
            {
                selectedRank = rank;
                break;
            }
        }

        if (selectedRank == null || selectedRank.items.Length == 0)
        {
            return null;
        }

        // Now select an item from the chosen rank
        float randomItemValue = UnityEngine.Random.Range(0f, 100f);
        float currentItemSum = 0f;
        ItemDrop selectedItemDrop = null;

        foreach (var item in selectedRank.items)
        {
            currentItemSum += item.dropRate;
            if (randomItemValue <= currentItemSum)
            {
                selectedItemDrop = item;
                break;
            }
        }

        if (selectedItemDrop == null)
        {
            return null;
        }

        // Create and return the selected item
        int count = UnityEngine.Random.Range(selectedItemDrop.dropCount.x, selectedItemDrop.dropCount.y + 1);
        if (selectedItemDrop.item is CountableItemData countableItemData)
        {
            var dropItem = countableItemData.CreateItem() as CountableItem;
            dropItem.SetAmount(count);
            return dropItem;
        }
        else if (selectedItemDrop.item is EquipmentItemData equipItemData)
        {
            return equipItemData.CreateItem();
        }

        return null;
    }

    public override List<Item> GetDropItems()
    {
        List<Item> itemDrops = new List<Item>();
        foreach (var rank in _rankedLootTable)
        {
            if (ProbabilityCheck.GetThisChanceResult_Percentage(rank.rankDropRate))
            {
                foreach (var itemDrop in rank.items)
                {
                    if (ProbabilityCheck.GetThisChanceResult_Percentage(itemDrop.dropRate))
                    {
                        int count = UnityEngine.Random.Range(itemDrop.dropCount.x, itemDrop.dropCount.y + 1);
                        var itemData = itemDrop.item;
                        if (itemData is CountableItemData countableItemData)
                        {
                            var dropItem = countableItemData.CreateItem() as CountableItem;
                            dropItem.SetAmount(count);
                            Debug.Log($"Reward added: Countable Item Data: {countableItemData.Name} x{count}");
                            itemDrops.Add(dropItem);
                        }
                        else if (itemData is EquipmentItemData equipItemData)
                        {
                            Debug.Log($"Reward added: Equipment Item Data: {equipItemData.Name} x{count}");
                            itemDrops.Add(equipItemData.CreateItem());
                        }
                    }
                }
            }
        }
        return itemDrops;
    }
}
