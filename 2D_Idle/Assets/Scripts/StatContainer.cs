using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Stat;
using Litkey.Utility;
using UnityEngine.Events;
using DarkTonic.MasterAudio;
using Litkey.Skill;
using Sirenix.OdinInspector;
using Litkey.InventorySystem;

public class StatContainer : MonoBehaviour
{
    [SerializeField] protected BaseStat baseStat;

    public int MonsterLevel;

    #region Stats
    public MainStat Strength { private set; get; } // �ٷ�
    public MainStat Vit { private set; get; } // ����
    public MainStat Avi { private set; get; } // ��ø
    public MainStat Sensation { private set; get; } // ����
    public MainStat Int { private set; get; } // ����

    public SubStat HP; // ü��
    [ShowInInspector]
    public SubStat Attack { private set; get; } // ���ݷ�
    public SubStat MagicAttack { private set; get; } // ���� ���ݷ�

    public SubStat Defense { private set; get; } // ����
    public SubStat MagicDefense { private set; get; } // ���� ����

    public SubStat AttackSpeed { private set; get; } // ���ݼӵ�
    public SubStat MoveSpeed { private set; get; } // �̵� �ӵ�

    public SubStat CritChance { private set; get; } // ũ��Ƽ�� Ȯ��
    public SubStat CritDamage { private set; get; } // ũ��Ƽ�� ������

    public SubStat Precision { private set; get; } // ����
    public SubStat Evasion { private set; get; } // ȸ��

    public SubStat p_resist { private set; get; } // ���� ���� %
    public SubStat m_resist { private set; get; } // ���� ���� %

    public SubStat p_penetration { private set; get; } // ���� ����� %
    public SubStat m_penetration { private set; get; } // ���� ����� %

    public SubStat Defense_Penetration { private set; get; } // �� �����

    public SubStat ExtraGold; // �߰� ���
    public SubStat ExtraExp; // �߰� ����ġ 

    public SubStat GiveMoreDamage; // �ִ� ��� ������ (���� / ����)
    public SubStat GiveLessDamage;
    public SubStat ReceiveLessDamage; // �޴� ������ (���� / ����)
    public SubStat ReceiveMoreDamage;
    public SubStat ExtraSkillDamage; // ��ų������
    

    #endregion

    [SerializeField] protected Alias alias;

    public int AbilityPoint { get; protected set; }

    public int addedStat
    {
        get
        {
            if (statGiven == null)
            {
                statGiven = new Dictionary<eMainStatType, int>() {
                    { eMainStatType.�ٷ�, 0 },
                    { eMainStatType.����, 0 },
                    { eMainStatType.��ø, 0 },
                    { eMainStatType.����, 0 },
                    { eMainStatType.����, 0 },
                };
            }
            _addedStat = 0;
            foreach (var statplus in statGiven.Values)
            {
                _addedStat += statplus;
            }
            return _addedStat;
        }
    }
    protected int _addedStat = 0;

    public Dictionary<eMainStatType, MainStat> mainStats
    {
        get
        {
            if (_mainStats == null)
            {
                _mainStats = new Dictionary<eMainStatType, MainStat>() {
                    { eMainStatType.�ٷ�, this.Strength },
                    { eMainStatType.����, this.Vit },
                    { eMainStatType.��ø, this.Avi },
                    { eMainStatType.����, this.Sensation },
                    { eMainStatType.����, this.Int },
                };
            }
            return _mainStats;
        }
    }
    protected Dictionary<eMainStatType, MainStat> _mainStats;

    [ShowInInspector]
    public Dictionary<eSubStatType, SubStat> subStats
    {
        get
        {
            return _subStats;
        }
    }
    [ShowInInspector]
    protected Dictionary<eSubStatType, SubStat> _subStats;

    [HideInInspector] public UnityEvent<eMainStatType> OnIncreaseStat = new();
    [HideInInspector] public UnityEvent<eMainStatType, int> OnTryIncreaseStat = new();
    [HideInInspector] public UnityEvent OnApplyStat; // ���� ��� �Ϸ��
    [HideInInspector] public UnityEvent OnCancelStat; // ���� ��� ��ҽ�

    protected Dictionary<eMainStatType, int> statGiven; // �ɷ�ġ ������ ���� ���� ���ݵ�

    public Dictionary<string, List<StatModifier>> passiveStats;
    public List<StatModifier> additionalStats;

    public UnityEvent<StatContainer> OnStatSetupComplete;
    public UnityEvent OnEquipSkill;

    protected virtual void Awake()
    {
        this.MonsterLevel = baseStat.MonsterLevel;
        SetupStats();
        additionalStats = new List<StatModifier>();
        passiveStats = new Dictionary<string, List<StatModifier>>();

        if (statGiven == null)
        {
            statGiven = new Dictionary<eMainStatType, int>() {
                { eMainStatType.�ٷ�, 0 },
                { eMainStatType.����, 0 },
                { eMainStatType.��ø, 0 },
                { eMainStatType.����, 0 },
                { eMainStatType.����, 0 },
            };
        }

        _mainStats = new Dictionary<eMainStatType, MainStat>() {
            { eMainStatType.�ٷ�, this.Strength },
            { eMainStatType.����, this.Vit },
            { eMainStatType.��ø, this.Avi },
            { eMainStatType.����, this.Sensation },
            { eMainStatType.����, this.Int },
        };


        _subStats = new Dictionary<eSubStatType, SubStat>() {
            { eSubStatType.ü��, this.HP },
            { eSubStatType.�������ݷ�, this.Attack },
            { eSubStatType.�������ݷ�, this.MagicAttack },
            { eSubStatType.��������, this.Defense },
            { eSubStatType.��������, this.MagicDefense },
            { eSubStatType.ũ��Ȯ��, this.CritChance },
            { eSubStatType.ũ��������, this.CritDamage },
            { eSubStatType.���ݼӵ�, this.AttackSpeed },
            { eSubStatType.�̵��ӵ�, this.MoveSpeed },
            { eSubStatType.���������, this.p_penetration },
            { eSubStatType.���������, this.m_penetration },
            { eSubStatType.����, this.Precision },
            { eSubStatType.ȸ��, this.Evasion },
            { eSubStatType.��������, this.p_resist },
            { eSubStatType.��������, this.m_resist },
            { eSubStatType.�߰�����ġ, this.ExtraExp },
            { eSubStatType.�߰����, this.ExtraGold },
            { eSubStatType.�������, this.Defense_Penetration },
            { eSubStatType.�ִ���������, this.GiveMoreDamage },
            { eSubStatType.�ִ����ذ���, this.GiveLessDamage },
            { eSubStatType.�޴���������, this.ReceiveMoreDamage },
            { eSubStatType.�޴����ذ���, this.ReceiveLessDamage },
        };

        // Log the initialization of each SubStat
        //foreach (var entry in _subStats)
        //{
        //    Debug.Log($"Initialized _subStats[{entry.Key}] = {entry.Value.DisplayName} in gameobject: " + gameObject.name);
        //}

    }

    private void Start()
    {
        OnStatSetupComplete?.Invoke(this);
    }
    public void ClearMainStats()
    {
        Strength.ClearStat();
        Vit.ClearStat();
        Avi.ClearStat();
        Sensation.ClearStat();
        Int.ClearStat();
    }

    protected void SetupStats()
    {
        Debug.Log("���� �¾� ����");
        Strength = new MainStat("�ٷ�", 0, eMainStatType.�ٷ�);
        Vit = new MainStat("����", 0, eMainStatType.����);
        Avi = new MainStat("��ø", 0, eMainStatType.��ø);
        Sensation = new MainStat("����", 0, eMainStatType.����);
        Int = new MainStat("����", 0, eMainStatType.����);

        HP = new SubStat("ü��", baseStat.MaxHP, eSubStatType.ü��).SetMaxUIValue(1000f);
        Attack = new SubStat("���ݷ�", baseStat.Attack, eSubStatType.�������ݷ�).SetMaxUIValue(100f);
        MagicAttack = new SubStat("���� ���ݷ�", baseStat.MagicAttack, eSubStatType.�������ݷ�).SetMaxUIValue(100f);

        Defense = new SubStat("����", baseStat.Defense, eSubStatType.��������).SetMaxUIValue(100f);
        MagicDefense = new SubStat("���� ����", baseStat.MagicDefense, eSubStatType.��������).SetMaxUIValue(100f);

        AttackSpeed = new SubStat("���ݼӵ�", baseStat.AttackSpeed, eSubStatType.���ݼӵ�, true).SetMaxUIValue(0.3f);
        MoveSpeed = new SubStat("�̵��ӵ�", baseStat.MoveSpeed, eSubStatType.�̵��ӵ�, true).SetMaxUIValue(0.3f);

        CritChance = new SubStat("ũ��Ƽ�� Ȯ��", baseStat.CritChance, eSubStatType.ũ��Ȯ��, true).SetMaxUIValue(1f);
        CritDamage = new SubStat("ũ��Ƽ�� ������", baseStat.CritDamage, eSubStatType.ũ��������, true).SetMaxUIValue(1f);

        ExtraGold = new SubStat("��� �߰�ŉ�淮", baseStat.ExtraGold, eSubStatType.�߰����, true).SetMaxUIValue(1f);
        ExtraExp = new SubStat("����ġ �߰�ŉ�淮", baseStat.ExtraExp, eSubStatType.�߰�����ġ, true).SetMaxUIValue(1f);

        Precision = new SubStat("����", baseStat.Precision, eSubStatType.����).SetMaxUIValue(100f);
        Evasion = new SubStat("ȸ��", baseStat.Evasion, eSubStatType.ȸ��).SetMaxUIValue(100f);

        p_resist = new SubStat("���� ���׷�", baseStat.p_resist, eSubStatType.��������, 0f, 100f).SetMaxUIValue(100f);
        m_resist = new SubStat("���� ���׷�", baseStat.magic_resist, eSubStatType.��������, 0f, 100f).SetMaxUIValue(100f);
        p_penetration = new SubStat("���� �����", baseStat.p_penetration, eSubStatType.���������, 0f, 100f).SetMaxUIValue(100f);
        m_penetration = new SubStat("���� �����", baseStat.magic_penetration, eSubStatType.���������, 0f, 100f).SetMaxUIValue(100f);

        Defense_Penetration = new SubStat("�� �����", baseStat.Defense_Penetration, eSubStatType.�������, true);

        ReceiveMoreDamage = new SubStat("�޴� ���� ����", 0f, eSubStatType.�޴���������, true);
        GiveMoreDamage = new SubStat("�ִ� ���� ����", 0f, eSubStatType.�ִ���������, true);
        ReceiveLessDamage = new SubStat("�޴� ���� ����", 0f, eSubStatType.�޴����ذ���, true);
        GiveLessDamage = new SubStat("�ִ� ���� ����", 0f, eSubStatType.�޴���������, true);


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
        Attack.AddAsInfluencer(StatUtility.StatPerValue(Avi, 1, 3f));

        MagicAttack.AddAsInfluencer(StatUtility.StatPerValue(Int, 1, 15f));

        Defense.AddAsInfluencer(StatUtility.StatPerValue(Vit, 1, 4f));

        MagicDefense.AddAsInfluencer(StatUtility.StatPerValue(Vit, 1, 3f));

        AttackSpeed.AddAsInfluencer(StatUtility.StatPerValue(Avi, 5, 0.01f));

        CritChance.AddAsInfluencer(StatUtility.StatPerValue(Avi, 5, 0.005f));
        CritDamage.AddAsInfluencer(StatUtility.StatPerValue(Sensation, 3, 0.01f));

        //ExtraGold = new SubStat("��� �߰�ŉ�淮", baseStat.ExtraGold, eSubStatType.�߰����, true);
        //ExtraExp = new SubStat("����ġ �߰�ŉ�淮", baseStat.ExtraExp, eSubStatType.�߰�����ġ, true);

        Precision.AddAsInfluencer(StatUtility.StatPerValue(Sensation, 2, 1f));
        Evasion.AddAsInfluencer(StatUtility.StatPerValue(Avi, 5, 1f));
        Debug.Log("���� �¾� �Ϸ�");

    }

    public void ApplyDifficulty(float difficulty)
    {
        HP.SetMultipliedStatValue(difficulty);
        Defense.SetMultipliedStatValue(difficulty);
        Attack.SetMultipliedStatValue(difficulty);

    }

    /// <summary>
    /// ���ν��� ����Ʈ������ �ش� ���꽺���� �� �������� ���� 
    /// </summary>
    /// <param name="subStatType"></param>
    /// <returns></returns>
    public float GetTotalPreviewOf(eSubStatType subStatType)
    {
        float total = 0f;
        var influencers = subStats[subStatType].Influencers;
        for (int i = 0; i < influencers.Count; i++)
        {
            // statGiven[mainStatType] != 0 => 
            var mainStatUsed = statGiven[influencers[i]._mainStat.mainStatType];
            if (mainStatUsed > 0)
            {
                // 
                total += influencers[i].GetPreviewValue(mainStatUsed);
            }
        }
        return total;
    }

    /// <summary>
    /// �� �������� ����Ѵ�
    /// </summary>
    /// <returns></returns>
    public float GetTotalPower(bool onlyBaseStat = false)
    {
        if (onlyBaseStat)
        {
            return baseStat.Attack * 1.5f
                 + baseStat.MaxHP * 0.5f
                + baseStat.Defense * 1.5f
                + baseStat.AttackSpeed * 500f
                + baseStat.Precision * 2f
                + baseStat.Evasion * 3f;
        }

        return Attack.GetFinalValueWithoutBuff() * 1.5f
            + HP.GetFinalValueWithoutBuff() * 0.5f
            + Defense.GetFinalValueWithoutBuff() * 1.5f
            + AttackSpeed.GetFinalValueWithoutBuff() * 500f
            + Precision.GetFinalValueWithoutBuff() * 2f
            + Evasion.GetFinalValueWithoutBuff() * 3f;
    }

    public void SumStatModifier(StatModifier statModifier)
    {
        additionalStats.Add(statModifier);
    }

    public void ClearStatModifiers()
    {
        additionalStats.Clear();
    }

    public void OnEquipPassive(PassiveSkill passive)
    {
        if (!passiveStats.ContainsKey(passive.skillName))
        {
            passiveStats[passive.skillName] = new List<StatModifier>();
        }
        if (passiveStats[passive.skillName].Count > 0)
        {
            RemoveETCStat(passiveStats[passive.skillName]);

        }
        passiveStats[passive.skillName].Clear();
        for (int i = 0; i < passive.AppliedLevelUpgrades.Count; i++)
        {
            passiveStats[passive.skillName].Add(passive.AppliedLevelUpgrades[i]);
        }
        // apply passive rank effects
        //passive.ApplyEffect(this, null);

        passive.EquipPassiveStat(this);
    }

    public void UnEquipStat(Skill skill)
    {

    }

    public void EquipEquipment(EquipmentItem equipmentItem)
    {
        bool isEquipped = false;
        // �ϳ��� �� ����� ���ݵ��� �������ִٸ� 
        foreach (var stat in equipmentItem.EquipmentData.GetStats())
        {
            var subStat = subStats[stat.statType];

            isEquipped = isEquipped || subStat.ContainsEquipmentStat(equipmentItem.ID, stat);
        }
        // ���������ʴ´�
        if (isEquipped) return;

        foreach (var stat in equipmentItem.EquipmentData.GetStats())
        {
            var subStat = subStats[stat.statType];

            subStat.EquipValue(equipmentItem.ID, stat);
        }
    }

    public void UnEquipEquipment(EquipmentItem equipItem)
    {
        var baseStats = equipItem.EquipmentData.GetStats();

        foreach (var stat in baseStats)
        {
            subStats[stat.statType].UnEquipValue(equipItem.ID, stat);
        }
    }

    // ETC ���ݵ��� ���� Equip
    // ��ų ���������� ���� ������ ���ݵ� ����
    public void AddETCStat(List<StatModifier> stats)
    {
        for (int i = 0; i < stats.Count; i++)
        {
            subStats[stats[i].statType].EquipETCStat(stats[i]);
        }
    }

    // ��ų ���������� ���� ������ ���ݵ� ����
    public void RemoveETCStat(List<StatModifier> stats)
    {
        for (int i = 0; i < stats.Count; i++)
        {
            subStats[stats[i].statType].UnEquipETCStat(stats[i]);
        }
    }
    // addedstat = 0, 1, 5
    // 1, 1, 4
    // 2, 1, 3
    // 3, 1, 2
    public void TryAddMainStat(eMainStatType mainStat, int val = 1)
    {
        if (this.addedStat + val > this.AbilityPoint) return;
        if (this.addedStat + val < 0) return;
        if (statGiven[mainStat] + val < 0) return;
        statGiven[mainStat] += val;

        OnTryIncreaseStat?.Invoke(mainStat, statGiven[mainStat]);
    }

    public void ApplyStat()
    {
        if (addedStat <= 0) return;
        foreach (var stat in mainStats.Keys)
        {
            var increaseStat = statGiven[stat];
            if (increaseStat == 0) continue;
            mainStats[stat].IncreaseStat(increaseStat);
            OnIncreaseStat?.Invoke(stat);
        }
        MasterAudio.PlaySound("�층");
        this.AbilityPoint -= this.addedStat;
        ClearStatGivenPoints();
        OnApplyStat?.Invoke();
        OnStatSetupComplete?.Invoke(this);
    }

    // ������� �ʱ�ȭ
    protected void ClearStatGivenPoints()
    {
        foreach (var stat in mainStats.Keys)
        {
            statGiven[stat] = 0;
        }
    }

    public void CancelStatChange()
    {
        foreach (var stat in mainStats.Keys)
        {
            statGiven[stat] = 0;
        }
        OnCancelStat?.Invoke();
    }

    #region Damage 
    // ü�� - (�� ���ݷ� - ���� (>=0) ) * (100 - (�Ʊ� �������� - �� ��������� >= 0) / 100 or (�Ʊ��������� - �� ���������) / 100)
    // => ��ü�� - ((�ư� - ����) * (100 - (�������� - �Ʊ������ / 100)) 
    // ������ * ( 
    // �����ڰ� �θ�,
    public Damage GetDamageAgainst(StatContainer enemyStat, float multiplier = 1f)
    {

        float dmg;
        var m_AttackVal = GetFinalDamage(enemyStat, multiplier);

        if (m_AttackVal.isPhysicalDmg)
        {
            float attackDmg = (m_AttackVal.damage * (1f + (p_penetration.FinalValue - enemyStat.p_resist.FinalValue)))
                - (enemyStat.Defense.FinalValue * 1f + (enemyStat.p_resist.FinalValue - p_penetration.FinalValue));

            dmg = GetRandomExtentDamage(attackDmg);
            dmg = (Mathf.Clamp(dmg, 1f, float.MaxValue));

        }
        else
        {
            // magic dmg
            float attackDmg = (m_AttackVal.damage * (1f + (m_penetration.FinalValue - enemyStat.m_resist.FinalValue)))
                - (enemyStat.MagicDefense.FinalValue * 1f + (enemyStat.m_resist.FinalValue - m_penetration.FinalValue));

            dmg = GetRandomExtentDamage(attackDmg);
            dmg = Mathf.Clamp(dmg, 1f, float.MaxValue);
        }
        return new Damage(dmg, m_AttackVal.isCrit, m_AttackVal.isPhysicalDmg);
    }
    // ally ���� �Ҹ�
    public List<Damage> GetDamagesAgainst(StatContainer enemyStat, int damageCount, float multiplier = 1f)
    {
        List<Damage> damages = new List<Damage>();
        for (int i = 0; i < damageCount; i++)
        {
            float dmg;
            var m_AttackVal = GetFinalDamage(enemyStat, multiplier);

            if (m_AttackVal.isPhysicalDmg)
            {
                float attackDmg = m_AttackVal.damage
                    * (1f + GiveMoreDamage.FinalValue - GiveLessDamage.FinalValue)
                    * (1f + enemyStat.ReceiveMoreDamage.FinalValue - enemyStat.ReceiveLessDamage.FinalValue);


                dmg = GetRandomExtentDamage(attackDmg);
                dmg = (Mathf.Clamp(dmg, 1f, float.MaxValue));
            }
            else
            {
                // magic dmg
                float attackDmg = m_AttackVal.damage
                     * (1f + GiveMoreDamage.FinalValue - GiveLessDamage.FinalValue)
                     * (1f + enemyStat.ReceiveMoreDamage.FinalValue - enemyStat.ReceiveLessDamage.FinalValue);

                dmg = GetRandomExtentDamage(attackDmg);
                dmg = (Mathf.Clamp(dmg, 1f, float.MaxValue));
            }
            damages.Add(new Damage(dmg, m_AttackVal.isCrit, m_AttackVal.isPhysicalDmg));

        }
        return damages;
    }

    /// <summary>
    ///  ���� ���ݷ��� �������� ġ��Ÿ�� ġ��Ÿ�������� �����´�
    /// </summary>
    /// <param name="multiplier"></param>
    /// <returns></returns>    
    private Damage GetFinalDamage(StatContainer enemyStat, float multiplier = 1f)
    {
        bool isPhysic = Attack.FinalValue >= MagicAttack.FinalValue;
        if (isPhysic)
        {
            var baseDMG = (Attack.FinalValue - (enemyStat.Defense.FinalValue * (1f - Defense_Penetration.FinalValue)))
                * (1 - Mathf.Max(0f, Mathf.Min(enemyStat.p_resist.FinalValue - p_penetration.FinalValue, 0.8f)));

            if (ProbabilityCheck.GetThisChanceResult(CritChance.FinalValue))
            {
                return new Damage(baseDMG  * multiplier * (1 + CritDamage.FinalValue), true, isPhysic);
            }
            return new Damage(baseDMG * multiplier, false, isPhysic);
        }
        else
        {
            var baseDMG = (MagicAttack.FinalValue - (enemyStat.MagicDefense.FinalValue * (1f - Defense_Penetration.FinalValue)))
                * (1 - Mathf.Max(0f, Mathf.Min(enemyStat.m_resist.FinalValue - m_penetration.FinalValue, 0.8f)));

            if (ProbabilityCheck.GetThisChanceResult(CritChance.FinalValue))
            {
                return new Damage(baseDMG * multiplier * (1 + CritDamage.FinalValue), true, false);
            }
            return new Damage(baseDMG * multiplier, false, false);
        }
    }

    // ��ų���� ��������
    public Damage GetSkillDamage(float skillDmgPercent, bool isPhysic)
    {
        if (ProbabilityCheck.GetThisChanceResult(CritChance.FinalValue))
        {
            return new Damage((Attack.FinalValue * skillDmgPercent) * (1 + CritDamage.FinalValue), true, isPhysic);
        }
        return new Damage(Attack.FinalValue * skillDmgPercent, false, isPhysic);
    }

    // ���������θ�
    // ���� = ���� 
    // ���߷� = ���� - �� ȸ��
    // True = ���߼��� 
    public bool CalculateHit(StatContainer enemyStat)
    {
        float hitChance = (Precision.FinalValue - enemyStat.Evasion.FinalValue) / (Precision.FinalValue + enemyStat.Evasion.FinalValue);
        // 5/15 = .333 /.

        if (TryGetComponent<LevelSystem>(out LevelSystem levelSystem))
        {
            // �� ������ ������ �� ȸ��ġ�� �ö�
            // �Ʊ������� ������ �ƹ��ϵ� ����
            // 5������ �� ȸ�� 5% �ö�
            int allyLevel = levelSystem.GetLevel();
            if (enemyStat.MonsterLevel > allyLevel)
            {
                int quotient = (enemyStat.MonsterLevel - allyLevel) / 5;
                hitChance -= 0.05f * quotient;
            } else if (allyLevel > enemyStat.MonsterLevel)
            {
                int quotient = (allyLevel - enemyStat.MonsterLevel) / 5;
                hitChance += 0.05f * quotient;
            }
        }
        Debug.Log("Hitchance: " + hitChance);
        hitChance = Mathf.Clamp(hitChance, 0.05f, 1f);
        return ProbabilityCheck.GetThisChanceResult(hitChance);
    }


    public float GetRandomExtentDamage(float dmg)
    {
        return Random.Range(dmg * 0.97f, dmg * 1.03f);
    }

    #endregion

    public void AddMaxHealth(float val)
    {
        HP.AddStatValue(val);
    }

    public void GiveAlias(Alias alias)
    {
        this.alias = alias;
    }

    public void RemoveAlias(Alias alias)
    {
        if (this.alias == null) return;

        if (alias.aliasName != this.alias.aliasName) return;
        
        foreach (var buffStat in alias.extraStats)
        {
            
            subStats[buffStat.statType].AddBuffValue(buffStat);
            
        }
    }
    public BaseStat GetBaseStat()
    {
        return this.baseStat;
    }

    public List<BuffInfo> appliedBuffs = new List<BuffInfo>();
    public UnityEvent<BuffInfo> OnAddBuff = new UnityEvent<BuffInfo>();
    public UnityEvent<BuffInfo> OnUpdateBuff = new UnityEvent<BuffInfo>();
    public UnityEvent<BuffInfo> OnRemoveBuff = new UnityEvent<BuffInfo>();
    private Dictionary<int, List<Coroutine>> buffTimers = new Dictionary<int, List<Coroutine>>();

    public void ApplyBuff(Buff buff, int buffStack = 1)
    {
        BuffInfo existingBuff = appliedBuffs.Find(b => b.buff.BuffID == buff.BuffID);

        if (existingBuff != null)
        {
            if (buff.Stackable)
            {
                int increasedStack = existingBuff.IncreaseStack(buffStack);
                if (increasedStack > 0)
                {
                    Debug.Log("Player buff increased Stack: " + increasedStack);
                    ApplyBuffEffect(existingBuff, increasedStack);
                    OnUpdateBuff?.Invoke(existingBuff);
                }
            }
            else
            {
                //RemoveBuffEffect(existingBuff, existingBuff.stackCount);
                //existingBuff.stackCount = buffStack;
                //ApplyBuffEffect(existingBuff, buffStack);
                Debug.Log($"Refreshing non-stackable buff: {buff.BuffName}");
            }
        }
        else
        {
            existingBuff = new BuffInfo(buff, buffStack);
            appliedBuffs.Add(existingBuff);
            ApplyBuffEffect(existingBuff, buffStack);
            OnAddBuff?.Invoke(existingBuff);
        }

        // Apply timer for non-permanent buffs
        if (!buff.PermanentDuration)
        {
            UpdateBuffTimer(existingBuff);
        }

        OnUpdateBuff?.Invoke(existingBuff);
    }

    private void ApplySingleBuffStack(BuffInfo buffInfo)
    {
        buffInfo.stackCount++;
        ApplyBuffEffect(buffInfo, 1);

        if (!buffInfo.buff.PermanentDuration)
        {
            if (!buffTimers.ContainsKey(buffInfo.buff.BuffID))
            {
                buffTimers[buffInfo.buff.BuffID] = new List<Coroutine>();
            }
            buffTimers[buffInfo.buff.BuffID].Add(StartCoroutine(BuffTimer(buffInfo)));
        }

        OnUpdateBuff?.Invoke(buffInfo);
    }

    private void ApplyBuffEffect(BuffInfo buffInfo, int stacks)
    {
        foreach (var buffStat in buffInfo.buff.BuffStats)
        {
            for (int i = 0; i < stacks; i++)
            {
                subStats[buffStat.statType].AddBuffValue(buffStat);
            }
        }
    }

    private void RemoveBuffEffect(BuffInfo buffInfo, int stacks)
    {
        foreach (var buffStat in buffInfo.buff.BuffStats)
        {
            for (int i = 0; i < stacks; i++)
            {
                subStats[buffStat.statType].RemoveBuffValue(buffStat);
            }
        }
    }

    private void RemoveSingleBuffStack(BuffInfo buffInfo)
    {
        if (buffInfo.stackCount > 0)
        {
            buffInfo.stackCount--;
            RemoveBuffEffect(buffInfo, 1);

            OnUpdateBuff?.Invoke(buffInfo);

            if (buffInfo.stackCount == 0)
            {
                appliedBuffs.Remove(buffInfo);
                OnRemoveBuff?.Invoke(buffInfo);
            }
        }
    }


    private IEnumerator BuffTimer(BuffInfo buffInfo)
    {
        yield return new WaitForSeconds(buffInfo.buff.Duration);
        RemoveSingleBuffStack(buffInfo);

        if (buffTimers.ContainsKey(buffInfo.buff.BuffID))
        {
            buffTimers[buffInfo.buff.BuffID].RemoveAt(0);
            if (buffTimers[buffInfo.buff.BuffID].Count == 0)
            {
                buffTimers.Remove(buffInfo.buff.BuffID);
            }
        }
    }

    private void UpdateBuffTimer(BuffInfo buffInfo)
    {
        if (buffTimers.ContainsKey(buffInfo.buff.BuffID))
        {
            // Stop all existing coroutines for this buff
            foreach (var coroutine in buffTimers[buffInfo.buff.BuffID])
            {
                StopCoroutine(coroutine);
            }

            // Clear the list of coroutines
            buffTimers[buffInfo.buff.BuffID].Clear();
        }
        else
        {
            // If the buff doesn't have an entry in buffTimers, create one
            buffTimers[buffInfo.buff.BuffID] = new List<Coroutine>();
        }

        // Start new coroutines for each stack of the buff
        for (int i = 0; i < buffInfo.stackCount; i++)
        {
            Coroutine newCoroutine = StartCoroutine(BuffTimer(buffInfo));
            buffTimers[buffInfo.buff.BuffID].Add(newCoroutine);
        }
    }

    public int RemoveBuff(Buff buff)
    {
        BuffInfo buffToRemove = appliedBuffs.Find(b => b.buff.BuffID == buff.BuffID);
        if (buffToRemove != null)
        {
            int stacksToRemove = buffToRemove.stackCount;
            for (int i = 0; i < stacksToRemove; i++)
            {
                RemoveSingleBuffStack(buffToRemove);
            }

            if (buffTimers.ContainsKey(buff.BuffID))
            {
                foreach (var coroutine in buffTimers[buff.BuffID])
                {
                    StopCoroutine(coroutine);
                }
                buffTimers.Remove(buff.BuffID);
            }
            return stacksToRemove;
        }
        return 0;
    }

    public bool HasBuff(Buff buff, int stackCount)
    {
        BuffInfo existingBuff = appliedBuffs.Find(b => b.buff.BuffID == buff.BuffID);
        if (existingBuff != null && existingBuff.stackCount >= stackCount)
        {
            return true;
        }
        return false;
    }

    public bool UseBuff(Buff buff, int stackCount)
    {
        BuffInfo existingBuff = appliedBuffs.Find(b => b.buff.BuffID == buff.BuffID);
        if (existingBuff != null && existingBuff.stackCount >= stackCount)
        {
            int removedStacks = RemoveBuff(buff, stackCount);
            return removedStacks > 0;
        }
        return false;
    }
    public int RemoveBuff(Buff buff, int removeStackCount)
    {
        BuffInfo buffToRemove = appliedBuffs.Find(b => b.buff.BuffID == buff.BuffID);
        if (buffToRemove != null)
        {
            int stacksToRemove = Mathf.Clamp(removeStackCount, 0, buffToRemove.stackCount);

            RemoveBuffEffect(buffToRemove, stacksToRemove);
            buffToRemove.stackCount -= stacksToRemove;

            if (buffTimers.ContainsKey(buff.BuffID))
            {
                for (int i = 0; i < stacksToRemove && buffTimers[buff.BuffID].Count > 0; i++)
                {
                    StopCoroutine(buffTimers[buff.BuffID][0]);
                    buffTimers[buff.BuffID].RemoveAt(0);
                }

                if (buffTimers[buff.BuffID].Count == 0)
                {
                    buffTimers.Remove(buff.BuffID);
                }
            }

            if (buffToRemove.stackCount <= 0)
            {
                appliedBuffs.Remove(buffToRemove);
                OnRemoveBuff?.Invoke(buffToRemove);
            }
            else
            {
                OnUpdateBuff?.Invoke(buffToRemove);
            }

            return stacksToRemove;
        }
        return 0;
    }

    #region AllStats

    private Dictionary<string, List<StatModifier>> allStatModifiers = new Dictionary<string, List<StatModifier>>();

    public void ApplyAllStatModifier(string modifierID, float value, OperatorType operatorType)
    {
        if (!allStatModifiers.ContainsKey(modifierID))
        {
            allStatModifiers[modifierID] = new List<StatModifier>();
        }

        foreach (var mainStat in mainStats.Values)
        {
            foreach (var subStat in mainStat.ChildSubstats)
            {
                StatModifier modifier = new StatModifier
                {
                    statType = subStat.statType,
                    oper = operatorType,
                    value = value
                };

                allStatModifiers[modifierID].Add(modifier);

                ApplyModifierToSubStat(subStat, modifier);
            }
        }

        OnStatSetupComplete?.Invoke(this);
    }

    private void ApplyModifierToSubStat(SubStat subStat, StatModifier modifier)
    {
        switch (modifier.oper)
        {
            case OperatorType.plus:
            case OperatorType.subtract:
                float modValue = modifier.oper == OperatorType.plus ? modifier.value : -modifier.value;
                subStat.AddStatValue(modValue);
                break;
        }
    }

    public void RemoveAllStatModifier(string modifierID)
    {
        if (allStatModifiers.TryGetValue(modifierID, out List<StatModifier> modifiers))
        {
            foreach (var modifier in modifiers)
            {
                SubStat subStat = GetSubStatByType(modifier.statType);
                if (subStat != null)
                {
                    RemoveModifierFromSubStat(subStat, modifier);
                }
            }

            allStatModifiers.Remove(modifierID);
            OnStatSetupComplete?.Invoke(this);
        }
    }

    private void RemoveModifierFromSubStat(SubStat subStat, StatModifier modifier)
    {
        switch (modifier.oper)
        {
            case OperatorType.plus:
                subStat.AddStatValue(-modifier.value);
                break;
            case OperatorType.subtract:
                subStat.AddStatValue(modifier.value);
                break;
            case OperatorType.multiply:
            case OperatorType.divide:
                subStat.UnEquipETCStat(modifier);
                break;
        }
    }

    private SubStat GetSubStatByType(eSubStatType statType)
    {
        return subStats.TryGetValue(statType, out SubStat subStat) ? subStat : null;
    }

    #endregion

}
