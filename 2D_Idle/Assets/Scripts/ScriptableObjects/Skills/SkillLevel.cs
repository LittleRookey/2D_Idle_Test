using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SkillLevels", menuName = "Litkey/SkillLevels")]
public class SkillLevel : UnitLevel
{
    [Header("Skill Settings")]
    public string skillName;
    public int extraLevelOnRankup; // 스킬 랭크업할떄 추가 레벨
    public bool isMaxLevel;

    public UnityEvent OnLevelMax;

    private void Awake()
    {
        currentExp = 0;
    }

    // 스킬 해당 만렙에 도달시, 새로운 매서드, 랭크업을 호출후 레벨 초기화 및 효과 부여
    public override bool GainExp(int value)
    {
        bool levelUp = false;
        currentExp += value;
        OnGainExp?.Invoke(currentExp, maxExp);
        while (currentExp >= maxExp || !isMaxLevel)
        {
            currentExp -= maxExp;
            levelUp = true;
            Grow();
        }

        return levelUp;
    }

    public override void Grow()
    {
        level += 1;
        if (level >= maxLevel)
        {
            isMaxLevel = true;
            
        }
        float fin_maxExp = MaxExpByLevel[level - 1];

        maxExp = Mathf.Round(fin_maxExp);

        OnLevelUp?.Invoke(currentExp, maxExp);
    }

    // 스킬 진화떄 쓰임
    public override void OnReachMaxLevel()
    {
        // 레벨 경험치 초기화
        this.SetLevel(1, 0f);
        isMaxLevel = false;

        this.maxExp = 150f;
        this.initMaxExp = this.maxExp;

        UpdateMaxExpsPerLevel();
        OnLevelMax?.Invoke();
    }

    public SkillLevel CloneSkillLevel()
    {
        return this.MemberwiseClone() as SkillLevel;
    }
}
