using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AdditionalAttackDecorator", menuName = "Litkey/SkillDecorators/AdditionalAttack")]
public class AdditionalAttackDecorator : PassiveSkillDecorator
{
    public float additionalDamagePercent;

    protected override void AddPassiveEffect(StatContainer ally, StatContainer target)
    {
        // Assuming target has a method to take additional damage
        var dmg = target.GetDamageAgainst(target, additionalDamagePercent);
        //float additionalDamage = target.GetComponent<Health>().CurrentHealth * (additionalDamagePercent / 100f);
        target.GetComponent<Health>().TakeDamage(ally, new List<Damage>() { dmg });
    }
}
