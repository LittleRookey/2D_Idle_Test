using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu (menuName ="Litkey/Loots/LeveledRankLootTable")]
public class LeveledRankLootTable : SerializedScriptableObject
{
    [SerializeField] private UnitLevel resourceLevel;
    [SerializeField] private Dictionary<int, RankedLootTable> lootTablePerLevel;
    public RankedLootTable GetRankedLootTable()
    {
        int level = resourceLevel.level;
        if (!lootTablePerLevel.ContainsKey(level)) Debug.LogError($"Ranked LootTable of level {level} does not exist");

        return lootTablePerLevel[level];
    }
    
}
