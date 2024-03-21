using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentTier", menuName = "Litkey/EquipmentTier")]
public class EquipmentTier : ScriptableObject
{
    public int currentTier; // 장비의 티어, 레벨이 끝에 다다르면 티어를 올릴수 있다
    public int currentLevel; // 실제 레벨
    public int requiredGold;
    public Sprite[] equipmentSprite;
    public AnimationCurve goldCurve;
    public int maxLevel = 20;
    public int[] requiredGolds;

    public int totalUpgradeLevel;

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
    public Sprite UpgradeTier()
    {
        currentTier++;
        return equipmentSprite[currentTier];
    }

    private void UpdateRequiredGold()
    {
        //requiredGold = (int)(requiredGold * (1f + goldCurve.Evaluate(currentLevel)));
        requiredGold = requiredGolds[totalUpgradeLevel];
    }

}
