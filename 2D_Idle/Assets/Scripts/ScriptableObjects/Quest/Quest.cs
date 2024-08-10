using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace Litkey.Quest
{
    public enum QuestType
    {
        KillEnemies,
        KillSpecificEnemies,
        CollectItems,
        ReachLocation,
        // Add more quest types as needed
    }

    [InlineEditor]
    [CreateAssetMenu(menuName ="Litkey/Quest/Quest")]
    public class Quest : SerializedScriptableObject
    {
        public string questID;
        public string questName;
        [TextArea]
        public string description;
        [EnumToggleButtons]
        public QuestType questType;
        public List<Objective> objectives;
        public List<string> prerequisiteQuestIds;
        public RewardGroup reward;

    }

    [System.Serializable]
    public class QuestData
    {
        public List<QuestProgress> questProgresses = new List<QuestProgress>();
    }

    [System.Serializable]
    public class QuestProgress
    {
        public string questId;
        public Dictionary<string, int> objectiveProgress;
        public bool isCompleted;
        public bool isActive;
        public bool rewardClaimed;

        public QuestProgress(string id)
        {
            questId = id;
            objectiveProgress = new Dictionary<string, int>();
            isCompleted = false;
            isActive = false;
            rewardClaimed = false;
        }
    }
}
