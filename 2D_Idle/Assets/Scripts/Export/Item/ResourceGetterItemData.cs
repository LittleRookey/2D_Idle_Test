using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Litkey.InventorySystem
{
    [CreateAssetMenu(menuName ="Inventory System/Item Data/ResourceGetterItemData")]
    public class ResourceGetterItemData : EquipmentItemData
    {
        public override Item CreateItem()
        {
            string _id = $"{intID.ToString()}_{Name}_{UniqueIDGenerator.GenerateUniqueID()}";
            return new ResourceGetterItem(this, MaxDurability, _id);
        }
    }


}
