using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Litkey.Stat;
using UnityEngine.Events;
using Litkey.Character.Cooldowns;

public enum eSkillRank
{
    �Ϲ�,
    ���,
    ���,
    ����,
    ����,
    �ʿ�,
    ���,
    �Ҹ�,
    ��ȭ,
}
public abstract class Skill : SerializedScriptableObject
{
    public Sprite _icon;
    public string skillName;
    public int skillLevel => Level.level;
    public eSkillRank startRank;
    public eSkillRank currentRank;
    public eSkillRank maxRank;
    public float baseSkillDamage;
    public SkillLevel Level;

    [SerializeField] protected string abilityUseSound;

    // ��ũ�� ���׷��̵� ���ݵ�
    public Dictionary<eSkillRank, List<Decorator>> rankUpgrades;


    protected float _addedSkillDamage = 0f;

    public float finalDamage => baseSkillDamage + _addedSkillDamage;

    [TextArea]
    public string skillExplanation;

    protected string skillLevelPath = "ScriptableObject/Skills/SkillLevel/SkillLevels";

    protected virtual void Awake()
    {
        //Level = Resources.Load<SkillLevel>(skillLevelPath).CloneSkillLevel();
    }

    public abstract void ApplyEffect(StatContainer allyStat, StatContainer target);

    public void IncreaseOneExp()
    {
        if (this.Level != null)
            this.Level.GainExp(1);
    }

    public eSkillRank GetMaxUpgradeRank()
    {
        return maxRank;

    }
    public virtual float GetTotalDamage(eSkillRank topRank) { return 0f; }

    public void AddSkillDamage(float additionalDmg) => _addedSkillDamage += additionalDmg;

    public void RemoveSkillDamage(float additionalDMg) => _addedSkillDamage -= additionalDMg;
}

public abstract class PassiveSkill : Skill
{
    [HorizontalGroup("Split", 0.9f)]

    

    // ������ ���׷��̵� ���ݵ�
    [SerializeField] protected Dictionary<eSkillRank, Dictionary<int, List<StatModifier>>> levelUpgrades; // � ���� ���׷��̵���� �ִ��� ����
    public Dictionary<eSkillRank, Dictionary<int, List<StatModifier>>> LevelUpgrades => levelUpgrades;
    //private List<SwordMasteryUpgrade> appliedRankUpgrades; // ����� ��ũ ȿ���� ����

    public List<StatModifier> AppliedLevelUpgrades => _appliedLevelUpgrades;
    protected List<StatModifier> _appliedLevelUpgrades; // ����� ���� ȿ���� ����
    public UnityEvent<PassiveSkill> OnSkillLevelUp;
    
    public virtual void Initialize()
    {
        
    }

    protected abstract void OnRankUp(eSkillRank rank);
    protected abstract void OnLevelUp();

    
    public abstract void EquipPassiveStat(StatContainer statContainer);

    
    

}

public abstract class ActiveSkill : Skill, IHasCooldown
{
    public float cooldown;
    public float TimeTilActive; // �ߵ����� �ɸ��½ð�
    public int targetNumber;
    public int attackNumber; // ����Ƚ��
    public string animationName;
    public GameObject skillEffect;

    public Dictionary<eSkillRank, Dictionary<int, float>> skillDamagePerLevel;
    public Dictionary<eSkillRank, float> skillDamagePerRank;
    public List<SkillCondition> SkillConditions;

    public int totalScore;
    public string ID => skillName;

    public float CooldownDuration => cooldown;

    public UnityEvent OnSkillUse;
    public UnityEvent<ActiveSkill> OnSkillLevelUp;

    public abstract void Use(StatContainer ally, Health target);

    private void OnEnable()
    {
        OnSkillUse.AddListener(IncreaseOneExp);
    }
    private void OnDisable()
    {
        OnSkillUse.RemoveListener(IncreaseOneExp);
    }

    public override float GetTotalDamage(eSkillRank topRank)
    {
        float startDMG = baseSkillDamage;

        for (int i = (int)startRank; i < (int)topRank; i++)
        {
            for (int k = 0; k < Level.level; k++)
            {
                startDMG += skillDamagePerLevel[(eSkillRank)i][k];

            }
        }
        return startDMG;
    }

    
    public void IncreaseTargetNumber(int num)
    {
        targetNumber += num;
    }

    public void ReduceCooldown(float cooldown)
    {
        this.cooldown = Mathf.Max(1f, this.cooldown - cooldown);
    }

    public float CalculateSkillScore(StatContainer ally)
    {
        int totalScore = 0;

        foreach (var condition in SkillConditions)
        {
            if (condition.EvaluateCondition(ally))
            {
                totalScore += condition.Score;
            }
        }
        this.totalScore = totalScore;
        return totalScore;
    }

}

public abstract class Decorator : ScriptableObject
{
    public bool isUnlocked;
    
    [Header("���׷��̵� ����")]
    [TextArea]
    public string upgradeExplanation;

    public abstract void AddEffect(Skill skill);

    public void Unlock() => isUnlocked = true;

    public void Lock() => isUnlocked = false;
}