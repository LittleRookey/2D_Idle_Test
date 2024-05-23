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
        //Init();
    }

    private void OnEnable()
    {
        // 레벨 로드하고 
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

    // 스킬 레벨업 했을떄 불림
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
        // 전에 있던 스텟들을 제거
        Debug.Log("AppliedLevelUpgrades: " + _appliedLevelUpgrades.Count);
        statContainer.AddETCStat(_appliedLevelUpgrades);
        // 스텟 
        
    }
}

