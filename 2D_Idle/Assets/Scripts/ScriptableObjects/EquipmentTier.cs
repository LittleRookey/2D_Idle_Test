using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentTier", menuName = "Litkey/EquipmentTier")]
public class EquipmentTier : ScriptableObject
{
    [Header("골드 관련")]
    public int requiredGold; // 현재 레벨에 필요한 골드
    public int initialRequiredGold = 100; // 처음에 필요한 골드
    public int extraGoldForNextLevel = 5; // 다음 레벨업에 필요한 추가골드
    public float growthFactor = 1.1f;
    public AnimationCurve goldCurve;
    public List<int> requiredGolds;

    [Header("장비티어 관련")]
    public int currentTier; // 장비의 티어, 레벨이 끝에 다다르면 티어를 올릴수 있다
    public int currentLevel; // 실제 레벨
    public Sprite[] equipmentSprite;
    
    public int maxLevel = 20;
    public int totalUpgradeLevel;

    [Header("장비 설명관련")]
    public string equipmentName;
    [TextArea]
    public string itemExplanation;

    private void OnEnable()
    {
        UpdateRequiredGold();
        //requiredGold = (int)(requiredGold * goldCurve.Evaluate(currentTier));
    }
    public void UpgradeLevel()
    {
        if (currentLevel >= maxLevel) return;

        currentLevel++;
        totalUpgradeLevel++;
        UpdateRequiredGold();
    }
    private Sprite UpgradeTier()
    {
        currentTier++;
        return equipmentSprite[currentTier];
    }

    private void UpdateRequiredGold()
    {
        //requiredGold = (int)(requiredGold * (1f + goldCurve.Evaluate(currentLevel)));
        requiredGold = requiredGolds[totalUpgradeLevel];
    }

    [Button("Update Max Exps Per Level")]
    public void UpdateMaxGolds()
    {
        requiredGolds.Clear();
        requiredGolds.Add(initialRequiredGold);
        for (int i = 0; i < maxLevel; i++)
        {
            int _level = i + 1;
            float growth = 1 + goldCurve.Evaluate((float)_level / (float)maxLevel);
            int fin_gold = (int)((initialRequiredGold + (extraGoldForNextLevel * i)) * Mathf.Pow(growthFactor, _level) * growth);

            requiredGolds.Add(fin_gold);
        }
    }
}
