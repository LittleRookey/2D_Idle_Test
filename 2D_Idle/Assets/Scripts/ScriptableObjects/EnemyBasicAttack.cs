using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.Skill
{
    [CreateAssetMenu(menuName = "Litkey/Skills/Basic Attack/EnemyBasicAttack")]
    public class EnemyBasicAttack : BasicAttack
    {
        public override void ApplyEffect(StatContainer allyStat, StatContainer target)
        {
            var dmg = allyStat.GetDamageAgainst(target);

            target.GetComponent<Health>().TakeDamage(allyStat, new List<Damage> { dmg }, false);
        }

        public override void SetInitialState()
        {
            throw new System.NotImplementedException();
        }
    }
}