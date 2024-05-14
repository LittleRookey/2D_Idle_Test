using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IncreaseSkillDamageUpgrade", menuName = "Litkey/SkillUpgrades/IncreaseSkillDamage")]
public class IncreaseSkillDamageUpgrade : SkillUpgrade
{
    public int additionalDamage;

    public override void ApplyUpgrade(Skill skill)
    {
        if (skill is ActiveSkill activeSkill)
        {
            activeSkill.damage += additionalDamage;
        }
    }
}
