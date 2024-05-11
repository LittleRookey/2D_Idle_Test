using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
[CreateAssetMenu(menuName = "Litkey/Skills/Basic Attack/PlayerBasicAttack")]
public class PlayerBasicAttack : BasicAttack
{
    public override void ApplyEffect(StatContainer allyStat, StatContainer target)
    {
        var dmg = allyStat.GetDamageAgainst(target);

        target.GetComponent<Health>().TakeDamage(allyStat.GetComponent<LevelSystem>(), new List<Damage> { dmg });
    }
}
[CreateAssetMenu(menuName = "Litkey/Skills/Basic Attack/EnemyBasicAttack")]
public class EnemyBasicAttack : BasicAttack
{
    public override void ApplyEffect(StatContainer allyStat, StatContainer target)
    {
        var dmg = allyStat.GetDamageAgainst(target);

        target.GetComponent<Health>().TakeDamage(allyStat, new List<Damage> { dmg });
    }
}
