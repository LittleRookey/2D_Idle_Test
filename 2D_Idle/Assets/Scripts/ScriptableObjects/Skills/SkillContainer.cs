using Litkey.Character.Cooldowns;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Litkey.Skill;

public class SkillContainer : MonoBehaviour
{
    public List<PassiveSkill> passiveSkills;
    public List<ActiveSkill> activeSkills;

    private PlayerStatContainer statContainer;

    private CooldownSystem cooldownSystem;
    TopdownPlayerController player;

    public ActiveSkill[] equippedActiveSkills;
    public int activeslotNumbers = 5;
    private void Awake()
    {
        player = GetComponent<TopdownPlayerController>();
        statContainer = GetComponent<PlayerStatContainer>();
        cooldownSystem = GetComponent<CooldownSystem>();
    }

    private void Start()
    {
        UpdateObtainedSkills();    
    }

    public void UpdateObtainedSkills()
    {
        passiveSkills = SkillInventory.Instance.GetPassives();
        activeSkills = SkillInventory.Instance.GetActives();
    }

    public ActiveSkill FindUsableSkill()
    {

        var health = player.GetTarget();
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

    public void ApplyPassiveEffects(Health target)
    {
        foreach (var skill in passiveSkills)
        {
            //skill.ApplyEffect(target);
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
