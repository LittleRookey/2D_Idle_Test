using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Litkey.InventorySystem
{

    [CreateAssetMenu(fileName = "New Upgrade Data", menuName = "Litkey/Inventory/Upgrade Data")]
    public class EquipmentUpgradeData : ScriptableObject
    {
        [SerializeField] private ItemRarity equipmentRarity;
        [SerializeField] private eEquipmentParts parts;
        [SerializeField] private int maxLevel;
        [SerializeField] private int base_UpgradeGold;

        public int MaxLevel => maxLevel;
         
        [System.Serializable]
        public class UpgradeRequirements
        {
            public int goldCost;
            public List<ItemRequirement> requiredItems;
        }

        [System.Serializable]
        public class ItemRequirement
        { 
            [HorizontalGroup("ItemIcon")]
            [PreviewField]
            public ItemData item;
            [HorizontalGroup("ItemIcon")]
            public int quantity;
        }

        [System.Serializable]
        public class UpgradeLevel
        {
            public int level;
            public List<StatModifier> statModifiers;
            public UpgradeRequirements requirements;
        }

        public List<UpgradeLevel> upgradeLevels;

        public List<StatModifier> GetUpgradeModifiers(int level)
        {
            return upgradeLevels.Find(u => u.level == level)?.statModifiers ?? new List<StatModifier>();
        }

        public UpgradeRequirements GetUpgradeRequirements(int level)
        {
            return upgradeLevels.Find(u => u.level == level)?.requirements;
        }

        public int GetMaxUpgradeLevel()
        {
            return upgradeLevels.Count;
        }

        // 기본 골드 비용 계산 함수 (장비 등급에 따라 다르게 설정 가능)
        public int CalculateBaseGoldCost(int level)
        {
            float rarityMultiplier = GetRarityMultiplier(equipmentRarity);
            return Mathf.RoundToInt(base_UpgradeGold * (level+1) * rarityMultiplier);
        }

        private float GetRarityMultiplier(ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.일반: return 1f;
                case ItemRarity.고급: return 1.5f;
                case ItemRarity.희귀: return 2f;
                case ItemRarity.영웅: return 3f;
                case ItemRarity.전설: return 5f;
                case ItemRarity.초월: return 7f;
                case ItemRarity.신화: return 9f;
                default: return 1f;
            }
        }

        // 에디터에서 기본 업그레이드 데이터 생성을 위한 함수
        [Button("Generate Default Upgrade Levels")]
        private void GenerateDefaultUpgradeLevels(ItemData itemData)
        {
            upgradeLevels = new List<UpgradeLevel>();
            for (int i = 0; i < maxLevel; i++) // 예: 10레벨까지 생성
            {
                UpgradeLevel level = new UpgradeLevel
                {
                    level = i,
                    statModifiers = new List<StatModifier>
                    {
                        new StatModifier { statType = eSubStatType.물리공격력, oper = OperatorType.plus, value = i * 5 }
                    },
                    requirements = new UpgradeRequirements
                    {
                        goldCost = CalculateBaseGoldCost(i),
                        requiredItems = new List<ItemRequirement>
                        {
                            new ItemRequirement { item = itemData, quantity = i } // 여기에 실제 강화석 ItemData를 넣어야 합니다.
                        }
                    }
                };
                upgradeLevels.Add(level);
            }
        }
    }
}