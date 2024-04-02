using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;
    [SerializeField] private MapManager mapManager;

    
    private InGameEvent currentEvent;



    private void StartEvent()
    {
        currentEvent = questManager.GetEvent();
        mapManager.SetDestination(currentEvent.eventPosition);
    }

    
}
