using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.MasterAudio;

namespace Litkey.Skill
{
    [CreateAssetMenu(menuName = "Litkey/Skills/Basic Attack/PlayerBasicAttack")]
    public class PlayerBasicAttack : BasicAttack
    {
        string swordSound = "일반검베기";
        public override void ApplyEffect(StatContainer allyStat, StatContainer target)
        {
            var dmg = allyStat.GetDamageAgainst(target);
            MasterAudio.PlaySound(swordSound);
            bool isTargetDead = target.GetComponent<Health>().TakeDamage(allyStat.GetComponent<LevelSystem>(), new List<Damage> { dmg }, true);
            if (isTargetDead)
            {
                allyStat.GetComponent<PlayerController>().SetTargetNull();
            }
            OnApplyEffect?.Invoke();
        }

        public override void SetInitialState()
        {
            throw new System.NotImplementedException();
        }
    }
}
