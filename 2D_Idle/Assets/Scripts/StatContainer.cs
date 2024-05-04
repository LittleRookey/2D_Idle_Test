using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Stat;
using Litkey.Utility;
using UnityEngine.Events;

public class StatContainer : MonoBehaviour
{
    [SerializeField] private BaseStat baseStat;

    public int MonsterLevel;

    public MainStat Strength { private set; get; } // 근력
    public MainStat Vit { private set; get; } // 맷집
    public MainStat Avi { private set; get; } // 민첩
    public MainStat Sensation { private set; get; } // 감각
    public MainStat Int { private set; get; } // 지혜

    public SubStat HP { private set; get; } // 체력

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

    //public UnitLevel unitLevel;
    [SerializeField] private Alias alias;

    public int AbilityPoint { get; private set; }

    public int addedStat { private set; get; } = 0;
    // 서브 스텟 값은 최종적으로 메인스텟 + 베이스 스텟 + 이명 스텟(성격)을 합한 값이 될것이다
    public Dictionary<eMainStatType, MainStat> mainStats
    {
        get
        {
            if (_mainStats == null || _mainStats.Count == 0)
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
    private Dictionary<eMainStatType, MainStat> _mainStats;

    public UnityEvent<eMainStatType> OnIncreaseStat = new();
    public UnityEvent<eMainStatType, int> OnTryIncreaseStat = new();

    private Dictionary<eMainStatType, int> statGiven;
    private void Awake()
    {
        this.MonsterLevel = baseStat.MonsterLevel;
        SetupStats();

        this.AbilityPoint = 5;

        statGiven = new Dictionary<eMainStatType, int>() {
            { eMainStatType.근력, 0 },
            { eMainStatType.맷집, 0 },
            { eMainStatType.민첩, 0 },
            { eMainStatType.감각, 0 },
            { eMainStatType.지혜, 0 },
        };

        _mainStats = new Dictionary<eMainStatType, MainStat>() {
            { eMainStatType.근력, this.Strength },
            { eMainStatType.맷집, this.Vit },
            { eMainStatType.민첩, this.Avi },
            { eMainStatType.감각, this.Sensation },
            { eMainStatType.지혜, this.Int },
        };

        if (TryGetComponent<LevelSystem>(out LevelSystem lvlSystem))
        {
            lvlSystem.unitLevel.OnLevelUp += (float a, float b) =>
            {
                // TODO stat per level 로드하기
                IncreaseAbilityPoint(5);
            };
        }
        //Evasion = new SubStat("회피", baseStat.Evasion, eSubStatType.회피);

        //receiveAdditionalDamage = new SubStat("받는 피해 증가", 0f, eSubStatType.받는피해증가, true);
        //giveAdditionalDamage = new SubStat("주는 피해 증가", 0f, eSubStatType.주는피해증가, true);
        //receiveLessDamage = new SubStat("받는 피해 감소", 0f, eSubStatType.받는피해감소, true);
        //giveLessDamage = new SubStat("주는 피해 감소", 0f, eSubStatType.받는피해증가, true);

    }

    private void SetupStats()
    {
        Strength = new MainStat("근력", 0);
        Vit = new MainStat("맷집", 0);
        Avi = new MainStat("민첩", 0);
        Sensation = new MainStat("감각", 0);
        Int = new MainStat("지혜", 0);

        HP = new SubStat("체력", baseStat.MaxHP, eSubStatType.health);
        Attack = new SubStat("공격력", baseStat.Attack, eSubStatType.attack);
        MagicAttack = new SubStat("마법 공격력", baseStat.MagicAttack, eSubStatType.magicAttack);

        Defense = new SubStat("방어력", baseStat.Defense, eSubStatType.defense);
        MagicDefense = new SubStat("마법 방어력", baseStat.MagicDefense, eSubStatType.magicDefense);

        AttackSpeed = new SubStat("공격속도", baseStat.AttackSpeed, eSubStatType.attackSpeed, true);
        MoveSpeed = new SubStat("이동속도", baseStat.MoveSpeed, eSubStatType.moveSpeed, true);

        CritChance = new SubStat("크리티컬 확률", baseStat.CritChance, eSubStatType.critChance, true);
        CritDamage = new SubStat("크리티컬 데미지", baseStat.CritDamage, eSubStatType.critDamage, true);

        ExtraGold = new SubStat("골드 추가흭득량", baseStat.ExtraGold, eSubStatType.추가골드, true);
        ExtraExp = new SubStat("경험치 추가흭득량", baseStat.ExtraExp, eSubStatType.추가경험치, true);

        Precision = new SubStat("명중", baseStat.Precision, eSubStatType.명중);
        Evasion = new SubStat("회피", baseStat.Evasion, eSubStatType.회피);

        p_resist = new SubStat("물리 저항력", baseStat.p_resist, eSubStatType.물리저항, 0f, 100f);
        m_resist = new SubStat("마법 저항력", baseStat.magic_resist, eSubStatType.마법저항, 0f, 100f);
        p_penetration = new SubStat("물리 관통력", baseStat.p_penetration, eSubStatType.물리관통력, 0f, 100f);
        m_penetration = new SubStat("마법 관통력", baseStat.magic_penetration, eSubStatType.마법관통력, 0f, 100f);

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
        Attack.AddAsInfluencer(StatUtility.StatPerValue(Vit, 1, 3f));

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
    }

    private void IncreaseAbilityPoint(int val)
    {
        this.AbilityPoint += val;
    }
    
    public void TryAddMainStat(eMainStatType mainStat, int val=1)
    {
        if (this.addedStat + val > this.AbilityPoint) return;

        this.addedStat += val;
        statGiven[mainStat] += val;

        this.AbilityPoint -= val;
        OnTryIncreaseStat?.Invoke(mainStat, statGiven[mainStat]);
    }

    public void ApplyStat()
    {
        foreach (var stat in mainStats.Keys)
        {
            var increaseStat = statGiven[stat];
            if (increaseStat == 0) continue;
            mainStats[stat].IncreaseStat(increaseStat);
            OnIncreaseStat?.Invoke(stat);
        }

        foreach (var stat in mainStats.Keys)
        {
            statGiven[stat] = 0;
        }
        this.AbilityPoint -= this.addedStat;
        this.addedStat = 0;
    }


    public void CancelStatChange()
    {
        this.AbilityPoint += this.addedStat;
        this.addedStat = 0;
        foreach (var stat in mainStats.Keys)
        {
            statGiven[stat] = 0;
        }

    }

    #region Damage 
    // 체력 - (적 공격력 - 방어력 (>=0) ) * (100 - (아군 물리저항 - 적 물리관통력 >= 0) / 100 or (아군마법저항 - 적 마법관통력) / 100)
    // => 적체력 - ((아공 - 적방) * (100 - (적군저항 - 아군관통력 / 100)) 
    // 데미지 * ( 
    // 공격자가 부름,
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

    public float Defend(float inComingDamage)
    {
        return Mathf.Max(1f, inComingDamage - Defense.FinalValue);
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
}
