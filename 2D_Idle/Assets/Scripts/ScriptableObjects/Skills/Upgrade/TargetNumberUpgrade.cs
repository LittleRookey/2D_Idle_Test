using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IncreaseTargetNumber", menuName = "Litkey/SkillDecorators/IncreaseTargetNumber")]
public class TargetNumberUpgrade : Decorator
{
    [SerializeField] private int additionalTargetNumber = 1;
    public override void AddEffect(Skill skill)
    {
        if (!isUnlocked) return;
        if (skill is ActiveSkill activeSkill)
        {
            activeSkill.IncreaseTargetNumber(additionalTargetNumber);
        }
    }

  
}
