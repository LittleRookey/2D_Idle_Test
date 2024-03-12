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
        HP = new SubStat("ü��", baseStat.MaxHP, eSubStatType.health);
        Attack = new SubStat("���ݷ�", baseStat.Attack, eSubStatType.attack);
        Defense = new SubStat("����", baseStat.Defense, eSubStatType.defense);
        AttackSpeed = new SubStat("���ݼӵ�", baseStat.AttackSpeed, eSubStatType.attackSpeed, true);
        MoveSpeed = new SubStat("�̵��ӵ�", baseStat.MoveSpeed, eSubStatType.moveSpeed, true);
        CritChance = new SubStat("ũ��Ƽ�� Ȯ��", baseStat.CritChance, eSubStatType.critChance, true);
        CritDamage = new SubStat("ũ��Ƽ�� ������", baseStat.CritDamage, eSubStatType.critDamage, true);

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
