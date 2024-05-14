using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum eSkillRank
{
    ÀÏ¹Ý,
    °í±Þ,
    Èñ±Í,
    ¿µ¿õ,
    Àü¼³,
    ÃÊ¿ù,
    ½ÅÈ­,
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
}

public abstract class PassiveSkillDecorator : Skill
{
    [SerializeField] protected Skill decoratedSkill;
    
    public void Initialize(Skill skill)
    {
        decoratedSkill = skill;
    }

    public override void ApplyEffect(StatContainer allyStat, StatContainer target)
    {

        decoratedSkill.ApplyEffect(allyStat, target);
        AddPassiveEffect(allyStat, target);
    }

    protected abstract void AddPassiveEffect(StatContainer ally, StatContainer target);
}

public abstract class ActiveSkill : Skill
{
    public float cooldown;
    public int damage;

    public abstract void Use(Health target);
}