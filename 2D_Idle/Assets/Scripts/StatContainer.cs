using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Stat;
using Litkey.Utility;

public class StatContainer : MonoBehaviour
{
    [SerializeField] private BaseStat baseStat;

    public SubStat HP;
    public SubStat Attack;
    public SubStat Defense;
    public SubStat AttackSpeed;
    public SubStat MoveSpeed;
    public SubStat CritChance;
    public SubStat CritDamage;

    private void Awake()
    {
        HP = new SubStat("체력", baseStat.MaxHP, eSubStatType.health);
        Attack = new SubStat("공격력", baseStat.Attack, eSubStatType.attack);
        Defense = new SubStat("방어력", baseStat.Defense, eSubStatType.defense);
        AttackSpeed = new SubStat("공격속도", baseStat.AttackSpeed, eSubStatType.attackSpeed, true);
        MoveSpeed = new SubStat("이동속도", baseStat.MoveSpeed, eSubStatType.moveSpeed, true);
        CritChance = new SubStat("크리티컬 확률", baseStat.CritChance, eSubStatType.critChance, true);
        CritDamage = new SubStat("크리티컬 데미지", baseStat.CritDamage, eSubStatType.critDamage, true);

    }
    
    public float GetFinalDamage()
    {
        if (ProbabilityCheck.GetThisChanceResult(CritChance.FinalValue))
        {
            return Attack.FinalValue * (1 + CritDamage.FinalValue);
        }
        return Attack.FinalValue;
    }
}
