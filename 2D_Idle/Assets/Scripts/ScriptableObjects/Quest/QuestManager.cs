using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Litkey.Quest
{
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance;

        public QuestDatabase questDatabase;
        public GameDatas gameDatas;

        private QuestData questData;
        private HashSet<string> activeQuestIds = new HashSet<string>();

        //public UnityEvent<Quest> OnProgressUpdated;
        //public UnityEvent<Quest> OnQuestCompleted;

        private void Awake()
        {
            Instance = this;
            InitializeQuestData();
        }

        private void OnEnable()
        {
            QuestEvents.OnActionPerformed += HandleActionPerformed;
        }

        private void OnDisable()
        {
            QuestEvents.OnActionPerformed -= HandleActionPerformed;
        }

        private void InitializeQuestData()
        {
            if (gameDatas.dataSettings.questData == null)
            {
                gameDatas.dataSettings.questData = new QuestData();
            }
            questData = gameDatas.dataSettings.questData;

            foreach (var quest in questDatabase.quests)
            {
                if (!questData.questProgresses.Exists(qp => qp.questId == quest.questID))
                {
                    QuestProgress newProgress = new QuestProgress(quest.questID);
                    foreach (var objective in quest.objectives)
                    {
                        newProgress.objectiveProgress[objective.objectiveId] = 0;
                    }
                    questData.questProgresses.Add(newProgress);
                }
                else
                {

                }
            }
        }

        public Quest GetQuest(string questID)
        {
            return questDatabase.GetQuest(questID);
        }

        public QuestProgress GetQuestProgress(string questID)
        {
            if (questData == null)
            {
                Debug.LogError("QuestData is null");
                return null;
            }
            if (!questData.questProgresses.Exists(qp => qp.questId == questID))
            {
                return null;
            }
            var questProgress = this.questData.questProgresses.Find(qp => qp.questId == questID);
            
            if (questProgress == null) return null;

            return questProgress;
        }

        public void ActivateQuest(string questId)
        {
            QuestProgress questProgress = questData.questProgresses.Find(qp => qp.questId == questId);
            if (questProgress != null && !questProgress.isActive)
            {
                Quest quest = questDatabase.GetQuest(questId);
                if (quest != null && ArePrerequisitesMet(quest))
                {
                    questProgress.isActive = true;
                    activeQuestIds.Add(questId);
                    QuestEvents.QuestActivated(questId);
                    Debug.Log($"Quest {questId} activated.");
                }
                else
                {
                    Debug.Log($"Cannot activate quest {questId}. Prerequisites not met.");
                }
            }
        }

        private bool ArePrerequisitesMet(Quest quest)
        {
            return quest.prerequisiteQuestIds.TrueForAll(preReqId =>
                questData.questProgresses.Exists(qp => qp.questId == preReqId && qp.isCompleted));
        }

        private void HandleActionPerformed(QuestType actionType, string targetId, int amount)
        {
            foreach (var questId in activeQuestIds)
            {
                UpdateQuestProgress(questId, actionType, targetId, amount);
            }
        }

        private void UpdateQuestProgress(string questId, QuestType actionType, string targetId, int amount)
        {
            Quest quest = questDatabase.GetQuest(questId);
            QuestProgress progress = questData.questProgresses.Find(qp => qp.questId == questId);

            if (quest == null || progress == null) return;

            foreach (var objective in quest.objectives)
            {
                if (objective.actionType.Equals(actionType) 
                    && (objective.targetId == targetId || objective.targetId == "any"))
                {
                    if (progress.objectiveProgress.TryGetValue(objective.objectiveId, out int currentProgress))
                    {
                        progress.objectiveProgress[objective.objectiveId] = currentProgress + amount;
                    }
                }
            }

            CheckQuestCompletion(progress, quest);
        }

        public bool CheckQuestCompleted(Quest quest)
        {
            var questProgress = GetQuestProgress(quest.questID);

            bool allCompleted = quest.objectives.TrueForAll(objective =>
            {
                return questProgress.objectiveProgress.TryGetValue(objective.objectiveId, out int progress)
                    && progress >= objective.requiredAmount;
            });
            
            // 퀘스트가 아직 완료가 안된 상태에서 퀘스타가 완료되면
            if (allCompleted && !questProgress.isCompleted)
            {
                return true;
            }
            return false;
        }

        private void CheckQuestCompletion(QuestProgress questProgress, Quest quest)
        {
            bool allCompleted = quest.objectives.TrueForAll(objective =>
            {
                return questProgress.objectiveProgress.TryGetValue(objective.objectiveId, out int progress)
                    && progress >= objective.requiredAmount;
            });

            if (allCompleted && !questProgress.isCompleted)
            {
                questProgress.isCompleted = true;

                QuestEvents.QuestCompleted(questProgress.questId);
                Debug.Log($"Quest {quest.questName} completed!");
            }
        }

        public void RemoveQuest(string questId)
        {
            Quest quest = questDatabase.GetQuest(questId);
            if (quest == null)
            {
                Debug.LogWarning($"Attempted to remove non-existent quest with ID: {questId}");
                return;
            }

            // Remove from active quests if it's active
            activeQuestIds.Remove(questId);

            // Remove quest progress
            questData.questProgresses.RemoveAll(qp => qp.questId == questId);

            Debug.Log($"Quest {questId} has been removed.");

            // You might want to trigger an event here to notify other systems
            // For example: QuestEvents.QuestRemoved(questId);
            QuestEvents.QuestRemoved(questId);

        }

        // Call this method when you want to save the game
        public void SaveQuestData()
        {
            gameDatas.SaveDataLocal();
        }

        // This method will be called when the game loads
        public void LoadQuestData()
        {
            gameDatas.LoadDataLocal();
            InitializeQuestData();
        }
    }

    public static class QuestEvents
    {
        public static event Action<QuestType, string, int> OnActionPerformed;
        public static event Action<string> OnQuestActivated;
        public static event Action<string> OnQuestCompleted;
        public static event Action<string> OnQuestRemoved;

        public static void ReportAction(QuestType actionType, string targetId, int amount = 1)
        {
            OnActionPerformed?.Invoke(actionType, targetId, amount);
        }

        public static void QuestActivated(string questId)
        {
            OnQuestActivated?.Invoke(questId);
        }

        public static void QuestRemoved(string questId)
        {
            OnQuestRemoved?.Invoke(questId);
        }

        public static void QuestCompleted(string questId)
        {
            OnQuestCompleted?.Invoke(questId);
        }
    }

    public static class ItemActions
    {
        public static void ReportItemCollected(string itemId, int amount = 1)
        {
            QuestEvents.ReportAction(QuestType.CollectItems, itemId, amount);
        }
    }
}

