using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class QuestManager : MonoBehaviour
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
    

   
}

