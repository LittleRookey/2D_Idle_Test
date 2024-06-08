using UnityEngine;

namespace Litkey.InventorySystem
{
    /// <summary> 장비 - 무기 아이템 </summary>
    [CreateAssetMenu(fileName = "Item_Weapon_", menuName = "Inventory System/Item Data/Weaopn")]
    public class WeaponItemData : EquipmentItemData
    {
        /// <summary> 공격력 </summary>

        public override Item CreateItem()
        {
            string _id = $"{intID.ToString()}_{Name}_{UniqueIDGenerator.GenerateUniqueID()}";
            var item = new WeaponItem(this, _id);

            return item;
        }

        
    }
}