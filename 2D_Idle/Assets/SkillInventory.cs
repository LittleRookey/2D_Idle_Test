using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

//��ų �κ��丮
//- �÷��̾ ���� �ִ� ��ų���� ����

//��� ��ų������ �����ִ� �Ŵ����� �ʿ�
//- �÷��̾ �����ִ� ��ų���̵� �ε�, 
//- ���̵�� �Ŵ������� ��ų�� ����� ������ ���õ��� ���߱�
//- ���������� ��ų�κ��丮�� �־��ֱ�
//- ��ų�κ��丮���� ���������� ������ϴ� �ڷᱸ���� ����Ʈ�� ���
public class SkillInventory : MonoBehaviour
{
    public static SkillInventory Instance;

    [SerializeField] private List<Skill> skillInventory;
    [SerializeField] private StatContainer playerStat;

    public UnityEvent<Skill> OnAddSkill;

    private Dictionary<string, PassiveSkill> passives;
    private Dictionary<string, ActiveSkill> actives;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        passives = new Dictionary<string, PassiveSkill>();
        actives = new Dictionary<string, ActiveSkill>();
    }

    [Button("AddSkill")]
    public void AddToInventory(Skill skill)
    {
        skillInventory.Add(skill);
        if (skill is PassiveSkill passive)
        {
            passive.Initialize();
            passive.OnSkillLevelUp.AddListener(playerStat.OnEquipPassive);
            playerStat.OnEquipPassive(passive);
            if (!passives.ContainsKey(passive.skillName))
            {
                passives.Add(passive.skillName, passive);
            }
        }
        else if (skill is ActiveSkill active)
        {
            //active.Initialize();

            if (!actives.ContainsKey(active.skillName))
            {
                actives.Add(active.skillName, active);
            }
        }
        OnAddSkill?.Invoke(skill);
    }

    public List<PassiveSkill> GetPassives()
    {
        
        return passives.Values.ToList();
    }

    public List<ActiveSkill> GetActives()
    {
        return actives.Values.ToList();
    }

    public Skill GetSkill(string skillName)
    {
        return skillInventory.Find((Skill skill) => skill.skillName.Equals(skillName));
    }

}
