using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.InventorySystem;
using Litkey.Utility;
using Litkey.Interface;


[CreateAssetMenu(fileName = "LootTable", menuName = "Litkey/LootTable")]
public class LootTable : ScriptableObject
{

    [System.Serializable]
    public class ItemDrop
    {
        public ItemData item;
        [Range(0f, 100f)]
        public float dropRate;
        public Vector2Int dropCount = Vector2Int.one;
    }

    public string _lootID;
    public Vector2Int gold;
    public int dropExp;
    [SerializeField] private ItemDrop[] _lootTable;
    
    public ItemDrop[] GetLootTableInfo()
    {
        return _lootTable;
    }

    public override string ToString()
    {
        string s = "\n";
        for (int i = 0; i < _lootTable.Length; i++)
        {
            s += _lootTable[i].item.ToString() + ": " + _lootID + "\n";
        }
        return s;
    }

    public int GetGoldReward()
    {
        return Random.Range(gold.x, gold.y+1);
    }

    public int GetExpReward()
    {
        return dropExp;
    }

    public bool HasDropItem()
    {
        return _lootTable.Length > 0;
    }

    public List<Item> GetDropItems()
    {
        List<Item> itemDrops = new List<Item>();
        for (int i = 0; i < _lootTable.Length; i++)
        {
            if (ProbabilityCheck.GetThisChanceResult_Percentage(_lootTable[i].dropRate))
            {
                int count = Random.Range(_lootTable[i].dropCount.x, _lootTable[i].dropCount.y);
                var itemData = _lootTable[i].item;
                if (itemData is CountableItemData countableItemData)
                {
                    var dropItem = countableItemData.CreateItem() as CountableItem;
                    itemDrops.Add(dropItem);
                    dropItem.SetAmount(count);
                }
                else if (itemData is EquipmentItemData equipItemData)
                {

                    itemDrops.Add(equipItemData.CreateItem());
                }
                
           
            }
        }
        return itemDrops;

    }


}


