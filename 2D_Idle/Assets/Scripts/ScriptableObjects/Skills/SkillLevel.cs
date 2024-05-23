using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SkillLevels", menuName = "Litkey/SkillLevels")]
public class SkillLevel : UnitLevel
{
    [Header("Skill Settings")]
    public string skillName;
}
