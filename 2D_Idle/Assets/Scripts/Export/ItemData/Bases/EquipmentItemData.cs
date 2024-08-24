using UnityEngine;
using Litkey.Stat;

using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Litkey.InventorySystem
{
    //public interface IEquippable
    //{
    //    public void Equip();
    //}

    public enum OperatorType
    {
        plus,
        multiply,

    }
    
    [InlineEditor]
    /// <summary> 장비 아이템 </summary>
    public abstract class EquipmentItemData : ItemData, IEquippable
    {
        public int MaxDurability => _maxDurability;
        [VerticalGroup("Item Data/Info")]
        [SerializeField] protected int _maxDurability = 100;

        [TableList]
        [SerializeField] protected StatModifier[] baseStats;
        
        [VerticalGroup("Item Data/Info")]
        [SerializeField] protected eEquipmentParts _parts;

        public eEquipmentParts Parts => _parts;


        [SerializeField] protected EquipmentUpgradeData upgradeData;

        public EquipmentUpgradeData UpgradeData => upgradeData;

        public StatModifier[] GetStats()
        {
            return baseStats;
        }

        public List<StatModifier> GetStats(eSubStatType statType)
        {
            List<StatModifier> statTypes = new List<StatModifier>();
            for (int i = 0; i < baseStats.Length; i++)
            {
                if (baseStats[i].IsStatType(statType)) statTypes.Add(baseStats[i]);
            }

            return statTypes;
        }

        public void SetParts(eEquipmentParts parts)
        {
            this._parts = parts;
        }

        public bool Equip()
        {
            //_isEquipped = true;
            return true;
        }

        public void UnEquip()
        {
            //_isEquipped = false;
        }

        public eEquipmentParts GetEquipType()
        {
            return _parts;
        }


    }

    

}