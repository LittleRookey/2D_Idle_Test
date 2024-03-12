using UnityEngine;
using Litkey.Stat;

using System.Collections.Generic;

namespace Litkey.InventorySystem
{
    //public interface IEquippable
    //{
    //    public void Equip();
    //}
    public enum StatType
    {
        maxHP,
        mana,
        hpRegen,
        manaGen,
        p_attack,
        p_critChance,
        p_critDMG,
        p_defense,
        p_penetration,
        attackSpeed,
        moveSpeed
    };

    public enum OperatorType
    {
        plus,
        multiply,
        divide,
        subtract

    }
    
    /// <summary> 장비 아이템 </summary>
    public abstract class EquipmentItemData : ItemData, IEquippable
    {
        public int MaxDurability => _maxDurability;
        [SerializeField] private int _maxDurability = 100;
        [SerializeField] protected StatModifier[] stats;
        //public bool IsEquipped => _isEquipped;
        //protected bool _isEquipped = false;
        [SerializeField] protected eEquipmentParts _parts;
        protected int upgradeNum = 0;
        public int UpgradeNum => upgradeNum;
        public StatModifier[] GetStats()
        {
            return stats;
        }

        public List<StatModifier> GetStats(eSubStatType statType)
        {
            List<StatModifier> statTypes = new List<StatModifier>();
            for (int i = 0; i < stats.Length; i++)
            {
                if (stats[i].IsStatType(statType)) statTypes.Add(stats[i]);
            }

            return statTypes;
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

        public void Upgrade()
        {
            upgradeNum += 1;
            // 업그레이드하고 스텟을 몇올리는지 등등

        }
        //public override ItemSaveData ToSaveData(int currentAmount)
        //{
        //    itemType = "Equipment";
        //    return new EquipItemSaveData(this, 1, _parts, IsEquipped);
        //}
    }

    

}