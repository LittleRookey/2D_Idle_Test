using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Litkey.InventorySystem
{
    /// <summary> 장비 - 무기 아이템 </summary>
    public class WeaponItem : EquipmentItem
    {
        public WeaponItem(WeaponItemData data, string id) : base(data, id) { }

        public WeaponItem(WeaponItemData data, ItemSaveData saveData) : base(data, saveData.ID) { }

    }


}