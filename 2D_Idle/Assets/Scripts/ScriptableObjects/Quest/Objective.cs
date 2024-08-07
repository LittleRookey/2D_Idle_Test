using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.Quest
{
    [System.Serializable]
    public class Objective
    {
        public string objectiveId;
        public string description;
        public int requiredAmount;
        public string targetId; // E.g., enemy type ID, item ID, location ID
        public string actionType; // E.g., "EnemyKilled", "ItemCollected", "LocationReached"
    }

    [System.Serializable]
    public class ObjectiveProgress
    {
        public string objectiveId;
        public int currentAmount;

        public ObjectiveProgress(string id)
        {
            objectiveId = id;
            currentAmount = 0;
        }
    }
}

