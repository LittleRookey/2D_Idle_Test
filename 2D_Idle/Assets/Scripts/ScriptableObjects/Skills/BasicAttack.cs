using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : Skill
{
    [Range(0, 10)]
    public float damagePercent;

    public override void ApplyEffect(Health target)
    {
        var dmg = allyStat.GetFinalDamage();
        allyStat.GetDamageAgainst(target.GetComponent<StatContainer>());

        //Target.GetComponent<StatContainer>().Defend(dmg.damage);
        target.TakeDamage(allyStat.GetComponent<LevelSystem>(), new List<Damage> { dmg });
        //target.TakeDamage(damage);
    }

   
}
