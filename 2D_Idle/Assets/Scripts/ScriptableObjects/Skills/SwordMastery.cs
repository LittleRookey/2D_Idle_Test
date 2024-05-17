using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SwordMastery", menuName = "Litkey/Skills/SwordMastery")]
public class SwordMastery : PassiveSkillDecorator
{
    public Dictionary<eSkillRank, SwordMasteryUpgrade> rankUpgrades => _rankUpgrades; 
    [SerializeField] private Dictionary<eSkillRank, SwordMasteryUpgrade> _rankUpgrades; // 어떤 랭크 업그레이드들이 있는지 저장 
    [SerializeField] private Dictionary<eSkillRank, List<StatModifier>> levelUpgrades; // 어떤 레벨 업그레이드들이 있는지 저장

    private List<SwordMasteryUpgrade> appliedRankUpgrades; // 적용된 랭크 효과들 모음
    private List<StatModifier> appliedLevelUpgrades; // 적용된 레벨 효과들 모음

    private void Awake()
    {
        appliedRankUpgrades = new List<SwordMasteryUpgrade>();
        appliedLevelUpgrades = new List<StatModifier>();
        
    }
    public void RankUp()
    {
        appliedRankUpgrades.Clear();

        currentRank++;
        foreach (var upgrade in rankUpgrades.Values)
        {
            if ((int)upgrade.rankToApply <= (int)currentRank)
            {
                appliedRankUpgrades.Add(upgrade);
            }
        }
    }

    protected override void AddPassiveEffect(StatContainer ally, StatContainer target)
    {
        throw new System.NotImplementedException();
    }
}

