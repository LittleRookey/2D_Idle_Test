using Litkey.Interface;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Litkey.Skill;
using Litkey.Utility;


//스킬 인벤토리
//- 플레이어가 갖고 있는 스킬들을 나열

//모든 스킬정보를 갖고있는 매니저가 필요
//- 플레이어가 갖고있는 스킬아이디를 로드, 
//- 아이디로 매니저에서 스킬을 갖고와 레벨과 숙련도를 맞추기
//- 마지막으로 스킬인벤토리에 넣어주기
//- 스킬인벤토리에는 수량제한이 없어야하니 자료구조중 리스트를 사용
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
        Debug.Log($"스킬 {skill.skillName}가 성공적으로 추가돼었습니다");
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
            WarningMessageInvoker.Instance.ShowMessage($"스킬 슬롯이 꽉 찼습니다");
            return false;
        }
        if (slotIndex >= equippedActiveSkills.Length || slotIndex < 0)
        {
            WarningMessageInvoker.Instance.ShowMessage($"스킬 슬롯{slotIndex}은 없습니다");
            return false;
        }
        if (slotIndex >= gameDatas.dataSettings.playerData.unlockedActiveSkillSlots)
        {
            WarningMessageInvoker.Instance.ShowMessage($"스킬 슬롯{slotIndex}은 잠겨있습니다");
            return false;
        }

        PlayerData playerData = gameDatas.dataSettings.playerData;

        //만약 같은 스킬을 장착하려고 하면 장착이 해제된다. 
        if (equippedActiveSkills[slotIndex].IsSameSkill(activeSkill))
        {
            equippedActiveSkills[slotIndex].UnEquip();
            Save();
            OnSkillEquipped.Invoke();
            return true;
        }

        // 이미 장착돼있는스킬을 다른 비어있는 스킬칸에다가 장착하면 스킬이 그 칸으로 옮겨 장착된다
        // 만약 해당 스킬칸이 비어있지않고 스킬이 있으면 Swap하기
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
            // 평범하게 해당 스킬칸이 빈 스킬칸이면 그대로 장착
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

    // 세이브 해야할떄:
    // 1. 스킬 경험치 얻었을때
    // 2. 스킬이 레벨업 했을떄
    // 3. 새 스킬을 얻었을때
    public void Save()
    {

        PlayerData playerData = gameDatas.dataSettings.playerData;
        
        
        playerData.SaveSkills(skillInventory);
        playerData.SaveEquippedActiveSkills(this);

        gameDatas.SaveDataLocal();
    }

    // 먼저 인벤토리에 없으면 인벤토리에 add하기
    // 인벤토리에 있으면 스킬데이터 업데이트

    public void Load()
    {
        if (skillInventory == null) skillInventory = new List<Skill>();
        PlayerData playerData = gameDatas.dataSettings.playerData;

        playerData = gameDatas.dataSettings.playerData;

        var skillDatas = playerData.GetSkillDatas();

        // 인벤토리에 없으면 Add하기
        for (int i = 0; i < skillDatas.Count; i++)
        {
            var skill = LoadSkillFromDB(skillDatas[i].skillName);
            Debug.Log($"스킬 {skill.skillName}를 DB에서 가져왔습니다");

            Debug.Log($"스킬 {skill.skillName}를 인벤토리에 추가중...");
            AddToInventory(skill);

        }

        // 인벤토리에 있는 스킬들의 데이터를 업데이트
        UpdateSkills(skillDatas);
        Debug.Log("스킬 데이터 로드 완료");

        int activeSkillSlots = playerData.unlockedActiveSkillSlots;
        //if (equippedActiveSkills != null && equippedActiveSkills.Length == activeSkillSlots)
        //{
        //    Debug.Log("스킬 슬롯 이미 로드 완료");
        //    return;
        //}
        Debug.Log("스킬 슬롯 로드 시작...");
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

    // 이미 인벤토리에 있는 스킬들의 데이터를 업데이트
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
