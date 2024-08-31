
using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.Skill
{

    public class DebuffPassive : PassiveSkill
    {
        [SerializeField] private Buff debuff;
        [Range(0,1f)]
        [SerializeField] private float applyChance = 1f; // 100% by default

        public override void OnTrigger(string triggerEvent, params object[] args)
        {
            if (triggerEvent == "OnAttack" && args.Length > 0 && args[0] is Health target)
            {
                if (Random.value <= applyChance)
                {
                    ApplyDebuffEffect(target);
                }
            }
        }

        private void ApplyDebuffEffect(Health target)
        {
            BuffReceiver buffReceiver = target.GetComponent<BuffReceiver>();
            if (buffReceiver != null)
            {
                buffReceiver.GiveBuff(debuff);
            }
        }
    }
}
