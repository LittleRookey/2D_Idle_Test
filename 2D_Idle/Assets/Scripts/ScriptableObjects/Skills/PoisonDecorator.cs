using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.Skill
{
    public class PoisonDecorator : PassiveSkill
    {
        public float poisonDamagePerTick;
        public int poisonDuration;

        public override void EquipPassiveStat(StatContainer statContainer)
        {
            
        }

        protected override void OnLevelUp()
        {
            throw new NotImplementedException();
        }


    }
}