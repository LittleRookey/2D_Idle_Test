using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillUpgrade : ScriptableObject
{
    public int requiredLevel;

    public List<StatModifier> statModifiers;
    [Header("���׷��̵� ����")]
    [TextArea]
    public string upgradeExplanation;

    public abstract void ApplyUpgrade(Skill skill);
}

