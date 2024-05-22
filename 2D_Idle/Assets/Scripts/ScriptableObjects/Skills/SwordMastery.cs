using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SwordMastery", menuName = "Litkey/Skills/SwordMastery")]
public class SwordMastery : PassiveSkill
{
    //public Dictionary<eSkillRank, SwordMasteryUpgrade> rankUpgrades => _rankUpgrades; 
    //[SerializeField] private Dictionary<eSkillRank, SwordMasteryUpgrade> _rankUpgrades; // � ��ũ ���׷��̵���� �ִ��� ���� 

    
    //private List<SwordMasteryUpgrade> appliedRankUpgrades; // ����� ��ũ ȿ���� ����


    private List<StatModifier> previousAppliedLevelUpgrades; // ���� ����� ���ݵ�;

    private StatContainer equippedStatContainer;
    private void Awake()
    {
        //appliedRankUpgrades = new List<SwordMasteryUpgrade>();
        _appliedLevelUpgrades = new List<StatModifier>();
        
    }

    protected override void OnRankUp()
    {
        //appliedRankUpgrades.Clear();

        //currentRank++;
        //foreach (var upgrade in rankUpgrades.Values)
        //{
        //    if ((int)upgrade.rankToApply <= (int)currentRank)
        //    {
        //        appliedRankUpgrades.Add(upgrade);
        //    }
        //}
    }

    // ��ų ������ ������ �Ҹ�
    protected override void OnLevelUp()
    {
        int currentLevel = this.Level.level;
        for (int i = 0; i < currentLevel; i++)
        {
            if (levelUpgrades.ContainsKey(i))
            {
                CombineStats(levelUpgrades[i]);
            }
        }

    }

    private void CombineStats(List<StatModifier> stats)
    {
        for (int i = 0; i < stats.Count; i++)
        {
            _appliedLevelUpgrades.Add(stats[i]);
        }
    }

    public override void ApplyEffect(StatContainer allyStat, StatContainer target)
    {
        throw new System.NotImplementedException();
    }

    // 
    public override void EquipPassiveStat(StatContainer statContainer)
    {
        if (equippedStatContainer == null) equippedStatContainer = statContainer;

        // ���� �ִ� ���ݵ��� ����
        statContainer.AddETCStat(_appliedLevelUpgrades);
        // ���� 
        
    }
}

