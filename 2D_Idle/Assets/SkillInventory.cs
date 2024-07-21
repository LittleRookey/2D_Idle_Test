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
        Debug.Log($"스킬 {skill.skillName}가 성공적으로 추가돼었습니다");
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
            WarningMessageInvoker.Instance.ShowMessage($"스킬 슬롯이 꽉 찼습니다");
            return;
        }
        if (slotIndex >= equippedActiveSkills.Length || slotIndex < 0)
        {
            WarningMessageInvoker.Instance.ShowMessage($"스킬 슬롯{slotIndex}은 없습니다");
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
        if (equippedActiveSkills != null && equippedActiveSkills.Length == activeSkillSlots)
        {
            Debug.Log("스킬 슬롯 이미 로드 완료");
            return;
        }
        Debug.Log("스킬 슬롯 로드 시작...");
        equippedActiveSkills = new ActiveSkill[activeSkillSlots];
        foreach (var kValue in playerData.equippedActiveSkills)
        {
            if (kValue.Value.Equals(string.Empty)) continue;
            equippedActiveSkills[kValue.Key] = GetActiveSkill(kValue.Value);
        }
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
