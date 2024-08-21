using Litkey.Skill;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillQuickSlotUI : MonoBehaviour
{
    [SerializeField] private SkillInventory skillInventory;
    [SerializeField] private List<SkillCooldownSlot> quickSlots;
    [SerializeField] private SkillContainer skillContainer;
    private void Awake()
    {
        
        // Subscribe to the OnSkillInventoryLoaded event
        skillInventory.OnSkillInventoryLoaded.AddListener(UpdateQuickSlots);
    }

    private void OnEnable()
    {
        // Subscribe to the OnSkillEquipped event
        skillContainer.OnUseSkill.AddListener(UpdateCooldown);
        skillInventory.OnSkillEquipped.AddListener(UpdateQuickSlots);
    }

    private void OnDisable()
    {
        // Unsubscribe from the OnSkillEquipped event
        skillContainer.OnUseSkill.RemoveListener(UpdateCooldown);
        skillInventory.OnSkillEquipped.RemoveListener(UpdateQuickSlots);
    }

    private void UpdateCooldown(ActiveSkill activeSkill)
    {
        var equippedActiveSkills = skillInventory.equippedActiveSkills;
        for (int i = 0; i < equippedActiveSkills.Length && i < quickSlots.Count; i++)
        {
            if (equippedActiveSkills[i].IsLocked)
            {
                continue;
            }
            else if (!equippedActiveSkills[i].IsEquipped)
            {
                continue;
            }
            else
            {
                if (quickSlots[i].IsSameSkill(activeSkill))
                {
                    quickSlots[i].PutOnCooldown(activeSkill.CooldownDuration);
                }
            }
        }
    }
    private void UpdateQuickSlots()
    {
        var equippedActiveSkills = skillInventory.equippedActiveSkills;
        for (int i = 0; i < equippedActiveSkills.Length && i < quickSlots.Count; i++)
        {
            if (equippedActiveSkills[i].IsLocked)
            {
                quickSlots[i].SetLocked();
            }
            else if (!equippedActiveSkills[i].IsEquipped)
            {
                quickSlots[i].SetEmpty();
            }
            else
            {
                quickSlots[i].SetSlot(skillInventory.GetSkillEquipSlot(i).EquippedSkill);
            }
        }
    }
}
