using UnityEngine;


namespace Litkey.InventorySystem
{
    /// <summary> ��� - �� ������ </summary>
    [CreateAssetMenu(fileName = "Item_Accessory_", menuName = "Inventory System/Item Data/Accessory")]
    public class AccessoryItemData : EquipmentItemData
    {
        /// <summary> ���� </summary>
        public override Item CreateItem(string newID = default)
        {
            string _id = $"{intID.ToString()}_{Name}_{UniqueIDGenerator.GenerateUniqueID()}";
            if (!string.IsNullOrEmpty(newID))
            {
                _id = newID;
            }
            //ResourceManager.Instance.MakeRandomStats(item);
            return new AccessoryItem(this, _id);
        }
    }
}
