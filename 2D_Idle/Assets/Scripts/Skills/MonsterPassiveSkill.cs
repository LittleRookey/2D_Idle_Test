using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.Skill
{
    [CreateAssetMenu(menuName ="Litkey/Skill/MonsterPassiveSkill")]
    public class MonsterPassiveSkill : PassiveSkill
    {
        [SerializeField] private Buff buff;


        public override void EquipPassiveStat(StatContainer statContainer)
        {
            statContainer.ApplyBuff(buff, 1);
        }


    }
}
