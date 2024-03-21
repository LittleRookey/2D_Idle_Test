using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentTier", menuName = "Litkey/EquipmentTier")]
public class EquipmentTier : ScriptableObject
{
    public int currentTier; // ����� Ƽ��, ������ ���� �ٴٸ��� Ƽ� �ø��� �ִ�
    public int currentLevel; // ���� ����
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
