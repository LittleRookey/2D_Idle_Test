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
        �Ϲ�,
        ���,
        ���,
        ����,
        ����,
        �ʿ�,
        ���,
        �Ҹ�,
        ��ȭ,
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

        // ��ũ�� ���׷��̵� ���ݵ�
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
        // ������ ���׷��̵� ���ݵ�
        [SerializeField] protected Dictionary<eSkillRank, Dictionary<int, List<StatModifier>>> levelUpgrades; // � ���� ���׷��̵���� �ִ��� ����
        public Dictionary<eSkillRank, Dictionary<int, List<StatModifier>>> LevelUpgrades => levelUpgrades;

        public List<StatModifier> AppliedLevelUpgrades => _appliedLevelUpgrades;
        protected List<StatModifier> _appliedLevelUpgrades; // ����� ���� ȿ���� ����

        public UnityEvent<PassiveSkill> OnSkillLevelUp;

        protected abstract void OnRankUp(eSkillRank rank);
        protected abstract void OnLevelUp();

        public abstract void EquipPassiveStat(StatContainer statContainer);

    }

    public abstract class ActiveSkill : Skill, IHasCooldown
    {
        public float cooldown;
        public float TimeTilActive; // �ߵ����� �ɸ��½ð�
        public int targetNumber;
        public int attackNumber; // ����Ƚ��
        public string animationName;
        public GameObject skillEffect;

        public Dictionary<eSkillRank, Dictionary<int, float>> skillDamagePerLevel; // �� ��ũ ������ ��������
        public Dictionary<eSkillRank, float> skillDamagePerRank; // �� ��ũ���� ��������
        public List<SkillCondition> SkillConditions; // ��ų ���� ����

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

            // ���� ��ũ�� �ֵ��� ����������
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

            // ��ũ�� �� ������ ���� Add�ϱ�
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

    // ��ų�� ���� �߰�ȿ����
    public abstract class Decorator : ScriptableObject
    {
        public bool isUnlocked;

        [Header("���׷��̵� ����")]
        [TextArea]
        public string upgradeExplanation;

        public abstract void AddEffect(Skill skill);

        public void Unlock() => isUnlocked = true;

        public void Lock() => isUnlocked = false;
    }
}