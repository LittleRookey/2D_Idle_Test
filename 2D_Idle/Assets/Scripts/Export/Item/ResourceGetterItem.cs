using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.InventorySystem
{

    public class ResourceGetterItem : EquipmentItem
    {
        public ResourceGetterItem(EquipmentItemData data, int currentDurability, string uniqueID) : base(data, currentDurability, uniqueID)
        {
        }

        public void Use(int usedDurability=1)
        {
            Durability -= usedDurability;

        } 
    }
}
