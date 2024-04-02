using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Utility;

[CreateAssetMenu(fileName = "EventTable", menuName = "Litkey/Quest/EventTable")]
public class EventTable : ScriptableObject
{
    [SerializeField] private WeightedRandomPicker<InGameEvent> eventTable;
    [SerializeField] private EventWeight[] eventWeights;

    private void OnEnable()
    {
        eventTable = null;
        eventTable = new WeightedRandomPicker<InGameEvent>();
        for (int i = 0; i < eventWeights.Length; i++)
        {
            eventTable.Add(eventWeights[i].Event, eventWeights[i].weight);

        }
    }

    public InGameEvent GetRandomEvent()
    {
        var events = eventTable.GetRandomPick();
        events.eventPosition = new Vector2(Random.Range(MapManager.mapMinX, MapManager.mapMaxX), Random.Range(MapManager.mapMinY, MapManager.mapMaxY));
        return events;
    }
}

