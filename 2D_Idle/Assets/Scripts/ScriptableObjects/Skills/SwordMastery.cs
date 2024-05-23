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
        //Init();
    }

    private void OnEnable()
    {
        // ���� �ε��ϰ� 
        OnLevelUp();


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
        _appliedLevelUpgrades.Clear();
        int currentLevel = this.Level.level;
        for (int i = 0; i < currentLevel; i++)
        {
            if (levelUpgrades.ContainsKey(i))
            {
                CombineStats(levelUpgrades[i]);
            }
        }

    }


    public override void Initialize()
    {
        Init();

    }
    public void Init()
    {
        _appliedLevelUpgrades = new List<StatModifier>();
        this.Level.SetLevel(1,0f);

        int currentLevel = this.Level.level;
        Debug.Log("Sword Mastery init level: " + currentLevel);
        for (int i = 0; i < currentLevel; i++)
        {
            Debug.Log("Sword Mastery init: " + levelUpgrades.ContainsKey(i));
            if (levelUpgrades.ContainsKey(i))
            {
                CombineStats(levelUpgrades[i]);
            }
        }
    }

    //public List<StatModifier> GetPassiveStats()
    //{

    //}
   
    private void CombineStats(List<StatModifier> stats)
    {
        Debug.Log("Combined stats");
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
        Debug.Log("Equip Stat called from Sword master");
        // ���� �ִ� ���ݵ��� ����
        Debug.Log("AppliedLevelUpgrades: " + _appliedLevelUpgrades.Count);
        statContainer.AddETCStat(_appliedLevelUpgrades);
        // ���� 
        
    }
}

