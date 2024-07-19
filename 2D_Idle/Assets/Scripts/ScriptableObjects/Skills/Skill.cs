using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Litkey.Stat;
using UnityEngine.Events;
using Litkey.Character.Cooldowns;
using Redcode.Pools;

namespace Litkey.Skill
{
    public enum eSkillRank
    {
        일반,
        고급,
        희귀,
        영웅,
        전설,
        초월,
        고대,
        불멸,
        신화,
    }
    [InlineEditor]
    [System.Serializable]
    public abstract class Skill : SerializedScriptableObject
    {
        [PreviewField]
        public Sprite _icon;
        public string skillName;
        public int skillLevel => Level.level;
        public eSkillRank startRank;
        public eSkillRank currentRank;
        public eSkillRank maxRank;
        public float baseSkillDamage;
        public SkillLevel Level;

        [SerializeField] protected string abilityUseSound;

        // 랭크당 업그레이드 스텟들
        public Dictionary<eSkillRank, List<Decorator>> rankUpgrades;


        protected float _addedSkillDamage = 0f;

        public float finalDamage => baseSkillDamage + _addedSkillDamage;

        [TextArea]
        public string skillExplanation;

        protected string skillLevelPath = "ScriptableObject/Skills/SkillLevel/SkillLevels";

        public UnityEvent<Skill> OnIncreaseExp;

        protected virtual void Awake() 
        {

        }

        protected virtual void OnEnable() { }

        protected virtual void OnDisable() { }

        public abstract void SetInitialState();

        public abstract void ApplyEffect(StatContainer allyStat, StatContainer target);

        public void IncreaseOneExp()
        {
            if (this.Level != null)
            {
                this.Level.GainExp(1);
                OnIncreaseExp?.Invoke(this);
            }
        }

        public eSkillRank GetMaxUpgradeRank()
        {
            return maxRank;

        }
        public virtual float GetTotalDamage() { return 0f; }

        public void AddSkillDamage(float additionalDmg) => _addedSkillDamage += additionalDmg;

        public void RemoveSkillDamage(float additionalDMg) => _addedSkillDamage -= additionalDMg;

        public void ClearAddedSkillDamage() => _addedSkillDamage = 0f;
        // 
        public virtual void SkillLevelUp() { }

        public void SetData(SkillData skillData)
        {
            this.Level.SetLevel(skillData.skillLevel, skillData.currentExp);
        }
    }

    public abstract class PassiveSkill : Skill
    {
        // 레벨당 업그레이드 스텟들
        [SerializeField] protected Dictionary<eSkillRank, Dictionary<int, List<StatModifier>>> levelUpgrades; // 어떤 레벨 업그레이드들이 있는지 저장
        public Dictionary<eSkillRank, Dictionary<int, List<StatModifier>>> LevelUpgrades => levelUpgrades;

        public List<StatModifier> AppliedLevelUpgrades => _appliedLevelUpgrades;
        protected List<StatModifier> _appliedLevelUpgrades; // 적용된 레벨 효과들 모음

        public UnityEvent<PassiveSkill> OnSkillLevelUp;

        protected abstract void OnRankUp(eSkillRank rank);
        protected abstract void OnLevelUp();

        public abstract void EquipPassiveStat(StatContainer statContainer);

    }

    public abstract class ActiveSkill : Skill, IHasCooldown
    {
        public float cooldown;
        public float TimeTilActive; // 발동까지 걸리는시간
        public int targetNumber;
        public int attackNumber; // 공격횟수
        public string animationName;
        public GameObject skillEffect;

        public Dictionary<eSkillRank, Dictionary<int, float>> skillDamagePerLevel; // 각 랭크 레벨별 데미지업
        public Dictionary<eSkillRank, float> skillDamagePerRank; // 각 랭크업별 데미지업
        public List<SkillCondition> SkillConditions; // 스킬 쓰는 조건

        public int totalScore;
        public string ID => skillName;

        public float CooldownDuration => cooldown;

        public LayerMask enemyLayer;

        public eSkillRangeType skillRangeType;

        public UnityEvent OnSkillUse;
        public UnityEvent OnSkillLevelUp;

        protected override void Awake()
        {
            base.Awake();
            InitializeLevel();
        }
        
        protected override void OnEnable()
        {
            OnSkillUse.AddListener(IncreaseOneExp);
            Level.OnLevelUp += SkillLevelUpLambda;
        }
        protected override void OnDisable()
        {
            OnSkillUse.RemoveListener(IncreaseOneExp);
            Level.OnLevelUp -= SkillLevelUpLambda;
        }

        protected void SkillLevelUpLambda(float x, float y) => SkillLevelUp();
        public abstract void Use(StatContainer ally, Health target);


        public void InitializeLevel()
        {
            this.Level.Init();
        }

        public override float GetTotalDamage()
        {
            SkillLevelUp();
            return finalDamage;
        }

        [Button("UpdateSkillDMG")]
        public override void SkillLevelUp()
        {
            ClearAddedSkillDamage(); 
            float startDMG = 0f;

            // 같은 랭크의 애들은 레벨에따라
            if (startRank == currentRank)
            {
                for (int k = 0; k < Level.level; k++)
                {
                    if (skillDamagePerLevel[startRank].ContainsKey(k))
                    {
                        startDMG += skillDamagePerLevel[startRank][k];
                    }
                }
            }

            // 랭크가 더 낮으면 전부 Add하기
            for (int i = (int)startRank; i < (int)currentRank; i++)
            {
                if (startRank < currentRank)
                {
                    for (int k = 0; k < Level.maxLevel; k++)
                    {
                        if (skillDamagePerLevel[(eSkillRank)i].ContainsKey(k))
                        {
                            startDMG += skillDamagePerLevel[(eSkillRank)i][k];
                        }
                    }
                }
            }
            Debug.Log("Skill Damage added: " + startDMG);
            AddSkillDamage(startDMG);
        }

        public void IncreaseTargetNumber(int num)
        {
            targetNumber += num;
        }

        public void ReduceCooldown(float cooldown)
        {
            this.cooldown = Mathf.Max(1f, this.cooldown - cooldown);
        }

        public float CalculateSkillScore(StatContainer ally)
        {
            int totalScore = 0;

            foreach (var condition in SkillConditions)
            {
                if (condition.EvaluateCondition(ally))
                {
                    totalScore += condition.Score;
                }
            }
            this.totalScore = totalScore;
            return totalScore;
        }

        

    }

    // 스킬에 넣을 추가효과용
    public abstract class Decorator : ScriptableObject
    {
        public bool isUnlocked;

        [Header("업그레이드 설명")]
        [TextArea]
        public string upgradeExplanation;

        public abstract void AddEffect(Skill skill);

        public void Unlock() => isUnlocked = true;

        public void Lock() => isUnlocked = false;
    }
}