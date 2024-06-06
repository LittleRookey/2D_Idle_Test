using Litkey.Interface;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Litkey.Skill;

//��ų �κ��丮
//- �÷��̾ ���� �ִ� ��ų���� ����

//��� ��ų������ �����ִ� �Ŵ����� �ʿ�
//- �÷��̾ �����ִ� ��ų���̵� �ε�, 
//- ���̵�� �Ŵ������� ��ų�� ����� ������ ���õ��� ���߱�
//- ���������� ��ų�κ��丮�� �־��ֱ�
//- ��ų�κ��丮���� ���������� ������ϴ� �ڷᱸ���� ����Ʈ�� ���
public class SkillInventory : MonoBehaviour, ILoadable, ISavable
{
    public static SkillInventory Instance;

    [SerializeField] private List<Skill> skillInventory;
    [SerializeField] private StatContainer playerStat;

    public UnityEvent<Skill> OnAddSkill;

    private Dictionary<string, PassiveSkill> passives;
    private Dictionary<string, ActiveSkill> actives;

    [SerializeField] private GameDatas gameDatas;
    PlayerData playerData;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        passives = new Dictionary<string, PassiveSkill>();
        actives = new Dictionary<string, ActiveSkill>();
    }

    [Button("AddSkill")]
    public void AddToInventory(Skill skill)
    {
        skillInventory.Add(skill);
        if (skill is PassiveSkill passive)
        {
            if (!passives.ContainsKey(passive.skillName))
            {
                passive.Initialize();
                passive.OnSkillLevelUp.AddListener(playerStat.OnEquipPassive);
                
                playerStat.OnEquipPassive(passive);
                
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

    // ���̺� �ؾ��ҋ�:
    // 1. ��ų ����ġ �������
    // 2. ��ų�� ������ ������
    // 3. �� ��ų�� �������
    public void Save()
    {
        if (playerData == null) playerData = gameDatas.dataSettings.playerData;
        
        this.playerData.SaveSkills(skillInventory);

        gameDatas.SaveDataLocal();
    }

    // ���� �κ��丮�� ������ �κ��丮�� add�ϱ�
    // �κ��丮�� ������ ��ų������ ������Ʈ

    public void Load()
    {
        if (skillInventory == null) skillInventory = new List<Skill>();

        playerData = gameDatas.dataSettings.playerData;

        var skillDatas = playerData.GetSkillDatas();

        // �κ��丮�� ������ Add�ϱ�
        for (int i = 0; i < skillDatas.Count; i++)
        {
            var skill = GetSkill(skillDatas[i].skillName);
            if (skill == null)
            {
                AddToInventory(skill);
            }
        }

        // �κ��丮�� �ִ� ��ų���� �����͸� ������Ʈ
        UpdateSkills(skillDatas);
    }

    // �̹� �κ��丮�� �ִ� ��ų���� �����͸� ������Ʈ
    private void UpdateSkills(List<SkillData> skillDatas)
    {
        for (int i = 0; i < skillDatas.Count; i++)
        {
            UpdateSkill(skillDatas[i]);
        }
    }

    private void UpdateSkill(SkillData skillData)
    {
        var skill = GetSkill(skillData.skillName);
        if (skill != null)
        {
            skill.SetData(skillData);
        }
        else
        {
            Debug.LogError("Skill not found!: " + skillData.skillName);
        }
    }
}
