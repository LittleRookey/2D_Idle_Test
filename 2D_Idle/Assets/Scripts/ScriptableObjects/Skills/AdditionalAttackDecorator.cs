using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AdditionalAttackDecorator", menuName = "Litkey/SkillDecorators/AdditionalAttack")]
public class AdditionalAttackDecorator : PassiveSkill
{
    public float additionalDamagePercent;

    protected void AddPassiveEffect(StatContainer ally, StatContainer target)
    {
        // Assuming target has a method to take additional damage
        var dmg = target.GetDamageAgainst(target, additionalDamagePercent);
        //float additionalDamage = target.GetComponent<Health>().CurrentHealth * (additionalDamagePercent / 100f);
        target.GetComponent<Health>().TakeDamage(ally, new List<Damage>() { dmg });
    }
    public override void ApplyEffect(StatContainer allyStat, StatContainer target)
    {
        // Assuming target has a method to take additional damage
        var dmg = target.GetDamageAgainst(target, additionalDamagePercent);
        //float additionalDamage = target.GetComponent<Health>().CurrentHealth * (additionalDamagePercent / 100f);
        target.GetComponent<Health>().TakeDamage(allyStat, new List<Damage>() { dmg });
    }

    protected override void OnLevelUp()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnRankUp()
    {
        throw new System.NotImplementedException();
    }

    public override void EquipPassiveStat(StatContainer statContainer)
    {
        throw new System.NotImplementedException();
    }
}
