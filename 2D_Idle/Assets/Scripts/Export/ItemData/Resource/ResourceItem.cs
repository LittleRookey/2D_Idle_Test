using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.InventorySystem
{
    public class ResourceItem : CountableItem
    {
        public ResourceItem(CountableItemData data, string id, int amount = 1) : base(data, id, amount)
        {

        }
    }
}
