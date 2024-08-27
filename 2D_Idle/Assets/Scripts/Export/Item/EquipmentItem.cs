

using Litkey.Utility;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Litkey.Interface;
using Litkey.Stat;

namespace Litkey.InventorySystem
{
    /// <summary> 장비 아이템</summary>
    [InlineEditor]
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

        [ShowInInspector]
        public int CurrentUpgrade { get; private set; }

        private List<StatModifier> _currentStats; 

        [ShowInInspector]
        private int _durability;

        public bool HasDurability() => _durability > 0;

        private Dictionary<eSubStatType, float> _additiveStats;
        private Dictionary<eSubStatType, float> _multiplicativeStats;
        public bool Upgrade()
        {
            if (CurrentUpgrade >= EquipmentData.UpgradeData.MaxLevel) return false;
            CurrentUpgrade++;
            UpdateStats();
            return true;
        }

        private void UpdateStats()
        {
            if (EquipmentData.UpgradeData == null) return;
            if (CurrentUpgrade <= 0) return;

            var upgradeModifiers = EquipmentData.UpgradeData.GetUpgradeModifiers(CurrentUpgrade-1);
            
            foreach (var upgradeMod in upgradeModifiers)
            {
                if (upgradeMod.oper == OperatorType.plus)
                {
                    if (!_additiveStats.ContainsKey(upgradeMod.statType))
                    {
                        _additiveStats.Add(upgradeMod.statType,  0f);
                    }
                    _additiveStats[upgradeMod.statType] += upgradeMod.value;
                }
                else if (upgradeMod.oper == OperatorType.multiply)
                {
                    if (!_multiplicativeStats.ContainsKey(upgradeMod.statType))
                    {
                        _multiplicativeStats.Add(upgradeMod.statType, 0f);
                    }
                    _multiplicativeStats[upgradeMod.statType] += upgradeMod.value;
                }
            }
        }

        public EquipmentUpgradeData.UpgradeRequirements GetNextUpgradeRequirements()
        {
            return EquipmentData.UpgradeData.GetUpgradeRequirements(CurrentUpgrade);
        }

        public List<StatModifier> GetNextUpgradeStat()
        {
            return EquipmentData.UpgradeData.GetUpgradeModifiers(CurrentUpgrade);
        }
        public EquipmentItem(EquipmentItemData data, int currentDurability, string uniqueID) : base(data, uniqueID)
        {
            EquipmentData = data;
            Durability = currentDurability;

            _additiveStats = new Dictionary<eSubStatType, float>();
            _multiplicativeStats = new Dictionary<eSubStatType, float>();
        }
        /// <summary>
        /// 기본 스텟을 포함해서 강화스텟까지 +나*을가진 스텟을 가져온다
        /// </summary>
        /// <param name="oper"></param>
        /// <returns></returns>
        public Dictionary<eSubStatType, float> GetFinalStats(OperatorType oper)
        {
            var finalStats = new Dictionary<eSubStatType, float>();

            // 기본스텟들 넣기
            foreach (var baseStat in EquipmentData.GetStats())
            {
                if (baseStat.oper != oper) continue;
                if (!finalStats.ContainsKey(baseStat.statType))
                {
                    finalStats[baseStat.statType] = baseStat.value;
                }
                else
                {
                    finalStats[baseStat.statType] += baseStat.value;
                }
            }
            if (oper == OperatorType.plus)
            {
                foreach (var additiveStat in _additiveStats)
                {
                    if (!finalStats.ContainsKey(additiveStat.Key))
                    {
                        finalStats[additiveStat.Key] = additiveStat.Value;
                    }
                    else
                    {
                        finalStats[additiveStat.Key] += additiveStat.Value;
                    }
                }
            }
            else
            {
                foreach (var multiplicativeStat in _multiplicativeStats)
                {
                    if (finalStats.ContainsKey(multiplicativeStat.Key))
                    {
                        finalStats[multiplicativeStat.Key] += multiplicativeStat.Value;
                    }
                }
            }

            return finalStats;
        }

        public Dictionary<eSubStatType, float> GetFinalStatsWithoutBaseValue(OperatorType oper)
        {
            var finalStats = new Dictionary<eSubStatType, float>();

            if (oper == OperatorType.plus)
            {
                foreach (var additiveStat in _additiveStats)
                {
                    if (!finalStats.ContainsKey(additiveStat.Key))
                    {
                        finalStats[additiveStat.Key] = additiveStat.Value;
                    }
                    else
                    {
                        finalStats[additiveStat.Key] += additiveStat.Value;
                    }
                }
            }
            else
            {
                foreach (var multiplicativeStat in _multiplicativeStats)
                {
                    if (finalStats.ContainsKey(multiplicativeStat.Key))
                    {
                        finalStats[multiplicativeStat.Key] += multiplicativeStat.Value;
                    }
                }
            }

            return finalStats;
        }


        public EquipmentItem(EquipmentItemData data, string uniqueID) : base(data, uniqueID)
        {
            EquipmentData = data;
            Durability = data.MaxDurability;
            CurrentUpgrade = 0;

            _additiveStats = new Dictionary<eSubStatType, float>();
            _multiplicativeStats = new Dictionary<eSubStatType, float>();
            UpdateStats();
        }

        public EquipmentItem(EquipmentItemData data, int currentDurability, int currentUpgrade, string uniqueID) : base(data, uniqueID)
        {
            EquipmentData = data;
            Durability = currentDurability;
            CurrentUpgrade = currentUpgrade;

            _additiveStats = new Dictionary<eSubStatType, float>();
            _multiplicativeStats = new Dictionary<eSubStatType, float>();
            UpdateStats();
        }

    }
}