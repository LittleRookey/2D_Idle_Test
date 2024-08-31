using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Litkey.Stat;
using Litkey.Skill;

[CreateAssetMenu(fileName = "New Monster Essence", menuName = "Litkey/InventorySystem/Monster Essence")]
public class MonsterEssence : ScriptableObject
{
    [BoxGroup("Basic Info")]
    [HorizontalGroup("Basic Info/Split")]
    [VerticalGroup("Basic Info/Split/Left"), PreviewField(100f)]
    public Sprite icon;

    [VerticalGroup("Basic Info/Split/Right")]
    public string essenceName;

    [VerticalGroup("Basic Info/Split/Right")]
    [TextArea(3, 5)]
    public string description;

    [TabGroup("Stats")]
    public List<MinMaxStatModifier> stats;

    [TabGroup("Skills")]
    public List<ActiveSkill> skills;

    [TabGroup("Passives")]
    public List<PassiveSkill> passives;

    [TabGroup("Restrictions")]
    [LabelText("Monster Level Slot Restriction")]
    public bool hasLevelRestriction;

    [TabGroup("Restrictions"), ShowIf("hasLevelRestriction")]
    [LabelText("Required Monster Level")]
    public int requiredLevel;

    [Button("Equip to Entity")]
    public void EquipToEntity(GameObject entity)
    {
        StatContainer statContainer = entity.GetComponent<StatContainer>();
        BuffReceiver buffReceiver = entity.GetComponent<BuffReceiver>();
        SkillInventory skillInventory = entity.GetComponent<SkillInventory>();

        if (statContainer != null)
        {
            statContainer.AddETCStat(stats);
        }

        if (buffReceiver != null)
        {
            // �нú� ���� �����
            // ������� �Ϲݰ��ݿ� ����ȿ���� ����Ű�� �нú갡 �ְ� ������Ű���� PlayerController�� Add��Ű�� �÷��̾ �ൿ�ҋ� �ߵ� ���Ѿ��Ѵ�. 
            for (int i = 0; i < passives.Count; i++)
            {
                
            }
        }
    }

    [Button("Unequip from Entity")]
    public void UnequipFromEntity(GameObject entity)
    {
        StatContainer statContainer = entity.GetComponent<StatContainer>();
        BuffReceiver buffReceiver = entity.GetComponent<BuffReceiver>();
        SkillInventory skillInventory = entity.GetComponent<SkillInventory>();

        if (statContainer != null)
        {
            statContainer.ClearStatModifiers();
        }

        if (buffReceiver != null)
        {
            // remove base stat
            statContainer.RemoveETCStat(stats);
            
            // remove additional stat 

        }

        if (skillInventory != null)
        {
            foreach (var skill in skills)
            {
                // Implement a method to remove skills from inventory if needed
                // skillInventory.RemoveFromInventory(skill);
            }

            foreach (var passive in passives)
            {
                // Implement a method to remove passives from inventory if needed
                // skillInventory.RemoveFromInventory(passive);
            }
        }
    }
}
