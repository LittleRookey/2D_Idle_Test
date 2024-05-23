using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public interface IGrowable
{


    public void Init(); // sets up the initial value of the ability 

    public bool GainExp(int value); // earns exp 

    public void Grow(); // increases the level of certain thing 
}


[CreateAssetMenu(fileName = "UnitLevel", menuName = "Litkey/UnitLevels")]
public class UnitLevel : ScriptableObject
{
    [SerializeField] public int maxLevel = 100;
    public bool showLog;
    public int level { get; private set; } = 1;

    public float CurrentExp => currentExp;
    public float MaxExp
    {
        get
        {
            maxExp = MaxExpByLevel[level - 1];
            return maxExp;
        }
    }



    private float currentExp;
    private float maxExp = 100f;

    public AnimationCurve curve;

    public float growthFactor = 1.1f;
    public float extraExpPerLevel = 50;
    [ListDrawerSettings(ShowIndexLabels = true)]
    public List<float> MaxExpByLevel = new List<float>();
    [SerializeField] private float initMaxExp = 100f;

    //public Queue<UnityAction> OnLevelUp;
    public UnityAction<float, float> OnLevelUp;
    public UnityAction<float, float> OnGainExp;
    public UnityAction<float, float> OnInitSetup;
    public bool GainExp(int value)
    {
        bool levelUp = false;
        currentExp += value;
        OnGainExp?.Invoke(currentExp, maxExp);
        while (currentExp >= maxExp)
        {
            currentExp -= maxExp;
            levelUp = true;
            Grow();
        }

        return levelUp;
    }

    public float GetCurrentExp()
    {
        return currentExp;
    }

    public virtual void Grow()
    {
        level += 1;

        float fin_maxExp = MaxExpByLevel[level - 1];

        maxExp = Mathf.Round(fin_maxExp);

        OnLevelUp?.Invoke(currentExp, maxExp);
    }


    public void SetLevel(int level, float currentExp)
    {
        this.level = level;
        maxExp = MaxExpByLevel[level - 1];
        this.currentExp = currentExp;
        OnInitSetup?.Invoke(currentExp, maxExp);
    }

    public void Init()
    {
        this.maxLevel = 100;
        this.maxExp = 100f;
        this.initMaxExp = this.maxExp;
        
        UpdateMaxExpsPerLevel();
    }

    public void UpdateMaxExpsPerLevel()
    {
        MaxExpByLevel.Clear();
        //float growthFactor = 1.1f;
        float _maxExp = initMaxExp;
        MaxExpByLevel.Add(Mathf.Round(_maxExp));

        for (int i = 1; i < maxLevel; i++)
        {
            int _level = i + 1;
            float growth = 1 + curve.Evaluate((float)_level / (float)maxLevel);
            //_maxExp *= growth;
            float fin_maxExp = (_maxExp + (extraExpPerLevel*i)) * Mathf.Pow(growthFactor, _level) * growth;
            MaxExpByLevel.Add(Mathf.Round(fin_maxExp));
        }
    }

    [Button("Update Max Exps Per Level")]
    public void ShowMaxExps()
    {
        UpdateMaxExpsPerLevel();
    }


    public float GetMaxExpAtLevel(int level)
    {
        return MaxExpByLevel[level - 1];
    }

    /// <summary>
    /// 현재 경험치 비율을 리턴
    /// </summary>
    /// <returns></returns>
    public float GetExpRatio()
    {
        return currentExp / maxExp;
    }

    public UnitLevel Clone()
    {
        Init();
        return this.Clone();
    }
}

//[System.Serializable]
//public class SkillLevel : AbilityLevel
//{
//    public int SkillID = 0;

//    public List<SkillGrowthData> skillGrowthDatas;

//    //public onSkillUpgrade onSkillUpgraded;

//    //public delegate void onSkillUpgrade();
//    public UnityAction<int, SkillGrowthData> OnSkillGrowth;
//    //public SkillLevel(int maxLevel, AnimationCurve animCurve, float growthFactor = 1.1f) : base(maxLevel, maxExp, animCurve, growthFactor)
//    //{
//    //    maxExp = 100f;
//    //}

//    public SkillLevel(int maxLevel, float maxExp, AnimationCurve animCurve, float growthFactor = 1.1f) : base(maxLevel, maxExp, animCurve, growthFactor)
//    {

//    }


//    [Button("Grow")]
//    public override void Grow()
//    {
//        base.Grow();

//        // check for skill upgrades
//        foreach (SkillGrowthData sk in skillGrowthDatas)
//        {
//            Debug.Log((sk.level == level) + sk.level.ToString() + " " + level.ToString());
//            if (sk.level == level)
//            { // 
//                OnSkillGrowth?.Invoke(SkillID, sk);

//            }
//        }
//    }
//}

//[System.Serializable]
//public class SkillGrowthData
//{
//    public int level; // when reach this level, run certian function

//    //public Ability ability; // 

//    //public onUpgrade OnUpgrade;

//    public int addedProjectileNumber;
//    public float addedSkillDamagePercent;

//    public float addedReducedCooldownTime;
//    public SkillGrowthData(int level, int addedProjectileNumber, float addedSkillDamagePercent, float addedReducedCooldownTime)
//    {
//        this.level = level;
//        this.addedProjectileNumber = addedProjectileNumber;
//        this.addedSkillDamagePercent = addedSkillDamagePercent;
//        this.addedReducedCooldownTime = addedReducedCooldownTime;
//    }

//    //public delegate void onUpgrade();
//}