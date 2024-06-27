using Litkey.Character.Cooldowns;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Litkey.Skill;


public class SkillContainer : MonoBehaviour
{
    public bool isEnemy = true;
    public List<PassiveSkill> passiveSkills;
    public List<ActiveSkill> activeSkills;

    private StatContainer statContainer;

    private CooldownSystem cooldownSystem;
    PlayerController player;

    [HideInInspector] public ActiveSkill[] equippedActiveSkills;
    public int activeslotNumbers = 5;
    private void Awake()
    {
        if (!isEnemy)
        {
            player = GetComponent<PlayerController>();
        }

        statContainer = GetComponent<StatContainer>();
        cooldownSystem = GetComponent<CooldownSystem>();
    }

    private void Start()
    {
        if (!isEnemy)
            UpdateObtainedSkills();    
    }

    public void UpdateObtainedSkills()
    {
        passiveSkills = SkillInventory.Instance.GetPassives();
        activeSkills = SkillInventory.Instance.GetActives();
    }

    public ActiveSkill FindUsableSkill()
    {
        ActiveSkill bestSkill = null;

        foreach (var skill in activeSkills)
        {
            if (cooldownSystem.IsOnCooldown(skill.skillName))
                continue;

            return skill;
        }

        return bestSkill;
    }

    private void Update()
    {

    }

    public void EquipPassiveEffects(StatContainer target)
    {
        foreach (var skill in passiveSkills)
        {
            //skill.ApplyEffect(target);
            //skill.ApplyEffect
        }
    }

    [Button("UseSkill")]
    public void UseActiveSkill(ActiveSkill skill, Health target)
    {
        skill.Use(statContainer, target);
        cooldownSystem.PutOnColdown(skill);
        //skill.Use(target);
    }
}
