using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [SerializeField] private List<Skill> skills; // �⺻���� ��ų���� ����Ǿ�����

    private Dictionary<string, Skill> skillDict;

    private void Awake()
    {
        skillDict = new Dictionary<string, Skill>();
        for (int i = 0; i < skills.Count; i++)
        {
            skillDict.Add(skills[i].skillName, skills[i]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SkillInventory.Instance.AddToInventory(GetSkill("����˹�"));
        SkillInventory.Instance.AddToInventory(GetSkill("��������")); 
    }

    public Skill GetSkill(string skillName)
    {
        return skillDict[skillName];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
