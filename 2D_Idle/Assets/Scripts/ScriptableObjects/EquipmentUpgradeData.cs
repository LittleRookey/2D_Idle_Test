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

        // �⺻ ��� ��� ��� �Լ� (��� ��޿� ���� �ٸ��� ���� ����)
        public int CalculateBaseGoldCost(int level)
        {
            float rarityMultiplier = GetRarityMultiplier(equipmentRarity);
            return Mathf.RoundToInt(base_UpgradeGold * (level+1) * rarityMultiplier);
        }

        private float GetRarityMultiplier(ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.�Ϲ�: return 1f;
                case ItemRarity.���: return 1.5f;
                case ItemRarity.���: return 2f;
                case ItemRarity.����: return 3f;
                case ItemRarity.����: return 5f;
                case ItemRarity.�ʿ�: return 7f;
                case ItemRarity.��ȭ: return 9f;
                default: return 1f;
            }
        }

        // �����Ϳ��� �⺻ ���׷��̵� ������ ������ ���� �Լ�
        [Button("Generate Default Upgrade Levels")]
        private void GenerateDefaultUpgradeLevels(ItemData itemData)
        {
            upgradeLevels = new List<UpgradeLevel>();
            for (int i = 0; i < maxLevel; i++) // ��: 10�������� ����
            {
                UpgradeLevel level = new UpgradeLevel
                {
                    level = i,
                    statModifiers = new List<StatModifier>
                    {
                        new StatModifier { statType = eSubStatType.�������ݷ�, oper = OperatorType.plus, value = i * 5 }
                    },
                    requirements = new UpgradeRequirements
                    {
                        goldCost = CalculateBaseGoldCost(i),
                        requiredItems = new List<ItemRequirement>
                        {
                            new ItemRequirement { item = itemData, quantity = i } // ���⿡ ���� ��ȭ�� ItemData�� �־�� �մϴ�.
                        }
                    }
                };
                upgradeLevels.Add(level);
            }
        }
    }
}