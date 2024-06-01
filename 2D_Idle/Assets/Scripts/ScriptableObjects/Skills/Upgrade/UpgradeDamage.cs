using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeDamage", menuName = "Litkey/SkillDecorators/UpgradeDamage")]
public class UpgradeDamage : Decorator
{
    public float additionalDamagePercent;

    public override void AddEffect(Skill skill)
    {
        if (!isUnlocked) return;
        skill.AddSkillDamage(additionalDamagePercent);
    }


}
