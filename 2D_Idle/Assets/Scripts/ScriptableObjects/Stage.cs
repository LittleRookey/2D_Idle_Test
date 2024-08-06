using Litkey.Weather;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[InlineEditor]
[CreateAssetMenu(menuName ="Litkey/Stage")]
public class Stage : ScriptableObject
{
    public int index;
    [TextArea]
    public string stageTitle;

    [EnumToggleButtons]
    public eTileType tileType;
    [InlineEditor]
    public Health Monster;

    public bool HasBoss;

    [ShowIf("HasBoss", true)]
    public Health Boss;
    [ShowIf("Monster", true)]
    public int enemyCount;
    [Range(0.1f, 100f)]
    public float difficultyRate = 1f;

    public Weather weather;
    public bool IsCompleted = false;
    public bool IsLocked = true;

    public void Unlock()
    {
        IsLocked = false;
        IsCompleted = false;
        difficultyRate = 1f + 0.03f * index;
    }

    public void Complete()
    {
        IsCompleted = true;
    }

}

public enum eMissionType
{
    일정적처치,
    수집,
    호위,
    생존,
    엘리트몬스터퇴치
}