using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Litkey.Skill;

[InlineEditor]
[CreateAssetMenu(menuName = "Litkey/SkillDB")]
public class SkillDatabase : SerializedScriptableObject
{
    public Dictionary<string, Skill> skillDB;


    [Button("AddToDB", Style = ButtonStyle.FoldoutButton)]
    public void AddItemToDB(Skill skill)
    {
        if (skillDB.ContainsKey(skill.skillName))
        {
            Debug.LogError($"There is already an skill with name {skill.skillName} in Skill Database");
            return;
        }
        skillDB.Add(skill.skillName, skill);
    }

    public Skill GetSkillByID(string skillName)
    {
        if (!skillDB.ContainsKey(skillName))
        {
            Debug.LogError($"There is no such Skill with ID {skillName} in Skill Database");
            return null;
        }

        return skillDB[skillName];
    }

}
