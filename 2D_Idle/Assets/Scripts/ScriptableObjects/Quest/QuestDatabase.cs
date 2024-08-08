using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Litkey.Quest
{
    [InlineEditor]
    [CreateAssetMenu(menuName = "Litkey/Quest/QuestDatabase")]
    public class QuestDatabase : SerializedScriptableObject
    {
        public List<Quest> quests;
        private Dictionary<string, Quest> questLookup;

        private void OnEnable()
        {
            InitializeQuestLookup();
        }

        private void InitializeQuestLookup()
        {
            questLookup = new Dictionary<string, Quest>();
            foreach (var quest in quests)
            {
                questLookup[quest.questID] = quest;
            }
        }

        public Quest GetQuest(string questId)
        {
            return questLookup.TryGetValue(questId, out Quest quest) ? quest : null;
        }
    }
}

