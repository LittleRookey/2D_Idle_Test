using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��ų �κ��丮
//- �÷��̾ ���� �ִ� ��ų���� ����

//��� ��ų������ �����ִ� �Ŵ����� �ʿ�
//- �÷��̾ �����ִ� ��ų���̵� �ε�, 
//- ���̵�� �Ŵ������� ��ų�� ����� ������ ���õ��� ���߱�
//- ���������� ��ų�κ��丮�� �־��ֱ�
//- ��ų�κ��丮���� ���������� ������ϴ� �ڷᱸ���� ����Ʈ�� ���
public class SkillInventory : MonoBehaviour
{
    [SerializeField] private List<Skill> skillInventory;
    [SerializeField] private StatContainer playerStat;

    [Button("AddSkill")]
    public void AddToInventory(Skill skill)
    {
        skillInventory.Add(skill);
        if (skill is PassiveSkill passive)
        {
            passive.Initialize();
            playerStat.OnEquipPassive(passive);
        }
    }




}
