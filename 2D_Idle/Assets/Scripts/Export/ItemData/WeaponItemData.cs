using UnityEngine;

namespace Litkey.InventorySystem
{
    /// <summary> ��� - ���� ������ </summary>
    [CreateAssetMenu(fileName = "Item_Weapon_", menuName = "Inventory System/Item Data/Weaopn")]
    public class WeaponItemData : EquipmentItemData
    {
        /// <summary> ���ݷ� </summary>

        public override Item CreateItem()
        {
            string _id = $"{intID.ToString()}_{Name}_{UniqueIDGenerator.GenerateUniqueID()}";
            var item = new WeaponItem(this, _id);

            return item;
        }

        
    }
}