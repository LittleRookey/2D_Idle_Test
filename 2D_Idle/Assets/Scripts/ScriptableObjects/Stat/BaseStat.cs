using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Litkey.Stat
{

    [InlineEditor]
    [CreateAssetMenu(fileName ="BaseStat", menuName ="Litkey/BaseStat")]
    public class BaseStat : ScriptableObject
    {
        [BoxGroup("BasicInfo")]
        [LabelWidth(100)]
        public int MonsterLevel;

        [BoxGroup("BasicInfo")]
        [LabelWidth(100)]
        public float MaxHP;
        [BoxGroup("BasicInfo")]
        [LabelWidth(100)]
        public float Attack;
        [BoxGroup("BasicInfo")]
        [LabelWidth(100)]
        public float MagicAttack;
        [BoxGroup("BasicInfo")]
        [LabelWidth(100)]
        public float Defense;
        [BoxGroup("BasicInfo")]
        [LabelWidth(100)]
        public float MagicDefense;
        [BoxGroup("BasicInfo")]
        [LabelWidth(100)]
        public float AttackSpeed;
        [BoxGroup("BasicInfo")]
        [LabelWidth(100)]
        public float MoveSpeed;
        [Range(0f, 1f)]
        [BoxGroup("BasicInfo")]
        [LabelWidth(100)]
        public float CritChance;
        [Range(0f, 1f)]
        [BoxGroup("BasicInfo")]
        [LabelWidth(100)]
        public float CritDamage;
        [Range(0, 100f)]
        [BoxGroup("BasicInfo")]
        [LabelWidth(100)]
        public float Precision;
        [Range(0, 100f)]
        [BoxGroup("BasicInfo")]
        [LabelWidth(100)]
        public float Evasion;

        [Range(0, 100f)]
        [BoxGroup("BasicInfo")]
        [LabelWidth(100)]
        public float p_penetration; // %
        [Range(0, 100f)]
        [BoxGroup("BasicInfo")]
        [LabelWidth(100)]
        public float magic_penetration; // %

        [Range(0, 100f)]
        [BoxGroup("BasicInfo")]
        [LabelWidth(100)]
        public float magic_resist; // %
        [Range(0, 100f)]
        [BoxGroup("BasicInfo")]
        [LabelWidth(100)]
        public float p_resist; // %
        [BoxGroup("BasicInfo")]
        [LabelWidth(100)]
        public float ExtraGold; // %
        [BoxGroup("BasicInfo")]
        [LabelWidth(100)]
        public float ExtraExp; // %
        [LabelWidth(100)]
        public float Defense_Penetration; // %
        [LabelWidth(100)]
        public float GiveMoreDamage; // %
        [LabelWidth(100)]
        public float GiveLessDamage; // %
        [LabelWidth(100)]
        public float ReceiveMoreDamage; // %
        [LabelWidth(100)]
        public float ReceiveLessDamage; // %

        public float Attack_Interval;
        //[Space]
        //public float MaxHP_Plus;
        //public float MaxHP_Multiply;

        //public float Attack_Plus;
        //public float Attack_Multiply;
        //public float Defense_Plus;
        //public float Defense_Multiply;
        //public float AttackSpeed_Plus;
        //public float AttackSpeed_Multiply;
        //public float MoveSpeed_Plus;
        //public float MoveSpeed_Multiply;

        //public float CritChance_Plus;
        //public float CritChance_Multiply;
        //public float CritDamage_Plus;
        //public float CritDamage_Multiply;
        
        //public float AttackEnemy(StatContainer enemy)
        //{
        //    float Ally_Attack = (Attack * (1 + Attack_Multiply) + Attack_Plus);
        //    float Enemy_Defense = (enemy.Defense * (1 + enemy.Defense_Multiply) + enemy.Defense_Plus);
        //    return Ally_Attack - Enemy_Defense > 0 ? Ally_Attack - Enemy_Defense : 1f;
        //}

    }
}
