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

        //MonsterPassive
        public override void ApplyEffect(StatContainer allyStat, StatContainer target)
        {
            
        }

        public override void EquipPassiveStat(StatContainer statContainer)
        {
            statContainer.ApplyBuff(buff, 1);
        }

        public override void SetInitialState()
        {
            
        }

        protected override void OnLevelUp()
        {
            
        }

        protected override void OnRankUp(eSkillRank rank)
        {
            
        }
    }
}
