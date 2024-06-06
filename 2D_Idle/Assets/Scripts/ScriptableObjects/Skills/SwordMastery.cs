using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Litkey.Skill
{


    [CreateAssetMenu(fileName = "SwordMastery", menuName = "Litkey/Skills/SwordMastery")]
    public class SwordMastery : PassiveSkill
    {
        private StatContainer equippedStatContainer;

        [SerializeField] private PlayerBasicAttack playerAttack;


        protected override void OnEnable()
        {
            base.OnEnable();
            // 레벨 로드하고 
            OnLevelUp();

            this.Level.OnLevelUp += LevelUp;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.Level.OnLevelUp -= LevelUp;
        }

        protected override void OnRankUp(eSkillRank rank)
        {
            if (rankUpgrades.ContainsKey(rank))
            {
                for (int i = 0; i < rankUpgrades[rank].Count; i++)
                {
                    rankUpgrades[rank][i].Unlock();
                    rankUpgrades[rank][i].AddEffect(this);
                }


            }
        }

        private void LevelUp(float cur, float max) => OnLevelUp();
        // 스킬 레벨업 했을떄 불림
        protected override void OnLevelUp()
        {
            if (this.equippedStatContainer != null)
                this.equippedStatContainer.UnEquipStat(this);

            _appliedLevelUpgrades.Clear();
            int currentLevel = this.Level.level;
            for (int i = 0; i < currentLevel; i++)
            {
                if (levelUpgrades[currentRank].ContainsKey(i))
                {
                    CombineStats(levelUpgrades[currentRank][i]);
                }
            }
            OnSkillLevelUp?.Invoke(this);
        }


        public override void Initialize()
        {
            Init();

        }
        public void Init()
        {
            _appliedLevelUpgrades = new List<StatModifier>();
            this.Level.SetLevel(1, 0f);

            int currentLevel = this.Level.level;
            Debug.Log("Sword Mastery init level: " + currentLevel);
            for (int i = 0; i < currentLevel; i++)
            {
                //Debug.Log("Sword Mastery init: " + levelUpgrades[currentRank].ContainsKey(i));
                if (levelUpgrades[currentRank].ContainsKey(i))
                {
                    CombineStats(levelUpgrades[currentRank][i]);
                }
            }
        }

        //public List<StatModifier> GetPassiveStats()
        //{

        //}

        private void CombineStats(List<StatModifier> stats)
        {
            //Debug.Log("Combined stats");
            for (int i = 0; i < stats.Count; i++)
            {
                _appliedLevelUpgrades.Add(stats[i]);
            }
        }

        public override void ApplyEffect(StatContainer allyStat, StatContainer target)
        {
            throw new System.NotImplementedException();
        }

        // 
        public override void EquipPassiveStat(StatContainer statContainer)
        {
            if (equippedStatContainer == null) equippedStatContainer = statContainer;
            Debug.Log("Equip Stat called from Sword master");
            // 전에 있던 스텟들을 제거
            Debug.Log("AppliedLevelUpgrades: " + _appliedLevelUpgrades.Count);
            statContainer.AddETCStat(_appliedLevelUpgrades);
            // 스텟 

        }
    }

}