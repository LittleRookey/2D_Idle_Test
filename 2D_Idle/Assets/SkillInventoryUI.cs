using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Litkey.Utility;
using Redcode.Pools;
using DG.Tweening;
using Litkey.Skill;
using UnityEngine.Events;

public class SkillInventoryUI : MonoBehaviour
{
    [SerializeField] private SkillInventory skillInventory;

    [Header("UIs")]
    [SerializeField] private RectTransform windowTransform;
 
    [SerializeField] private Button leftActiveSkillButton;
    [SerializeField] private Button rightPassiveSkillButton;
    
    // ��Ʈ��
    [SerializeField] private DOTweenAnimation leftBtnAnim;
    [SerializeField] private DOTweenAnimation rightBtnAnim;



    [SerializeField] private RarityColor skillRarity;

  
    [SerializeField] private RectTransform skillSlotUIParent;
    
    [SerializeField] private SkillSlotUI skillSlotUIPrefab;

    [SerializeField] private SkillInformationWindowUI skillInfoWindow;


    private List<SkillSlotUI> skillSlots;

    private Pool<SkillSlotUI> skillSlotPool;

    private Skill currentSkill;

    public bool DisableSkillWindowOnStart;

    [SerializeField] private List<SkillEquipSlotUI> skillEquipSlots;

    [SerializeField] private bool enterEquipSkillMode;

    private void Awake()
    {
        skillSlots = new List<SkillSlotUI>();

        skillSlotPool = Pool.Create<SkillSlotUI>(skillSlotUIPrefab);
        skillSlotPool.SetContainer(skillSlotUIParent);
        skillInventory.OnSkillInventoryLoaded.AddListener(UpdateSkillEquipSlots);
    }

    private void OnEnable()
    {
        skillInventory.OnAddSkill.AddListener(AddSlot);
    }

    private void OnDisable()
    {
        skillInventory.OnAddSkill.AddListener(AddSlot);
    }
    private void Start()
    {
        if (DisableSkillWindowOnStart) CloseInventory();
    }

    public void OpenInventory()
    {
        windowTransform.gameObject.SetActive(true);
        ActiveMode();
        //ShowSkillInfo(skillInventory.GetSkill("����˹�"));
    }

    public void CloseInventory()
    {
        RemoveAllSkillSlots();
        passiveMode = true;
        enterEquipSkillMode = false;

        skillInfoWindow.CloseSkillWindow();

        windowTransform.gameObject.SetActive(false);

        currentSkill = null;
    }

    public void AddSlot(Skill skill)
    {
        var slot = skillSlotPool.Get(); 
        
        skillSlots.Add(slot);
        slot.SetSlot(skill);
        //slot.OnClickSlot.AddListener((Skill mSkill) => ShowSkillInfo(mSkill));
        slot.AddListener(OnSkillSlotAdded);

    }

    private void OnSkillSlotAdded(Skill slotSkill)
    {
        skillInfoWindow.OpenSkillWindow(slotSkill, currentSkill == null);
        skillInfoWindow.OnCloseSkillWindow = null;
        skillInfoWindow.OnCloseSkillWindow += () => currentSkill = null;
        currentSkill = slotSkill;
    }

    private void RemoveAllSkillSlots()
    {
        for (int i = 0; i < skillSlots.Count; i++)
        {
            skillSlots[i].ClearSlot();
            skillSlotPool.Take(skillSlots[i]);
        }
        skillSlots.Clear();
    }

    bool passiveMode;
    public void PassiveMode()
    {
        if (passiveMode) return;
        passiveMode = true;
        var passives = skillInventory.GetPassives();

        RemoveAllSkillSlots();

        if (passives != null)
        {
            for (int i = 0; i < passives.Count; i++)
            {
                AddSlot(passives[i]);
            }
        }
        rightBtnAnim.DORestartById("Right");

    }

    public void ActiveMode()
    {
        if (!passiveMode) return;
        passiveMode = false;
        var actives = skillInventory.GetActives();

        RemoveAllSkillSlots();

        if (actives != null)
        {
            for (int i = 0; i < actives.Count; i++)
            {
                AddSlot(actives[i]);
            }

        }
        leftBtnAnim.DORestartById("Left");
        
    }

    public void EnterSkillEquipMode(int skillSlotIndex)
    {
        if (enterEquipSkillMode) return;
        if (skillInventory.GetSkillEquipSlot(skillSlotIndex).IsLocked)
        {
            WarningMessageInvoker.Instance.ShowMessage($"��ų ����{skillSlotIndex}�� ����ֽ��ϴ�");
            return;
        }
        enterEquipSkillMode = true;

        SkillEquip(skillSlotIndex);
    }

    // ��ų ������� True
    // ��ų���Ե鿡�� ������ �����Ǵ� �ż��� �Լ� ��ư�� �ֱ�
    // ��ų������ �Ϸ�Ǹ� ������� ����
    private void SkillEquip(int skillEquipSlotIndex)
    {
        if (!enterEquipSkillMode) return;
        
        for (int i = 0; i < skillSlots.Count; i++)
        {
            skillSlots[i].AddListener((skill) => EquipSkillUI(skill, skillEquipSlotIndex));
        }

    }

    private void EquipSkillUI(Skill skill, int slotIndex)
    {
        if (skill is ActiveSkill activeSkill)
        {
            bool success = skillInventory.EquipActiveSkill(activeSkill, slotIndex);

            if (success)
            {
                Debug.Log($"���������� ��ų {skill.skillName}�� ���� {slotIndex}�� �����߽��ϴ�");
                //UpdateSkillEquipSlot(slotIndex, activeSkill);
                UpdateSkillEquipSlots();
            }
            else
            {
                Debug.Log($"��ų {skill.skillName}�� ���� {slotIndex}�� ���� ����");
            }
            ExitSkillEquipMode(slotIndex);
        }
    }

    private void ExitSkillEquipMode(int skillEquipSlotIndex)
    {
        enterEquipSkillMode = false;

        for (int i = 0; i < skillSlots.Count; i++)
        {
            skillSlots[i].RemoveAllListener();
            skillSlots[i].AddListener(OnSkillSlotAdded);
        }

    }

    private void UpdateSkillEquipSlot(int index, ActiveSkill skill)
    {
        skillEquipSlots[index].EquipSkill(skill);
    }

    private void UpdateSkillEquipSlots()
    {
        var equippedActiveSkills = skillInventory.equippedActiveSkills;
        for (int i = 0; i < equippedActiveSkills.Length; i++)
        {
            if (equippedActiveSkills[i].IsLocked)
            {
                skillEquipSlots[i].SetLocked();
                continue;
            }
            if (!equippedActiveSkills[i].IsEquipped) 
            {
                skillEquipSlots[i].SetEmpty();
                continue;
            }
            skillEquipSlots[i].EquipSkill(skillInventory.GetSkillEquipSlot(i).EquippedSkill);
        }
    }
   
}
