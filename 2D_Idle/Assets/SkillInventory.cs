using Litkey.Interface;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Litkey.Skill;
using Litkey.Utility;


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
    [SerializeField] private SkillDatabase skillDB;

    public ActiveSkill[] equippedActiveSkills;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        passives = new Dictionary<string, PassiveSkill>();
        actives = new Dictionary<string, ActiveSkill>();

        gameDatas.OnGameDataLoaded.AddListener(Load);
    }

    [Button("AddSkill")]
    public void AddToInventory(Skill skill)
    {
        
        skill.Level.Init();

        bool alreadyContainsSkill = true;

        if (skill is PassiveSkill passive)
        {
            if (!passives.ContainsKey(passive.skillName))
            {
                passive.SetInitialState();
                passive.OnSkillLevelUp.AddListener(playerStat.OnEquipPassive);
                
                playerStat.OnEquipPassive(passive);
                
                passives.Add(passive.skillName, passive);
                alreadyContainsSkill = false;
            }
        }
        else if (skill is ActiveSkill active)
        {
            //active.Initialize();

            if (!actives.ContainsKey(active.skillName))
            {
                actives.Add(active.skillName, active);
                alreadyContainsSkill = false;
            }
        }

        if (!alreadyContainsSkill)
        {
            skillInventory.Add(skill);
        }

        OnAddSkill?.Invoke(skill);
        Debug.Log($"��ų {skill.skillName}�� ���������� �߰��ž����ϴ�");
        Save();

    }

    private bool IsSkillSlotsFull()
    {
        bool isNotFull = false;
        for (int i = 0; i < equippedActiveSkills.Length; i++)
        {
            isNotFull = isNotFull || equippedActiveSkills[i] == null;
        }
        return isNotFull;
    }

    public void EquipActiveSkill(ActiveSkill activeSkill, int slotIndex)
    {
        if (IsSkillSlotsFull())
        {
            WarningMessageInvoker.Instance.ShowMessage($"��ų ������ �� á���ϴ�");
            return;
        }
        if (slotIndex >= equippedActiveSkills.Length || slotIndex < 0)
        {
            WarningMessageInvoker.Instance.ShowMessage($"��ų ����{slotIndex}�� �����ϴ�");
            return;
        }

        equippedActiveSkills[slotIndex] = activeSkill;
        PlayerData playerData = gameDatas.dataSettings.playerData;
        playerData.SaveEquippedSkills(slotIndex, activeSkill);
    }

    public void UnEquipActiveSkill(int slotIndex)
    {
        equippedActiveSkills[slotIndex] = null;
        PlayerData playerData = gameDatas.dataSettings.playerData;
        playerData.SaveEquippedSkills(slotIndex, null);
    }

    public List<PassiveSkill> GetPassives()
    {
        if (passives.Values.Count <= 0) return null;
        return passives.Values.ToList();
    }

    public List<ActiveSkill> GetActives()
    {
        if (actives.Values.Count <= 0) return null;

        return actives.Values.ToList();
    }

    public Skill GetSkill(string skillName)
    {
        return skillInventory.Find((Skill skill) => skill.skillName.Equals(skillName));
    }

    public ActiveSkill GetActiveSkill(string skillName)
    {
        return actives.Values.FirstOrDefault((ActiveSkill skill) => skill.skillName.Equals(skillName));
    }

    public Skill LoadSkillFromDB(string skillName)
    {
        return skillDB.GetSkillByID(skillName);
    }

    // ���̺� �ؾ��ҋ�:
    // 1. ��ų ����ġ �������
    // 2. ��ų�� ������ ������
    // 3. �� ��ų�� �������
    public void Save()
    {

        PlayerData playerData = gameDatas.dataSettings.playerData;
        
        
        playerData.SaveSkills(skillInventory);
        playerData.SaveEquippedActiveSkills(this);

        gameDatas.SaveDataLocal();
    }

    // ���� �κ��丮�� ������ �κ��丮�� add�ϱ�
    // �κ��丮�� ������ ��ų������ ������Ʈ

    public void Load()
    {
        if (skillInventory == null) skillInventory = new List<Skill>();
        PlayerData playerData = gameDatas.dataSettings.playerData;

        playerData = gameDatas.dataSettings.playerData;

        var skillDatas = playerData.GetSkillDatas();

        // �κ��丮�� ������ Add�ϱ�
        for (int i = 0; i < skillDatas.Count; i++)
        {
            var skill = LoadSkillFromDB(skillDatas[i].skillName);
            Debug.Log($"��ų {skill.skillName}�� DB���� �����Խ��ϴ�");

            Debug.Log($"��ų {skill.skillName}�� �κ��丮�� �߰���...");
            AddToInventory(skill);

        }

        // �κ��丮�� �ִ� ��ų���� �����͸� ������Ʈ
        UpdateSkills(skillDatas);
        Debug.Log("��ų ������ �ε� �Ϸ�");

        int activeSkillSlots = playerData.unlockedActiveSkillSlots;
        if (equippedActiveSkills != null && equippedActiveSkills.Length == activeSkillSlots)
        {
            Debug.Log("��ų ���� �̹� �ε� �Ϸ�");
            return;
        }
        Debug.Log("��ų ���� �ε� ����...");
        equippedActiveSkills = new ActiveSkill[activeSkillSlots];
        foreach (var kValue in playerData.equippedActiveSkills)
        {
            if (kValue.Value.Equals(string.Empty)) continue;
            equippedActiveSkills[kValue.Key] = GetActiveSkill(kValue.Value);
        }
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
