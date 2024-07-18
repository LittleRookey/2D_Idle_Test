using UnityEngine;


namespace Litkey.InventorySystem
{
    /// <summary> 장비 - 방어구 아이템 </summary>
    [CreateAssetMenu(fileName = "Item_Accessory_", menuName = "Inventory System/Item Data/Accessory")]
    public class AccessoryItemData : EquipmentItemData
    {
        /// <summary> 방어력 </summary>
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
