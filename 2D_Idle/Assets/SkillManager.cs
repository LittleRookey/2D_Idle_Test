using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Skill;
using Sirenix.OdinInspector;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;

    
    //public List<Skill> skillsDB; // 기본적인 스킬들이 내장되어잇음

    [SerializeField, ShowInInspector] private Dictionary<string, Skill> skillDict;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        //skillDict = new Dictionary<string, Skill>();
        //for (int i = 0; i < skills.Count; i++)
        //{
        //    skillDict.Add(skills[i].skillName, skills[i]);
        //}
    }

    // Start is called before the first frame update
    void Start()
    {
        SkillInventory.Instance.AddToInventory(GetSkill("삼재검법"));
        SkillInventory.Instance.AddToInventory(GetSkill("더블슬래쉬")); 
    }

    public Skill GetSkill(string skillID)
    {
        return skillDict[skillID];
    }

}
