using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SwordMastery", menuName = "Litkey/Skills/SwordMastery")]
public class SwordMastery : PassiveSkillDecorator
{
    public Dictionary<eSkillRank, SwordMasteryUpgrade> rankUpgrades => _rankUpgrades; 
    [SerializeField] private Dictionary<eSkillRank, SwordMasteryUpgrade> _rankUpgrades; // � ��ũ ���׷��̵���� �ִ��� ���� 
    [SerializeField] private Dictionary<eSkillRank, List<StatModifier>> levelUpgrades; // � ���� ���׷��̵���� �ִ��� ����

    private List<SwordMasteryUpgrade> appliedRankUpgrades; // ����� ��ũ ȿ���� ����
    private List<StatModifier> appliedLevelUpgrades; // ����� ���� ȿ���� ����

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

