using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.InventorySystem;
using Litkey.Utility;


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
        return Random.Range(gold.x, gold.y);
    }

    public int GetExpReward()
    {
        return dropExp;
    }

    public bool HasDropItem()
    {
        return _lootTable.Length > 0;
    }

    public List<CountableItem> GetDropItems()
    {
        List<CountableItem> itemDrops = new List<CountableItem>();
        for (int i = 0; i < _lootTable.Length; i++)
        {
            if (ProbabilityCheck.GetThisChanceResult_Percentage(_lootTable[i].dropRate))
            {
                int count = Random.Range(_lootTable[i].dropCount.x, _lootTable[i].dropCount.y);
                var item = _lootTable[i].item.CreateItem() as CountableItem;
                item.SetAmount(count);
                itemDrops.Add(item);
            }
        }
        return itemDrops;

    }

    //public IDroppable DropItem(Transform dropTransform)
    //{

    //    if (_lootTable.Length <= 0) return null;
    //    int count = 1;
    //    foreach (ItemDrop itdp in _lootTable)
    //    {
    //        bool dropSuccess = ProbabilityCheck.GetThisChanceResult_Percentage(itdp.dropRate);
    //        if (dropSuccess)
    //        {
    //            int dropNum = Random.Range(itdp.dropCount.x, itdp.dropCount.y);

    //            //JumpDrop jd = DropItemManager.Instance.GetEmptyDrop();
    //            //jd.transform.position = dropTransform.position;

    //            //float dropTime = DropItemManager.Instance.dropIntervalTime * count;
    //            //jd.SetItem(itdp.item, itdp.item.rarity, dropNum, dropTime);

    //            count += 1;

    //            //FunctionTimer.Create(() => jd.gameObject.SetActive(true), dropTime);
    //            return itdp.item;

    //        }
    //    }
    //    //JumpDrop jdrop = DropItemManager.Instance.GetEmptyDrop();
    //    //jdrop.transform.position = dropTransform.position;
    //    //jdrop.SetCoin(Random.Range(gold.x, gold.y + 1));


    //}


}


