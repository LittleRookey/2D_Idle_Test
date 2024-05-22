using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Litkey.Stat;

public enum eSkillRank
{
    �Ϲ�,
    ���,
    ���,
    ����,
    ����,
    �ʿ�,
    ��ȭ,
}
public abstract class Skill : SerializedScriptableObject
{
    public Sprite _icon;
    public string skillName;
    public int skillLevel;
    public eSkillRank startRank;
    public eSkillRank currentRank;
    public eSkillRank maxRank;
    
    public UnitLevel Level;
    [SerializeField] protected string abilityUseSound;
    
    public abstract void ApplyEffect(StatContainer allyStat, StatContainer target);

    public virtual void IncreaseExp(StatContainer allyStat, StatContainer target) { } 
}

public abstract class PassiveSkill : Skill
{
    [SerializeField] protected Skill decoratedSkill;
    [SerializeField] protected Dictionary<int, List<StatModifier>> levelUpgrades; // � ���� ���׷��̵���� �ִ��� ����

    //private List<SwordMasteryUpgrade> appliedRankUpgrades; // ����� ��ũ ȿ���� ����

    public List<StatModifier> AppliedLevelUpgrades => _appliedLevelUpgrades;
    protected List<StatModifier> _appliedLevelUpgrades; // ����� ���� ȿ���� ����
    public void Initialize(Skill skill)
    {
        decoratedSkill = skill;
    }

    protected abstract void OnRankUp();
    protected abstract void OnLevelUp();

    public abstract void EquipPassiveStat(StatContainer statContainer);
}

public abstract class ActiveSkill : Skill
{
    public float cooldown;
    public int damage;

    public abstract void Use(Health target);
}