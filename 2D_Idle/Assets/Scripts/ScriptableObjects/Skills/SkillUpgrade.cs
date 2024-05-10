using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUpgrade : ScriptableObject
{
    public int requiredLevel;
    public virtual void ApplyUpgrade(Health target)
    {
        // apply ugprade effect

    }

}
