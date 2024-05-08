using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    public Queue<InGameEvent> InGameEvents;
    [SerializeField] private EventTable eventTable;

    private void Awake()
    {
        InGameEvents = new Queue<InGameEvent>();
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

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

