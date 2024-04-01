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
        HP = new SubStat("ü��", baseStat.MaxHP, eSubStatType.health);
        Attack = new SubStat("���ݷ�", baseStat.Attack, eSubStatType.attack);
        Defense = new SubStat("����", baseStat.Defense, eSubStatType.defense);
        AttackSpeed = new SubStat("���ݼӵ�", baseStat.AttackSpeed, eSubStatType.attackSpeed, true);
        MoveSpeed = new SubStat("�̵��ӵ�", baseStat.MoveSpeed, eSubStatType.moveSpeed, true);
        CritChance = new SubStat("ũ��Ƽ�� Ȯ��", baseStat.CritChance, eSubStatType.critChance, true);
        CritDamage = new SubStat("ũ��Ƽ�� ������", baseStat.CritDamage, eSubStatType.critDamage, true);
        ExtraGold = new SubStat("��� �߰�ŉ�淮", baseStat.ExtraGold, eSubStatType.�߰����, true);
        ExtraExp = new SubStat("����ġ �߰�ŉ�淮", baseStat.ExtraExp, eSubStatType.�߰�����ġ, true);
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
