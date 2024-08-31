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

    public SkillEquipSlot[] equippedActiveSkills;

    [SerializeField] public int maxSkillEquipSlotNumber { get; private set; } = 5;

    [HideInInspector] public UnityEvent OnSkillInventoryLoaded;
    [HideInInspector] public UnityEvent OnSkillEquipped;

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

        //if (skill is PassiveSkill passive)
        //{
        //    if (!passives.ContainsKey(passive.skillName))
        //    {
        //        passive.SetInitialState();
        //        passive.OnSkillLevelUp.AddListener(playerStat.OnEquipPassive);
                
        //        playerStat.OnEquipPassive(passive);
                
        //        passives.Add(passive.skillName, passive);
        //        alreadyContainsSkill = false;
        //    }
        //}
        //else 
        if (skill is ActiveSkill active)
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
        //Save();

    }

    public SkillEquipSlot GetSkillEquipSlot(int index)
    {
        if (index >= equippedActiveSkills.Length) return null;
        if (index < 0) return null;
        if (equippedActiveSkills[index].IsLocked) return null;
        //if (!equippedActiveSkills[index].IsEquipped) return null;

        return equippedActiveSkills[index];
    }

    private bool IsSkillSlotsFull()
    {
        for (int i = 0; i < equippedActiveSkills.Length; i++)
        {
            if (!equippedActiveSkills[i].IsEquipped)
            {
                return false; // If any slot is not equipped, the slots are not full
            }
        }
        return true; // All slots are equipped
    }

    public bool EquipActiveSkill(ActiveSkill activeSkill, int slotIndex)
    {
        if (IsSkillSlotsFull())
        {
            WarningMessageInvoker.Instance.ShowMessage($"��ų ������ �� á���ϴ�");
            return false;
        }
        if (slotIndex >= equippedActiveSkills.Length || slotIndex < 0)
        {
            WarningMessageInvoker.Instance.ShowMessage($"��ų ����{slotIndex}�� �����ϴ�");
            return false;
        }
        if (slotIndex >= gameDatas.dataSettings.playerData.unlockedActiveSkillSlots)
        {
            WarningMessageInvoker.Instance.ShowMessage($"��ų ����{slotIndex}�� ����ֽ��ϴ�");
            return false;
        }

        PlayerData playerData = gameDatas.dataSettings.playerData;

        //���� ���� ��ų�� �����Ϸ��� �ϸ� ������ �����ȴ�. 
        if (equippedActiveSkills[slotIndex].IsSameSkill(activeSkill))
        {
            equippedActiveSkills[slotIndex].UnEquip();
            Save();
            OnSkillEquipped.Invoke();
            return true;
        }

        // �̹� �������ִ½�ų�� �ٸ� ����ִ� ��ųĭ���ٰ� �����ϸ� ��ų�� �� ĭ���� �Ű� �����ȴ�
        // ���� �ش� ��ųĭ�� ��������ʰ� ��ų�� ������ Swap�ϱ�
        int currentEquippedIndex = GetEquippedSlotIndexWithSkill(activeSkill);
        if (currentEquippedIndex != -1)
        {
            if (!equippedActiveSkills[slotIndex].IsEquipped)
            {
                // Move the skill to the new slot
                equippedActiveSkills[currentEquippedIndex].UnEquip();
                equippedActiveSkills[slotIndex].Equip(activeSkill);
            }
            else
            {
                // Swap skills
                ActiveSkill skillInTargetSlot = equippedActiveSkills[slotIndex].EquippedSkill;
                equippedActiveSkills[slotIndex].Equip(activeSkill);
                equippedActiveSkills[currentEquippedIndex].Equip(skillInTargetSlot);
            }
        }
        else
        {
            // ����ϰ� �ش� ��ųĭ�� �� ��ųĭ�̸� �״�� ����
            equippedActiveSkills[slotIndex].Equip(activeSkill);
        }
        OnSkillEquipped.Invoke();
        Save();
        return true;
    }
    private int GetEquippedSlotIndexWithSkill(ActiveSkill skill)
    {
        for (int i = 0; i < equippedActiveSkills.Length; i++)
        {
            if (equippedActiveSkills[i].IsLocked) continue;
            if (!equippedActiveSkills[i].IsEquipped) continue;

            if (equippedActiveSkills[i].IsSameSkill(skill))
            {
                return i;
            }
        }
        return -1;
    }
    private bool IsAlreadyEquipped(ActiveSkill activeSkill)
    {
        for (int i = 0; i < equippedActiveSkills.Length; i++)
        {
            if (equippedActiveSkills[i].IsLocked) continue;
            if (!equippedActiveSkills[i].IsEquipped) continue;

            if (equippedActiveSkills[i].IsSameSkill(activeSkill)) 
            {
                return true;
            }
        }
        return false;
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
        //if (equippedActiveSkills != null && equippedActiveSkills.Length == activeSkillSlots)
        //{
        //    Debug.Log("��ų ���� �̹� �ε� �Ϸ�");
        //    return;
        //}
        Debug.Log("��ų ���� �ε� ����...");
        equippedActiveSkills = new SkillEquipSlot[maxSkillEquipSlotNumber];
        for (int i = 0; i < equippedActiveSkills.Length; i++)
        {
            equippedActiveSkills[i] = new SkillEquipSlot(i);
            if (i < activeSkillSlots)
            {
                equippedActiveSkills[i].Unlock();
            }
        }
        
        foreach (var kValue in playerData.equippedActiveSkills)
        {
            if (kValue.Value.Equals(string.Empty)) continue;
            equippedActiveSkills[kValue.Key].Equip(GetActiveSkill(kValue.Value));
        }
        OnSkillInventoryLoaded?.Invoke();
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

[System.Serializable]
public class SkillEquipSlot
{
    public bool IsLocked;
    public int Index { private set; get; }
    public ActiveSkill EquippedSkill;
    public bool IsEquipped => EquippedSkill != null;
    
    public SkillEquipSlot(int index)
    {
        IsLocked = true;
        Index = index;
        EquippedSkill = null;
    }

    public void Equip(ActiveSkill skill)
    {
        if (IsLocked) return;
        EquippedSkill = skill;
    }

    public void UnEquip()
    {
        EquippedSkill = null;
    }

    public void Lock() => IsLocked = true;
    public void Unlock() => IsLocked = false;

    public bool IsSameSkill(ActiveSkill skill)
    {
        if (EquippedSkill == null) return false;
        return EquippedSkill.skillName.Equals(skill.skillName);
    }
}
