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

    public SubStat ExtraGold;
    public SubStat ExtraExp;

    //public UnitLevel unitLevel;

    private void Awake()
    {
        HP = new SubStat("체력", baseStat.MaxHP, eSubStatType.health);
        Attack = new SubStat("공격력", baseStat.Attack, eSubStatType.attack);
        Defense = new SubStat("방어력", baseStat.Defense, eSubStatType.defense);
        AttackSpeed = new SubStat("공격속도", baseStat.AttackSpeed, eSubStatType.attackSpeed, true);
        MoveSpeed = new SubStat("이동속도", baseStat.MoveSpeed, eSubStatType.moveSpeed, true);
        CritChance = new SubStat("크리티컬 확률", baseStat.CritChance, eSubStatType.critChance, true);
        CritDamage = new SubStat("크리티컬 데미지", baseStat.CritDamage, eSubStatType.critDamage, true);
        ExtraGold = new SubStat("골드 추가흭득량", baseStat.ExtraGold, eSubStatType.추가골드, true);
        ExtraExp = new SubStat("경험치 추가흭득량", baseStat.ExtraExp, eSubStatType.추가경험치, true);
    }
    
    public Damage GetFinalDamage()
    {
        if (ProbabilityCheck.GetThisChanceResult(CritChance.FinalValue))
        {
            return new Damage(Attack.FinalValue * (1 + CritDamage.FinalValue), true);
        }
        return new Damage(Attack.FinalValue, false);
    }

    public Damage GetSkillDamage(float skillDmgPercent)
    {
        if (ProbabilityCheck.GetThisChanceResult(CritChance.FinalValue))
        {
            return new Damage((Attack.FinalValue * skillDmgPercent) * (1 + CritDamage.FinalValue), true);
        }
        return new Damage(Attack.FinalValue * skillDmgPercent, false);
    }

    public float Defend(float inComingDamage)
    {
        return Mathf.Max(1f, inComingDamage - Defense.FinalValue);
    }

    public void AddMaxHealth(float val)
    {
        HP.AddStatValue(val);
    }
}
