using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Litkey.InventorySystem
{
    [CreateAssetMenu(menuName ="Inventory System/Item Data/ResourceGetterItemData")]
    public class ResourceGetterItemData : EquipmentItemData
    {

        public override Item CreateItem(string newID=default)
        {

            string _id = $"{intID.ToString()}_{Name}_{UniqueIDGenerator.GenerateUniqueID()}";
            if (!string.IsNullOrEmpty(newID))
            {
                _id = newID;
            }
            return new ResourceGetterItem(this, MaxDurability, _id);
        }
    }


}
