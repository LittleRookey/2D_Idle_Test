using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Stat;
using Litkey.Utility;
using UnityEngine.Events;
using DarkTonic.MasterAudio;
using Litkey.Skill;

public class StatContainer : MonoBehaviour
{
    [SerializeField] protected BaseStat baseStat;

    public int MonsterLevel;

    #region Stats
    public MainStat Strength { private set; get; } // 근력
    public MainStat Vit { private set; get; } // 맷집
    public MainStat Avi { private set; get; } // 민첩
    public MainStat Sensation { private set; get; } // 감각
    public MainStat Int { private set; get; } // 지혜

    public SubStat HP; // 체력

    public SubStat Attack { private set; get; } // 공격력
    public SubStat MagicAttack { private set; get; } // 마법 공격력

    public SubStat Defense { private set; get; } // 방어력
    public SubStat MagicDefense { private set; get; } // 마법 방어력

    public SubStat AttackSpeed { private set; get; } // 공격속도
    public SubStat MoveSpeed { private set; get; } // 이동 속도

    public SubStat CritChance { private set; get; } // 크리티컬 확률
    public SubStat CritDamage { private set; get; } // 크리티컬 데미지

    public SubStat Precision { private set; get; } // 명중
    public SubStat Evasion { private set; get; } // 회피

    public SubStat p_resist { private set; get; } // 물리 저항 %
    public SubStat m_resist { private set; get; } // 마법 저항 %

    public SubStat p_penetration { private set; get; } // 물리 관통력 %
    public SubStat m_penetration { private set; get; } // 마법 관통력 %

    // 특별 스텟
    //public SubStat receiveAdditionalDamage; // 받는 피해 증가
    //public SubStat giveAdditionalDamage; // 주는 피해 증가

    //public SubStat receiveLessDamage; // 받는 피해 감소
    //public SubStat giveLessDamage; // 주는 피해 감소

    public SubStat ExtraGold; // 추가 골드
    public SubStat ExtraExp; // 추가 경험치 
    #endregion

    //public UnitLevel unitLevel;
    [SerializeField] protected Alias alias;

    public int AbilityPoint { get; protected set; }

    public int addedStat { 
        get
        {
            if (statGiven == null)
            {
                statGiven = new Dictionary<eMainStatType, int>() {
                    { eMainStatType.근력, 0 },
                    { eMainStatType.맷집, 0 },
                    { eMainStatType.민첩, 0 },
                    { eMainStatType.감각, 0 },
                    { eMainStatType.지혜, 0 },
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

    // 서브 스텟 값은 최종적으로 메인스텟 + 베이스 스텟 + 이명 스텟(성격)을 합한 값이 될것이다
    public Dictionary<eMainStatType, MainStat> mainStats
    {
        get
        {
            if (_mainStats == null)
            {
                _mainStats = new Dictionary<eMainStatType, MainStat>() {
                    { eMainStatType.근력, this.Strength },
                    { eMainStatType.맷집, this.Vit },
                    { eMainStatType.민첩, this.Avi },
                    { eMainStatType.감각, this.Sensation },
                    { eMainStatType.지혜, this.Int },
                };
            }
            return _mainStats;
        }
    }
    protected Dictionary<eMainStatType, MainStat> _mainStats;

    public Dictionary<eSubStatType, SubStat> subStats
    {
        get
        {
            if (_subStats == null || _subStats.Count == 0)
            {
                _subStats = new Dictionary<eSubStatType, SubStat>() {
                    { eSubStatType.체력, this.HP },
                    { eSubStatType.물리공격력, this.Attack },
                    { eSubStatType.마법공격력, this.MagicAttack },
                    { eSubStatType.물리방어력, this.Defense },
                    { eSubStatType.마법방어력, this.MagicDefense },
                    //{ eSubStatType.attackRange, this.},
                    //{ eSubStatType.cc_Resistance, this. },
                    { eSubStatType.크리확률, this.CritChance },
                    { eSubStatType.크리데미지, this.CritDamage },
                    { eSubStatType.공격속도, this.AttackSpeed },
                    //{ eSubStatType.healthRegen, this. },
                    //{ eSubStatType.mana, this. },
                    //{ eSubStatType.manaRegen, this. },
                    { eSubStatType.이동속도, this.MoveSpeed },
                    { eSubStatType.물리관통력, this.p_penetration },
                    { eSubStatType.마법관통력, this.m_penetration },
                    { eSubStatType.명중, this.Precision },
                    { eSubStatType.회피, this.Evasion },
                    { eSubStatType.물리저항, this.p_resist },
                    { eSubStatType.마법저항, this.m_resist },
                    //{ eSubStatType.받는피해감소, this. },
                    //{ eSubStatType.받는피해증가, this.Int },
                    //{ eSubStatType.주는피해감소, this.Int },
                    //{ eSubStatType.주는피해증가, this.Int },
                    { eSubStatType.추가경험치, this.ExtraExp },
                    { eSubStatType.추가골드, this.ExtraGold },
                };
            }
            return _subStats;
        }
    }
    protected Dictionary<eSubStatType, SubStat> _subStats;


    [HideInInspector] public UnityEvent<eMainStatType> OnIncreaseStat = new();
    [HideInInspector] public UnityEvent<eMainStatType, int> OnTryIncreaseStat = new();
    [HideInInspector] public UnityEvent OnApplyStat; // 스텟 찍기 완료시
    [HideInInspector] public UnityEvent OnCancelStat; // 스텟 찍기 취소시

    protected Dictionary<eMainStatType, int> statGiven; // 능력치 찍을떄 현재 찍은 스텟들

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
                { eMainStatType.근력, 0 },
                { eMainStatType.맷집, 0 },
                { eMainStatType.민첩, 0 },
                { eMainStatType.감각, 0 },
                { eMainStatType.지혜, 0 },
            };
        }


        _mainStats = new Dictionary<eMainStatType, MainStat>() {
            { eMainStatType.근력, this.Strength },
            { eMainStatType.맷집, this.Vit },
            { eMainStatType.민첩, this.Avi },
            { eMainStatType.감각, this.Sensation },
            { eMainStatType.지혜, this.Int },
        };

        if (_subStats == null)
        {
            _subStats = new Dictionary<eSubStatType, SubStat>() {
                    { eSubStatType.체력, this.HP },
                    { eSubStatType.물리공격력, this.Attack },
                    { eSubStatType.마법공격력, this.MagicAttack },
                    { eSubStatType.물리방어력, this.Defense },
                    { eSubStatType.마법방어력, this.MagicDefense },
                    //{ eSubStatType.attackRange, this.},
                    //{ eSubStatType.cc_Resistance, this. },
                    { eSubStatType.크리확률, this.CritChance },
                    { eSubStatType.크리데미지, this.CritDamage },
                    { eSubStatType.공격속도, this.AttackSpeed },
                    //{ eSubStatType.healthRegen, this. },
                    //{ eSubStatType.mana, this. },
                    //{ eSubStatType.manaRegen, this. },
                    { eSubStatType.이동속도, this.MoveSpeed },
                    { eSubStatType.물리관통력, this.p_penetration },
                    { eSubStatType.마법관통력, this.m_penetration },
                    { eSubStatType.명중, this.Precision },
                    { eSubStatType.회피, this.Evasion },
                    { eSubStatType.물리저항, this.p_resist },
                    { eSubStatType.마법저항, this.m_resist },
                    //{ eSubStatType.받는피해감소, this. },
                    //{ eSubStatType.받는피해증가, this.Int },
                    //{ eSubStatType.주는피해감소, this.Int },
                    //{ eSubStatType.주는피해증가, this.Int },
                    { eSubStatType.추가경험치, this.ExtraExp },
                    { eSubStatType.추가골드, this.ExtraGold },
                };
        }
        
        //Evasion = new SubStat("회피", baseStat.Evasion, eSubStatType.회피);

        //receiveAdditionalDamage = new SubStat("받는 피해 증가", 0f, eSubStatType.받는피해증가, true);
        //giveAdditionalDamage = new SubStat("주는 피해 증가", 0f, eSubStatType.주는피해증가, true);
        //receiveLessDamage = new SubStat("받는 피해 감소", 0f, eSubStatType.받는피해감소, true);
        //giveLessDamage = new SubStat("주는 피해 감소", 0f, eSubStatType.받는피해증가, true);

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
        Debug.Log("스텟 셋업 시작");
        Strength = new MainStat("근력", 0, eMainStatType.근력);
        Vit = new MainStat("맷집", 0, eMainStatType.맷집);
        Avi = new MainStat("민첩", 0, eMainStatType.민첩);
        Sensation = new MainStat("감각", 0, eMainStatType.감각);
        Int = new MainStat("지혜", 0, eMainStatType.지혜);

        HP = new SubStat("체력", baseStat.MaxHP, eSubStatType.체력).SetMaxUIValue(1000f);
        Attack = new SubStat("공격력", baseStat.Attack, eSubStatType.물리공격력).SetMaxUIValue(100f);
        MagicAttack = new SubStat("마법 공격력", baseStat.MagicAttack, eSubStatType.마법공격력).SetMaxUIValue(100f);

        Defense = new SubStat("방어력", baseStat.Defense, eSubStatType.물리방어력).SetMaxUIValue(100f);
        MagicDefense = new SubStat("마법 방어력", baseStat.MagicDefense, eSubStatType.마법방어력).SetMaxUIValue(100f);

        AttackSpeed = new SubStat("공격속도", baseStat.AttackSpeed, eSubStatType.공격속도, true).SetMaxUIValue(0.3f);
        MoveSpeed = new SubStat("이동속도", baseStat.MoveSpeed, eSubStatType.이동속도, true).SetMaxUIValue(0.3f);

        CritChance = new SubStat("크리티컬 확률", baseStat.CritChance, eSubStatType.크리확률, true).SetMaxUIValue(1f);
        CritDamage = new SubStat("크리티컬 데미지", baseStat.CritDamage, eSubStatType.크리데미지, true).SetMaxUIValue(1f);

        ExtraGold = new SubStat("골드 추가흭득량", baseStat.ExtraGold, eSubStatType.추가골드, true).SetMaxUIValue(1f);
        ExtraExp = new SubStat("경험치 추가흭득량", baseStat.ExtraExp, eSubStatType.추가경험치, true).SetMaxUIValue(1f);

        Precision = new SubStat("명중", baseStat.Precision, eSubStatType.명중).SetMaxUIValue(100f);
        Evasion = new SubStat("회피", baseStat.Evasion, eSubStatType.회피).SetMaxUIValue(100f);

        p_resist = new SubStat("물리 저항력", baseStat.p_resist, eSubStatType.물리저항, 0f, 100f).SetMaxUIValue(100f);
        m_resist = new SubStat("마법 저항력", baseStat.magic_resist, eSubStatType.마법저항, 0f, 100f).SetMaxUIValue(100f);
        p_penetration = new SubStat("물리 관통력", baseStat.p_penetration, eSubStatType.물리관통력, 0f, 100f).SetMaxUIValue(100f);
        m_penetration = new SubStat("마법 관통력", baseStat.magic_penetration, eSubStatType.마법관통력, 0f, 100f).SetMaxUIValue(100f);

        //receiveAdditionalDamage = new SubStat("받는 피해 증가", 0f, eSubStatType.받는피해증가, true);
        //giveAdditionalDamage = new SubStat("주는 피해 증가", 0f, eSubStatType.주는피해증가, true);
        //receiveLessDamage = new SubStat("받는 피해 감소", 0f, eSubStatType.받는피해감소, true);
        //giveLessDamage = new SubStat("주는 피해 감소", 0f, eSubStatType.받는피해증가, true);


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

        // 서브스텟에서 메인스텟과 관계맺기
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

        //ExtraGold = new SubStat("골드 추가흭득량", baseStat.ExtraGold, eSubStatType.추가골드, true);
        //ExtraExp = new SubStat("경험치 추가흭득량", baseStat.ExtraExp, eSubStatType.추가경험치, true);

        Precision.AddAsInfluencer(StatUtility.StatPerValue(Sensation, 2, 1f));
        Evasion.AddAsInfluencer(StatUtility.StatPerValue(Avi, 5, 1f));
        Debug.Log("스텟 셋업 완료");

        
    }

    /// <summary>
    /// 메인스텟 포인트찍힌후 해당 서브스텟의 총 증가값을 리턴 
    /// </summary>
    /// <param name="subStatType"></param>
    /// <returns></returns>
    public float GetTotalPreviewOf(eSubStatType subStatType)
    {
        float total = 0f;
        var influencers = subStats[subStatType].Influencers;
        for (int i = 0; i< influencers.Count; i++)
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
    

    public void UnEquipStat(PassiveSkill skill)
    {

    }
    // addedstat = 0, 1, 5
    // 1, 1, 4
    // 2, 1, 3
    // 3, 1, 2
    public void TryAddMainStat(eMainStatType mainStat, int val=1)
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
        MasterAudio.PlaySound("띠링");
        this.AbilityPoint -= this.addedStat;
        ClearStatGivenPoints();
        OnApplyStat?.Invoke();
        OnStatSetupComplete?.Invoke(this);
    }

    // 스텟찍기 초기화
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
    // 체력 - (적 공격력 - 방어력 (>=0) ) * (100 - (아군 물리저항 - 적 물리관통력 >= 0) / 100 or (아군마법저항 - 적 마법관통력) / 100)
    // => 적체력 - ((아공 - 적방) * (100 - (적군저항 - 아군관통력 / 100)) 
    // 데미지 * ( 
    // 공격자가 부름,
    public Damage GetDamageAgainst(StatContainer enemyStat, float multiplier=1f)
    {

        float dmg;
        var m_AttackVal = GetFinalDamage(multiplier);

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

    public List<Damage> GetDamagesAgainst(StatContainer enemyStat, int damageCount, float multiplier = 1f)
    {
        List<Damage> damages = new List<Damage>();
        for (int i = 0; i < damageCount; i++)
        {
            float dmg;
            var m_AttackVal = GetFinalDamage(multiplier);

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
                //float attackDmg = m_AttackVal.damage - enemyStat.MagicDefense.FinalValue;
                //dmg = (Mathf.Clamp(attackDmg, 1f, 999999999) * (1f + (m_penetration.FinalValue - enemyStat.m_resist.FinalValue) / 100f));
                float attackDmg = (m_AttackVal.damage * (1f + (m_penetration.FinalValue - enemyStat.m_resist.FinalValue)))
                    - (enemyStat.MagicDefense.FinalValue * 1f + (enemyStat.m_resist.FinalValue - m_penetration.FinalValue));

                dmg = GetRandomExtentDamage(attackDmg);
                dmg = (Mathf.Clamp(dmg, 1f, float.MaxValue));
            }
            damages.Add(new Damage(dmg, m_AttackVal.isCrit, m_AttackVal.isPhysicalDmg));
            
        }
        return damages;
    }

    /// <summary>
    ///  현재 공격력을 가져오고 치명타면 치명타데미지를 가져온다
    /// </summary>
    /// <param name="multiplier"></param>
    /// <returns></returns>    
    private Damage GetFinalDamage(float multiplier=1f)
    {
        bool isPhysic = Attack.FinalValue >= MagicAttack.FinalValue;
        if (isPhysic)
        {
            if (ProbabilityCheck.GetThisChanceResult(CritChance.FinalValue))
            {
                return new Damage(Attack.FinalValue * multiplier * (1 + CritDamage.FinalValue), true, isPhysic);
            }
            return new Damage(Attack.FinalValue * multiplier, false, isPhysic);
        }
        else
        {
            if (ProbabilityCheck.GetThisChanceResult(CritChance.FinalValue))
            {
                return new Damage(MagicAttack.FinalValue * multiplier * (1 + CritDamage.FinalValue), true, false);
            }
            return new Damage(MagicAttack.FinalValue * multiplier, false, false);
        }
    }
    
    // 스킬쓸때 데미지용
    public Damage GetSkillDamage(float skillDmgPercent, bool isPhysic)
    {
        if (ProbabilityCheck.GetThisChanceResult(CritChance.FinalValue))
        {
            return new Damage((Attack.FinalValue * skillDmgPercent) * (1 + CritDamage.FinalValue), true, isPhysic);
        }
        return new Damage(Attack.FinalValue * skillDmgPercent, false, isPhysic);
    }

    // 공격저가부름
    // 명중 = 명중 
    // 명중률 = 명중 - 적 회피
    // True = 명중성공 
    public bool CalculateHit(StatContainer enemyStat)
    {
        float hitChance = (Precision.FinalValue - enemyStat.Evasion.FinalValue) / (Precision.FinalValue + enemyStat.Evasion.FinalValue);
        // 5/15 = .333 /.
        
        if (TryGetComponent<LevelSystem>(out LevelSystem levelSystem))
        {
            // 적 레벨이 높으면 적 회피치가 올라감
            // 아군레벨이 높으면 아무일도 없음
            // 5레벨당 적 회피 5% 올라감
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

    public BaseStat GetBaseStat()
    {
        return this.baseStat;
    }

    public void AddBuffEffect(StatModifier statModifier)
    {
        // TODO
    }

    // ETC 스텟들을 전부 Equip
    // 스킬 레벨업으로 인해 증가된 스텟들 장착
    public void AddETCStat(List<StatModifier> stats)
    {
        for (int i = 0; i < stats.Count; i++)
        {
            subStats[stats[i].statType].EquipETCStat(stats[i]);
        }
    }

    // 스킬 레벨업으로 인해 증가된 스텟들 해제
    public void RemoveETCStat(List<StatModifier> stats)
    {
        for (int i = 0; i < stats.Count; i++)
        {
            subStats[stats[i].statType].UnEquipETCStat(stats[i]);
        }
    }
}
