using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "EquipmentTier", menuName = "Litkey/EquipmentTier")]
public class EquipmentTier : SerializedScriptableObject
{
    [Header("��� ����")]
    private int _requiredGold; // ���� ������ �ʿ��� ���
    public int requiredGold
    {
        get
        {
            UpdateRequiredGold();
            return _requiredGold;
        }
    }
    public int initialRequiredGold = 1; // ó���� �ʿ��� ���
    public int extraGoldForNextLevel = 3; // ���� �������� �ʿ��� �߰����
    public float growthFactor = 1.023f;
    public AnimationCurve goldCurve;
    public List<int> requiredGolds;

    [Header("���Ƽ�� ����")]
    public int currentTier; // ����� Ƽ��, ������ ���� �ٴٸ��� Ƽ� �ø��� �ִ�
    public int currentLevel; // ���� ����
    [Range(0, 1000)]
    public int maxTier = 30; // Ƽ�� �ƽ�

    [DictionaryDrawerSettings(KeyLabel = "Tier", ValueLabel = "Max Level")]
    public Dictionary<int, int> maxLevelsPerTier = new Dictionary<int, int>(); // �� Ƽ� Max ����

    public int currentMaxLevel = 20;
    public int maxLevel = 500; // �� ����
    public int totalUpgradeLevel;

    [Header("��� �������")]
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
