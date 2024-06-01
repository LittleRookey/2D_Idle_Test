using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillUpgrade : ScriptableObject
{
    public int requiredLevel;

    

    public abstract void ApplyUpgrade(Skill skill);
}

