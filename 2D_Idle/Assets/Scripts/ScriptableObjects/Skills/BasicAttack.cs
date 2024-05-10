using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Litkey/Skills/Basic Attack")]
public class BasicAttack : Skill
{
    [Range(0, 10)]
    public float damagePercent;

    public virtual void Initialize(StatContainer stat)
    {
        this.allyStat = stat;
    }

    public override void ApplyEffect(Health target)
    {
        var dmg = allyStat.GetFinalDamage();
        allyStat.GetDamageAgainst(target.GetComponent<StatContainer>());

        target.TakeDamage(allyStat.GetComponent<LevelSystem>(), new List<Damage> { dmg });
    }

   
}
