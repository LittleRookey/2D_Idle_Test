using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SwordMastery", menuName = "Litkey/Skills/SwordMastery")]
public class SwordMastery : PassiveSkill
{
    //public Dictionary<eSkillRank, SwordMasteryUpgrade> rankUpgrades => _rankUpgrades; 
    //[SerializeField] private Dictionary<eSkillRank, SwordMasteryUpgrade> _rankUpgrades; // 어떤 랭크 업그레이드들이 있는지 저장 

    
    //private List<SwordMasteryUpgrade> appliedRankUpgrades; // 적용된 랭크 효과들 모음


    private List<StatModifier> previousAppliedLevelUpgrades; // 전에 적용된 스텟들;

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

    // 스킬 레벨업 했을떄 불림
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

        // 전에 있던 스텟들을 제거
        statContainer.AddETCStat(_appliedLevelUpgrades);
        // 스텟 
        
    }
}

