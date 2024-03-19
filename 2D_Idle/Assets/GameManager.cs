using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InGameEventManager inGameEventManager;
    [SerializeField] private MapManager mapManager;

    
    private InGameEvent currentEvent;



    private void StartEvent()
    {
        currentEvent = inGameEventManager.GetEvent();
        mapManager.SetDestination(currentEvent.eventPosition);
    }

    
}
