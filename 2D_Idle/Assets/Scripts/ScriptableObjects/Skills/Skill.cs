using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : ScriptableObject
{
    public Sprite _icon;
    public string skillName;
    public int skillLevel;
    [SerializeField] protected string abilityUseSound;
    [SerializeField] protected StatContainer allyStat;
    public abstract void ApplyEffect(Health target);
}

public abstract class PassiveSkillDecorator : Skill
{
    [SerializeField] protected Skill decoratedSkill;
    public void Initialize(Skill skill)
    {
        decoratedSkill = skill;
    }

    public override void ApplyEffect(Health target)
    {

        decoratedSkill.ApplyEffect(target);
        AddPassiveEffect(target);
    }

    protected abstract void AddPassiveEffect(Health target);
}

public abstract class ActiveSkill : Skill
{
    public float cooldown;

    public abstract void Use(Health target);
}