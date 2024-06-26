using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Litkey.Skill
{
    public abstract class BasicAttack : Skill
    {
        [Range(0, 10)]
        public float damagePercent;

        public UnityEvent OnApplyEffect;

        public override void ApplyEffect(StatContainer allyStat, StatContainer target)
        {

            var dmg = allyStat.GetDamageAgainst(target);

            target.GetComponent<Health>().TakeDamage(allyStat, new List<Damage> { dmg });
            OnApplyEffect?.Invoke();
        }

    }
}


