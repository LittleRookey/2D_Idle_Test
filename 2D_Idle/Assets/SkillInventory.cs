using Litkey.Interface;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Litkey.Skill;

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

    // 세이브 해야할떄:
    // 1. 스킬 경험치 얻었을때
    // 2. 스킬이 레벨업 했을떄
    // 3. 새 스킬을 얻었을때
    public void Save()
    {
        if (playerData == null) playerData = gameDatas.dataSettings.playerData;
        
        this.playerData.SaveSkills(skillInventory);

        gameDatas.SaveDataLocal();
    }

    // 먼저 인벤토리에 없으면 인벤토리에 add하기
    // 인벤토리에 있으면 스킬데이터 업데이트

    public void Load()
    {
        if (skillInventory == null) skillInventory = new List<Skill>();

        playerData = gameDatas.dataSettings.playerData;

        var skillDatas = playerData.GetSkillDatas();

        // 인벤토리에 없으면 Add하기
        for (int i = 0; i < skillDatas.Count; i++)
        {
            var skill = GetSkill(skillDatas[i].skillName);
            if (skill == null)
            {
                AddToInventory(skill);
            }
        }

        // 인벤토리에 있는 스킬들의 데이터를 업데이트
        UpdateSkills(skillDatas);
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
