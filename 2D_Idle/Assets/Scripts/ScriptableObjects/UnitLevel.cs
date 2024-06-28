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
    [ShowInInspector] public int level { get; protected set; } = 1;

    public float CurrentExp => currentExp;
    public float MaxExp
    {
        get
        {
            maxExp = MaxExpByLevel[level - 1];
            return maxExp;
        }
    }

    public AnimationCurve curve;

    [SerializeField] protected float currentExp;
    protected float maxExp = 100f;


    public float growthFactor = 1.1f;
    public float extraExpPerLevel = 50;

    [ListDrawerSettings(ShowIndexLabels = true)]
    public List<float> MaxExpByLevel = new List<float>();
    [SerializeField] protected float initMaxExp = 100f;

    public UnityAction<float, float> OnLevelUp;
    public UnityAction<float, float> OnGainExp;
    public UnityAction<float, float> OnInitSetup;

    [Header("EXP Per Time")]
    [SerializeField] private float totalEXPTilMaxLevel;
    [SerializeField] private float minutesTilMaxLevel;
    [SerializeField] private float hoursTilMaxLevel;
    [SerializeField] private float daysTilMaxLevel;
    [SerializeField] private int expGainPerSec = 1;
    public virtual bool GainExp(int value)
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

    [Button("Levelup")]
    public virtual void Grow()
    {
        level += 1;
        if (level >= maxLevel)
        {
            OnReachMaxLevel();
        }
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

    [Button("Initialize")]
    public virtual void Init()
    {
        this.level = 1;
        this.currentExp = 0f;
        //this.initMaxExp = this.maxExp;
        
        UpdateMaxExpsPerLevel();
    }

    public void UpdateMaxExpsPerLevel()
    {
        MaxExpByLevel.Clear();

        float _maxExp = initMaxExp;
        MaxExpByLevel.Add(Mathf.Round(_maxExp));
        totalEXPTilMaxLevel = 0;
        for (int i = 1; i < maxLevel; i++)
        {
            int _level = i + 1;
            float growth = 1 + curve.Evaluate((float)_level / (float)maxLevel);

            float fin_maxExp = (_maxExp + (extraExpPerLevel*i)) * Mathf.Pow(growthFactor, _level) * growth;
            MaxExpByLevel.Add(Mathf.Round(fin_maxExp));
            totalEXPTilMaxLevel += fin_maxExp;
        }

        minutesTilMaxLevel = Mathf.Round(totalEXPTilMaxLevel / 60f);
        hoursTilMaxLevel = Mathf.Round(totalEXPTilMaxLevel / 3600f);
        daysTilMaxLevel = Mathf.Round(totalEXPTilMaxLevel / (3600f*24));
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

    public virtual void OnReachMaxLevel() { }

}
