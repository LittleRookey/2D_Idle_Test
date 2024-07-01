﻿

using Litkey.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.InventorySystem
{
    /// <summary> 장비 아이템</summary>
    [System.Serializable]
    public abstract class EquipmentItem : Item
    {
        public EquipmentItemData EquipmentData { get; private set; }

        /// <summary> 현재 내구도 </summary>
        public int Durability
        {
            get => _durability;
            set
            {
                if (value < 0) value = 0;
                if (value > EquipmentData.MaxDurability)
                    value = EquipmentData.MaxDurability;

                _durability = value;
            }
        }
        private int _durability;

        public bool HasDurability() => _durability > 0;
        public EquipmentItem(EquipmentItemData data, string uniqueID) : base(data, uniqueID)
        {
            EquipmentData = data;
            Durability = data.MaxDurability;

        }

        public EquipmentItem(EquipmentItemData data, int currentDurability, string uniqueID) : base(data, uniqueID)
        {
            EquipmentData = data;
            Durability = currentDurability;

        }



    }
}