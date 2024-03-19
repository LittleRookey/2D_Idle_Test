using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InGameEventManager : MonoBehaviour
{
    public Queue<InGameEvent> InGameEvents;
    [SerializeField] private EventTable eventTable;

    private void Awake()
    {
        InGameEvents = new Queue<InGameEvent>();
    }
    public InGameEvent AddEvent()
    {
        var newEvent = eventTable.GetRandomEvent();
        InGameEvents.Enqueue(newEvent);
        return newEvent;
    }

    public InGameEvent GetEvent()
    {
        if (InGameEvents.TryDequeue(out InGameEvent res))
        {
            return res;
        }
        var newEvent = AddEvent();
        return newEvent;
    }
    

    public void PerformEvent(InGameEvent iEvent)
    {
        switch(iEvent.eventType)
        {
            case eEventType.일반몬스터토벌:
                break;
            case eEventType.엘리트몬스터:
                break;
            case eEventType.보스몬스터:
                break;
            case eEventType.몬스터토벌:
                break;

        }
    }
}

