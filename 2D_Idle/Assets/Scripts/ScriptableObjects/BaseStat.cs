using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.Stat
{

    [CreateAssetMenu(fileName ="BaseStat", menuName ="Litkey/BaseStat")]
    public class BaseStat : ScriptableObject
    {
        public int MonsterLevel;
        [Header("±âº»½ºÅÝ")]
        public float MaxHP;

        public float Attack;
        public float MagicAttack;

        public float Defense;
        public float MagicDefense;

        public float AttackSpeed;
        public float MoveSpeed;
        [Range(0f, 1f)]
        public float CritChance;
        [Range(0f, 1f)]
        public float CritDamage;
        [Range(0, 100f)]
        public float Precision;
        [Range(0, 100f)]
        public float Evasion;

        [Range(0, 100f)]
        public float p_penetration; // %
        [Range(0, 100f)]
        public float magic_penetration; // %

        [Range(0, 100f)]
        public float magic_resist; // %
        [Range(0, 100f)]
        public float p_resist; // %

        public float ExtraGold; // %
        public float ExtraExp; // %

        [Header("¸ó½ºÅÍ ½ºÅÝ")]
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
