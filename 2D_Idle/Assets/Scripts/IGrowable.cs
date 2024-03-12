using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IGrowable 
{
   

    public void Init(); // sets up the initial value of the ability 

    public bool GainExp(int value); // earns exp 

    public void Grow(); // increases the level of certain thing 
}

[System.Serializable]
public class AbilityLevel: IGrowable
{
    [SerializeField] public int maxLevel = 100;
    public bool showLog;
    public int level = 1;

    public float currentExp;
    public float maxExp = 100f;

    public AnimationCurve curve;

    public float growthFactor = 1.1f;
    public float extraExpPerLevel = 50;
    
    public List<float> MaxExpByLevel = new List<float>();
    private float initMaxExp = 100f;

    public UnityAction OnLevelUp;

    public AbilityLevel(AnimationCurve animCurve)
    {
        this.maxLevel = 100;
        this.maxExp = 100f;
        this.initMaxExp = this.maxExp;
        this.curve = new AnimationCurve(animCurve.keys);
        this.growthFactor = 1.1f;

        UpdateMaxExpsPerLevel();
    }

    public AbilityLevel(int maxLevel, float maxExp, AnimationCurve animCurve, float growthFactor=1.1f)
    {
        this.maxLevel = maxLevel;
        this.maxExp = maxExp;
        this.initMaxExp = this.maxExp;
        this.curve = new AnimationCurve(animCurve.keys);
        this.growthFactor = growthFactor;

        UpdateMaxExpsPerLevel();
    }

    public AbilityLevel(int currentLevel, float currentExp, AnimationCurve animCurve)
    {
        this.maxLevel = 100;
        this.maxExp = 100f;
        this.initMaxExp = this.maxExp;
        this.curve = new AnimationCurve(animCurve.keys);
        this.growthFactor = 1.1f;

        UpdateMaxExpsPerLevel();
    }
    //public Dictionary<int, skillUpgrade> skillUpgrades;

    //public delegate void HitEffect(SpellEffect spellEffect, GameObject caster, GameObject target, Vector3 hitPoint);
    //public delegate void skillUpgrade(Ability ability);
    public bool GainExp(int value)
    {
        bool levelUp = false;
        currentExp += value;

        while (currentExp >= maxExp)
        {
            currentExp -= maxExp;
            levelUp = true;
            Grow();
        }

        return levelUp;
    }

    public virtual void Grow()
    {
        level += 1;
        float growth = 1 + curve.Evaluate((float)level / (float)maxLevel);

        //_maxExp *= growth;
        float fin_maxExp = (initMaxExp + extraExpPerLevel) * Mathf.Pow(growthFactor, level) * growth;
        //if (growth < 1f)
        //    Debug.LogError("Growth is decreasing");
        maxExp = Mathf.Round(fin_maxExp);

        OnLevelUp?.Invoke();

        if (showLog)
            Debug.Log($"Level {level} - maxEXP - {maxExp}\nGrowth - {growth}");
        
    }

    public void Init()
    {
        
    }

    public void UpdateMaxExpsPerLevel()
    {
        MaxExpByLevel.Clear();
        //float growthFactor = 1.1f;
        float _maxExp = maxExp;
        for (int i = 0; i <  maxLevel; i++)
        {
            
            int _level = i + 1;
            float growth = 1 + curve.Evaluate((float)_level / (float)maxLevel);
            //_maxExp *= growth;
            float fin_maxExp = (_maxExp + extraExpPerLevel) * Mathf.Pow(growthFactor, _level) * growth;
            MaxExpByLevel.Add(Mathf.Round(fin_maxExp));
        }
    }

    
    public void ShowMaxExps()
    {
        UpdateMaxExpsPerLevel();
    }

    public float GetMaxExpAtLevel(int level)
    {
        return MaxExpByLevel[level - 1];
    }
}

[System.Serializable]
public class SkillLevel: AbilityLevel
{
    public int SkillID = 0;

    public List<SkillGrowthData> skillGrowthDatas;


    public UnityAction<int, SkillGrowthData> OnSkillGrowth;

    public SkillLevel(int maxLevel, float maxExp, AnimationCurve animCurve, float growthFactor = 1.1f) : base (maxLevel, maxExp, animCurve, growthFactor)
    {
        
    }



    public override void Grow()
    {
        base.Grow();

        // check for skill upgrades
        foreach(SkillGrowthData sk in skillGrowthDatas)
        {
            //Debug.Log((sk.level == level) + sk.level.ToString() + " " + level.ToString());
            //if (sk.level == level)
            //{ // 
            //    OnSkillGrowth?.Invoke(SkillID, sk);

            //}
        }
    }
}

[System.Serializable]
public class SkillGrowthData
{
    public int addedProjectileNumber;
    public float addedSkillDamagePercent;
    public float addedReducedCooldownTime;

    
    public SkillGrowthData(int addedProjectileNumber, float addedSkillDamagePercent, float addedReducedCooldownTime)
    {
        this.addedProjectileNumber = addedProjectileNumber;
        this.addedSkillDamagePercent = addedSkillDamagePercent;
        this.addedReducedCooldownTime = addedReducedCooldownTime;

    }

    //public delegate void onUpgrade();
}