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
    [SerializeField] private float skillInterval; // 스킬 사용사이의 쿨타임

    CountdownTimer countdownTimer;

    private void Awake()
    {
        if (!isEnemy)
        {
            player = GetComponent<PlayerController>();
        }

        statContainer = GetComponent<StatContainer>();
        cooldownSystem = GetComponent<CooldownSystem>();
        passiveSkills = new List<PassiveSkill>();
        activeSkills = new List<ActiveSkill>();
        countdownTimer = new CountdownTimer(skillInterval);
    }

    private void Start()
    {
        if (!isEnemy)
            UpdateObtainedSkills();    
    }


    public void UpdateObtainedSkills()
    {
        activeSkills.Clear();
        passiveSkills.Clear();
        // 패시브랑 액티브 스킬들 장착시키기
        for (int i = 0; i < SkillInventory.Instance.maxSkillEquipSlotNumber; i++)
        {
            var equipSlot = SkillInventory.Instance.GetSkillEquipSlot(i);
            if (equipSlot != null)
            {
                activeSkills.Add(equipSlot.EquippedSkill);
            }
        }
        passiveSkills = SkillInventory.Instance.GetPassives();
        //passiveSkills = SkillInventory.Instance.GetPassives();
        //activeSkills = SkillInventory.Instance.GetActives();
    }

    public ActiveSkill FindUsableSkill()
    {
        if (countdownTimer.IsRunning) return null;

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
        countdownTimer.Tick(Time.deltaTime);
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
        countdownTimer.Start();
        //skill.Use(target);
    }
}
