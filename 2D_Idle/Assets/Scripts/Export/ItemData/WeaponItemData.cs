using UnityEngine;

namespace Litkey.InventorySystem
{
    /// <summary> ��� - ���� ������ </summary>
    [CreateAssetMenu(fileName = "Item_Weapon_", menuName = "Inventory System/Item Data/Weaopn")]
    public class WeaponItemData : EquipmentItemData
    {
        /// <summary> ���ݷ� </summary>

        public override Item CreateItem(string newID=default)
        {
            string _id = $"{intID.ToString()}_{Name}_{UniqueIDGenerator.GenerateUniqueID()}";
            if (!string.IsNullOrEmpty(newID))
            {
                _id = newID;
            }

            return new WeaponItem(this, _id);
        }

        
    }
}