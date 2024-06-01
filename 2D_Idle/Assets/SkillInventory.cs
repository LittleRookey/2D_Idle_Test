using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

//스킬 인벤토리
//- 플레이어가 갖고 있는 스킬들을 나열

//모든 스킬정보를 갖고있는 매니저가 필요
//- 플레이어가 갖고있는 스킬아이디를 로드, 
//- 아이디로 매니저에서 스킬을 갖고와 레벨과 숙련도를 맞추기
//- 마지막으로 스킬인벤토리에 넣어주기
//- 스킬인벤토리에는 수량제한이 없어야하니 자료구조중 리스트를 사용
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
