using Litkey.Weather;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName ="Litkey/Stage")]
public class Stage : ScriptableObject
{
    public eMissionType missionType;
    [InlineEditor]
    public MonsterTable monsterTable;
    [Range(0.1f, 100f)]
    public float difficultyRate = 1f;
    [InlineEditor]
    public Weather weather;
    [InlineEditor]
    public Shop shop;
    public eResourceType appearningResourceType;
    public float TimeLimit;
    
    // 보상

    // 맵
    public GameObject mapPrefab;

}

public enum eMissionType
{
    일정적처치,
    수집,
    호위,
    생존,
    엘리트몬스터퇴치
}