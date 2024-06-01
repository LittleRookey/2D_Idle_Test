using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Litkey/Skills/Basic Attack/EnemyBasicAttack")]
public class EnemyBasicAttack : BasicAttack
{
    public override void ApplyEffect(StatContainer allyStat, StatContainer target)
    {
        var dmg = allyStat.GetDamageAgainst(target);

        target.GetComponent<Health>().TakeDamage(allyStat, new List<Damage> { dmg }, false);
    }
}