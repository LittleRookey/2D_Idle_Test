using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaManager : MonoBehaviour
{
    [SerializeField] private Area[] areas;

    private static Dictionary<eRegion, List<Area>> regions;
    private int regionLength;
    private void Awake()
    {
        areas = GetComponentsInChildren<Area>();
        regions = new Dictionary<eRegion, List<Area>>();
        foreach(eRegion reg in System.Enum.GetValues(typeof(eRegion)))
        {
            regions[reg] = new List<Area>();
        }
        
        for (int i = 0; i < areas.Length; i++)
        {
            regions[areas[i].region].Add(areas[i]);
            areas[i].TurnOutlineOff();
        }



        regionLength = regions.Count;
    }
    
    public static Area GetAreaOf(eRegion reg)
    {
        if (regions[reg].Count == 0) return null;
        var areaList = regions[reg];
        return areaList[Random.Range(0, areaList.Count)];
    }
}
