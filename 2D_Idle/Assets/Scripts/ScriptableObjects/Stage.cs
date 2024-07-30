using Litkey.Weather;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName ="Litkey/Stage")]
public class Stage : ScriptableObject
{


}

public enum eMissionType
{
    일정적처치,
    수집,
    호위,
    생존,
    엘리트몬스터퇴치
}