using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "EquipmentTier", menuName = "Litkey/EquipmentTier")]
public class EquipmentTier : SerializedScriptableObject
{
    [Header("골드 관련")]
    private int _requiredGold; // 현재 레벨에 필요한 골드
    public int requiredGold
    {
        get
        {
            UpdateRequiredGold();
            return _requiredGold;
        }
    }
    public int initialRequiredGold = 1; // 처음에 필요한 골드
    public int extraGoldForNextLevel = 3; // 다음 레벨업에 필요한 추가골드
    public float growthFactor = 1.023f;
    public AnimationCurve goldCurve;
    public List<int> requiredGolds;

    [Header("장비티어 관련")]
    public int currentTier; // 장비의 티어, 레벨이 끝에 다다르면 티어를 올릴수 있다
    public int currentLevel; // 실제 레벨
    [Range(0, 1000)]
    public int maxTier = 30; // 티어 맥스

    [DictionaryDrawerSettings(KeyLabel = "Tier", ValueLabel = "Max Level")]
    public Dictionary<int, int> maxLevelsPerTier = new Dictionary<int, int>(); // 각 티어별 Max 레벨

    public int currentMaxLevel = 20;
    public int maxLevel = 500; // 총 레벨
    public int totalUpgradeLevel;

    [Header("장비 설명관련")]
    public string equipmentName;
    [TextArea]
    public string itemExplanation;
    public Sprite[] equipmentSprite;

    
    private void OnEnable()
    {
        UpdateRequiredGold();
        //requiredGold = (int)(requiredGold * goldCurve.Evaluate(currentTier));
        
    }


    public void Initialize(EquipmentUpgradeStatus status)
    {
        currentLevel = status.weaponLevel;
        currentTier = status.weaponRank;
        totalUpgradeLevel = status.totalUpgradeLevel;
        // current level
    }
    public void UpgradeLevel()
    {
        if (currentLevel >= currentMaxLevel) return;

        IncreaseLevel();
        UpdateRequiredGold();
    }

    private Sprite UpgradeTier()
    {
        IncreaseLevel();
        currentLevel = 0;
        currentTier++;
        return equipmentSprite[currentTier];
    }

    private void IncreaseLevel()
    {
        currentLevel++;
        totalUpgradeLevel++;
    }

    private void UpdateRequiredGold()
    {
        if (totalUpgradeLevel > maxLevel) return;
        //requiredGold = (int)(requiredGold * (1f + goldCurve.Evaluate(currentLevel)));
        _requiredGold = requiredGolds[totalUpgradeLevel];
    }

    [Button("Update Max Exps Per Level")]
    public void UpdateMaxGolds()
    {
        requiredGolds.Clear();
        requiredGolds.Add(initialRequiredGold);
        for (int i = 1; i < maxLevel; i++)
        {
            int _level = i + 1;
            float growth = 1 + goldCurve.Evaluate((float)_level / (float)maxLevel);
            int fin_gold = (int)((initialRequiredGold + (extraGoldForNextLevel * i)) * Mathf.Pow(growthFactor, _level) * growth);

            requiredGolds.Add(fin_gold);
        }
    }

    //public void UpdateMaxExpsPerLevel()
    //{
    //    requiredGolds.Clear();

    //    float _initialGold = initialRequiredGold;

    //    for (int i = 1; i < maxLevel; i++)
    //    {
    //        int _level = i + 1;
    //        float growth = 1 + goldCurve.Evaluate((float)_level / (float)maxLevel);
    //        //_maxExp *= growth;
    //        float fin_gold = (int)((_initialGold + (extraGoldForNextLevel * i)) * Mathf.Pow(growthFactor, _level) * growth);
    //        requiredGolds.Add(Mathf.Round(fin_gold));
    //    }
    //}

}
