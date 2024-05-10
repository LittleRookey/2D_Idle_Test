using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Litkey/Skills/Basic Attack")]
public class BasicAttack : Skill
{
    [Range(0, 10)]
    public float damagePercent;

    public override void ApplyEffect(StatContainer allyStat, StatContainer target)
    {
        //var dmg = allyStat.GetFinalDamage();
        //Debug.Log("ApplyEffect1:" );
        var dmg = allyStat.GetDamageAgainst(target);
        //Debug.Log("ApplyEffect2:" );
        target.GetComponent<Health>().TakeDamage(allyStat, new List<Damage> { dmg });
        //Debug.Log("ApplyEffect3:" );
    }

   
}
