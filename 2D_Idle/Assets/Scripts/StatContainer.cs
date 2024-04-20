using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Stat;
using Litkey.Utility;

public class StatContainer : MonoBehaviour
{
    [SerializeField] private BaseStat baseStat;
    public MainStat Strength; // �ٷ�
    public MainStat Vit; // ����
    public MainStat Avi; // ��ø
    public MainStat Sensation; // ����
    public MainStat Int; // ����

    public SubStat HP; // ü��

    public SubStat Attack; // ���ݷ�
    public SubStat MagicAttack; // ���� ���ݷ�

    public SubStat Defense; // ����
    public SubStat MagicDefense; // ���� ����

    public SubStat AttackSpeed; // ���ݼӵ�
    public SubStat MoveSpeed; // �̵� �ӵ�

    public SubStat CritChance; // ũ��Ƽ�� Ȯ��
    public SubStat CritDamage; // ũ��Ƽ�� ������

    public SubStat Precision; // ����
    public SubStat Evasion; // ȸ��

    public SubStat p_resist; // ���� ���� %
    public SubStat m_resist; // ���� ���� %

    public SubStat p_penetration; // ���� ����� %
    public SubStat m_penetration; // ���� ����� %

    // Ư�� ����
    //public SubStat receiveAdditionalDamage; // �޴� ���� ����
    //public SubStat giveAdditionalDamage; // �ִ� ���� ����

    //public SubStat receiveLessDamage; // �޴� ���� ����
    //public SubStat giveLessDamage; // �ִ� ���� ����

    public SubStat ExtraGold; // �߰� ���
    public SubStat ExtraExp; // �߰� ����ġ 

    //public UnitLevel unitLevel;
    [SerializeField] private Alias alias;

    // ���� ���� ���� ���������� ���ν��� + ���̽� ���� + �̸� ����(����)�� ���� ���� �ɰ��̴�

    private void Awake()
    {
        Strength = new MainStat("�ٷ�", 0);
        Vit = new MainStat("����", 0);
        Avi = new MainStat("��ø", 0);
        Sensation = new MainStat("����", 0);
        Int = new MainStat("����", 0);

        HP = new SubStat("ü��", baseStat.MaxHP, eSubStatType.health);
        Attack = new SubStat("���ݷ�", baseStat.Attack, eSubStatType.attack);
        MagicAttack = new SubStat("���� ���ݷ�", baseStat.MagicAttack, eSubStatType.magicAttack);

        Defense = new SubStat("����", baseStat.Defense, eSubStatType.defense);
        MagicDefense = new SubStat("���� ����", baseStat.MagicDefense, eSubStatType.magicDefense);

        AttackSpeed = new SubStat("���ݼӵ�", baseStat.AttackSpeed, eSubStatType.attackSpeed, true);
        MoveSpeed = new SubStat("�̵��ӵ�", baseStat.MoveSpeed, eSubStatType.moveSpeed, true);

        CritChance = new SubStat("ũ��Ƽ�� Ȯ��", baseStat.CritChance, eSubStatType.critChance, true);
        CritDamage = new SubStat("ũ��Ƽ�� ������", baseStat.CritDamage, eSubStatType.critDamage, true);

        ExtraGold = new SubStat("��� �߰�ŉ�淮", baseStat.ExtraGold, eSubStatType.�߰����, true);
        ExtraExp = new SubStat("����ġ �߰�ŉ�淮", baseStat.ExtraExp, eSubStatType.�߰�����ġ, true);

        Precision = new SubStat("����", baseStat.Precision, eSubStatType.����);
        Evasion = new SubStat("ȸ��", baseStat.Evasion, eSubStatType.ȸ��);

        p_resist = new SubStat("���� ���׷�", baseStat.p_resist, eSubStatType.��������, 0f, 100f);
        m_resist = new SubStat("���� ���׷�", baseStat.magic_resist, eSubStatType.��������, 0f, 100f);
        p_penetration = new SubStat("���� �����", baseStat.p_penetration, eSubStatType.���������, 0f, 100f);
        m_penetration = new SubStat("���� �����", baseStat.magic_penetration, eSubStatType.���������, 0f, 100f);

        //receiveAdditionalDamage = new SubStat("�޴� ���� ����", 0f, eSubStatType.�޴���������, true);
        //giveAdditionalDamage = new SubStat("�ִ� ���� ����", 0f, eSubStatType.�ִ���������, true);
        //receiveLessDamage = new SubStat("�޴� ���� ����", 0f, eSubStatType.�޴����ذ���, true);
        //giveLessDamage = new SubStat("�ִ� ���� ����", 0f, eSubStatType.�޴���������, true);


        Strength.AddSubStatAsChild(Attack);
        Strength.AddSubStatAsChild(HP);

        Vit.AddSubStatAsChild(HP);
        Vit.AddSubStatAsChild(Defense);
        Vit.AddSubStatAsChild(MagicDefense);

        Avi.AddSubStatAsChild(AttackSpeed);
        Avi.AddSubStatAsChild(Evasion);
        Avi.AddSubStatAsChild(CritChance);

        Sensation.AddSubStatAsChild(Precision);
        Sensation.AddSubStatAsChild(CritDamage);

        Int.AddSubStatAsChild(MagicAttack);

        // ���꽺�ݿ��� ���ν��ݰ� ����α�
        HP.AddAsInfluencer(StatUtility.StatPerValue(Strength, 1, 13f));
        HP.AddAsInfluencer(StatUtility.StatPerValue(Vit, 1, 25f));

        Attack.AddAsInfluencer(StatUtility.StatPerValue(Strength, 1, 9f));
        Attack.AddAsInfluencer(StatUtility.StatPerValue(Vit, 1, 3f));

        MagicAttack.AddAsInfluencer(StatUtility.StatPerValue(Int, 1, 15f));

        Defense.AddAsInfluencer(StatUtility.StatPerValue(Vit, 1, 4f));

        MagicDefense.AddAsInfluencer(StatUtility.StatPerValue(Vit, 1, 3f));

        AttackSpeed.AddAsInfluencer(StatUtility.StatPerValue(Avi, 5, 0.01f));

        CritChance.AddAsInfluencer(StatUtility.StatPerValue(Avi, 5, 0.005f));
        CritDamage.AddAsInfluencer(StatUtility.StatPerValue(Sensation, 3, 0.01f));

        //ExtraGold = new SubStat("��� �߰�ŉ�淮", baseStat.ExtraGold, eSubStatType.�߰����, true);
        //ExtraExp = new SubStat("����ġ �߰�ŉ�淮", baseStat.ExtraExp, eSubStatType.�߰�����ġ, true);

        Precision.AddAsInfluencer(StatUtility.StatPerValue(Sensation, 2, 1f));
        //Evasion = new SubStat("ȸ��", baseStat.Evasion, eSubStatType.ȸ��);

        //receiveAdditionalDamage = new SubStat("�޴� ���� ����", 0f, eSubStatType.�޴���������, true);
        //giveAdditionalDamage = new SubStat("�ִ� ���� ����", 0f, eSubStatType.�ִ���������, true);
        //receiveLessDamage = new SubStat("�޴� ���� ����", 0f, eSubStatType.�޴����ذ���, true);
        //giveLessDamage = new SubStat("�ִ� ���� ����", 0f, eSubStatType.�޴���������, true);

    }

    // ü�� - (�� ���ݷ� - ���� (>=0) ) * (100 - (�Ʊ� �������� - �� ��������� >= 0) / 100 or (�Ʊ��������� - �� ���������) / 100)
    // => ��ü�� - ((�ư� - ����) * (100 - (�������� - �Ʊ������ / 100)) 
    // ������ * ( 
    // �����ڰ� �θ�,
    public Damage GetDamageAgainst(StatContainer enemyStat)
    {

        float dmg;
        var m_AttackVal = GetFinalDamage();

        if (m_AttackVal.isPhysicalDmg)
        {
            float attackDmg = (m_AttackVal.damage * (1f + (p_penetration.FinalValue - enemyStat.p_resist.FinalValue))) 
                - (enemyStat.Defense.FinalValue * 1f + (enemyStat.p_resist.FinalValue - p_penetration.FinalValue));


            dmg = (Mathf.Clamp(attackDmg, 1f, 999999999));

        }
        else
        {
            // magic dmg
            //float attackDmg = m_AttackVal.damage - enemyStat.MagicDefense.FinalValue;
            //dmg = (Mathf.Clamp(attackDmg, 1f, 999999999) * (1f + (m_penetration.FinalValue - enemyStat.m_resist.FinalValue) / 100f));
            float attackDmg = (m_AttackVal.damage * (1f + (m_penetration.FinalValue - enemyStat.m_resist.FinalValue)))
                - (enemyStat.MagicDefense.FinalValue * 1f + (enemyStat.m_resist.FinalValue - m_penetration.FinalValue));
            dmg = (Mathf.Clamp(attackDmg, 1f, 999999999));
        }
        return new Damage(dmg, m_AttackVal.isCrit, m_AttackVal.isPhysicalDmg);
    }

    public Damage GetFinalDamage()
    {
        bool isPhysic = Attack.FinalValue >= MagicAttack.FinalValue;
        if (isPhysic)
        {
            if (ProbabilityCheck.GetThisChanceResult(CritChance.FinalValue))
            {
                return new Damage(Attack.FinalValue * (1 + CritDamage.FinalValue), true, isPhysic);
            }
            return new Damage(Attack.FinalValue, false, isPhysic);
        }
        else
        {
            if (ProbabilityCheck.GetThisChanceResult(CritChance.FinalValue))
            {
                return new Damage(MagicAttack.FinalValue * (1 + CritDamage.FinalValue), true, false);
            }
            return new Damage(MagicAttack.FinalValue, false, false);
        }
    }

    public Damage GetSkillDamage(float skillDmgPercent, bool isPhysic)
    {
        if (ProbabilityCheck.GetThisChanceResult(CritChance.FinalValue))
        {
            return new Damage((Attack.FinalValue * skillDmgPercent) * (1 + CritDamage.FinalValue), true, isPhysic);
        }
        return new Damage(Attack.FinalValue * skillDmgPercent, false, isPhysic);
    }

    public float Defend(float inComingDamage)
    {
        return Mathf.Max(1f, inComingDamage - Defense.FinalValue);
    }

    public void AddMaxHealth(float val)
    {
        HP.AddStatValue(val);
    }

    public void GiveAlias(Alias alias)
    {
        this.alias = alias;
    }
}
