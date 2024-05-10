using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : ScriptableObject
{
    public Sprite _icon;
    public string skillName;
    public int skillLevel;
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
        AddPassiveEffect(target);
    }

    protected abstract void AddPassiveEffect(StatContainer target);
}

public abstract class ActiveSkill : Skill
{
    public float cooldown;

    public abstract void Use(Health target);
}