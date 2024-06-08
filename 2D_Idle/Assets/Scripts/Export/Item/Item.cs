

using Litkey.Interface;
using System;

namespace Litkey.InventorySystem
{
    /*
        [��� ����]
        Item : �⺻ ������
            - EquipmentItem : ��� ������
            - CountableItem : ������ �����ϴ� ������
    */
    [System.Serializable]
    public abstract class Item 
    {
        public ItemData Data { get; private set; }
        public string ID => _id; // ����ũ ���̵�

        private string _id;
        public Item(ItemData data, string id)
        {
            Data = data;
            this._id = id; 
        }

        public virtual ItemSaveData ToSaveData()
        {
            return new ItemSaveData(this);
        }
    }

    public static class UniqueIDGenerator
    {
        public static string GenerateUniqueID()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }
    }

    public class ItemSaveData
    {
        public string ID;
        public eItemType itemType;
        public int intID;

        public ItemSaveData(Item item)
        {
            this.ID = item.ID;
            // intID
            this.intID = item.Data.intID;
            if (item is EquipmentItem)
                itemType = eItemType.Equipment;
            else if (item is CountableItem)
                itemType = eItemType.ETC;
        }
    }
}