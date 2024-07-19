using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SkillLevels", menuName = "Litkey/SkillLevels")]
public class SkillLevel : UnitLevel
{
    [Header("Skill Settings")]
    public string skillName;
    public int extraLevelOnRankup; // ��ų ��ũ���ҋ� �߰� ����
    public bool isMaxLevel;

    public UnityEvent OnLevelMax;

    private void Awake()
    {
        level = 1;
        currentExp = 0;
    }

    // ��ų �ش� ������ ���޽�, ���ο� �ż���, ��ũ���� ȣ���� ���� �ʱ�ȭ �� ȿ�� �ο�
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

    // ��ų ��ȭ�� ����
    public override void OnReachMaxLevel()
    {
        // ���� ����ġ �ʱ�ȭ
        this.SetLevel(1, 0f);
        isMaxLevel = false;

        this.maxExp = 150f;
        this.initMaxExp = this.maxExp;

        UpdateMaxExpsPerLevel();
        OnLevelMax?.Invoke();
    }

    public override void SetLevel(int level, float currentExp)
    {
        this.level = level;
        maxExp = MaxExpByLevel[level - 1];
        this.currentExp = currentExp;
        OnInitSetup?.Invoke(currentExp, maxExp);
        isMaxLevel = level == maxLevel;
        
    }

    public SkillLevel CloneSkillLevel()
    {
        return this.MemberwiseClone() as SkillLevel;
    }
}
