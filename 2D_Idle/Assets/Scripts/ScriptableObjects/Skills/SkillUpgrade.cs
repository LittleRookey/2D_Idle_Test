using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillUpgrade : ScriptableObject
{
    public int requiredLevel;

    public List<StatModifier> statModifiers;
    [Header("업그레이드 설명")]
    [TextArea]
    public string upgradeExplanation;

    public abstract void ApplyUpgrade(Skill skill);
}

