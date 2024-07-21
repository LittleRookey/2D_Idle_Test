using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Litkey.Utility;
using Redcode.Pools;
using DG.Tweening;
using Litkey.Skill;


public class SkillInventoryUI : MonoBehaviour
{
    [SerializeField] private SkillInventory skillInventory;

    [Header("UIs")]
    [SerializeField] private RectTransform windowTransform;
 
    [SerializeField] private Button leftActiveSkillButton;
    [SerializeField] private Button rightPassiveSkillButton;
    
    // ¥Â∆Æ¿©
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
    private void Awake()
    {
        skillSlots = new List<SkillSlotUI>();

        skillSlotPool = Pool.Create<SkillSlotUI>(skillSlotUIPrefab);
        skillSlotPool.SetContainer(skillSlotUIParent);
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
        //ShowSkillInfo(skillInventory.GetSkill("ªÔ¿Á∞Àπ˝"));
    }

    public void CloseInventory()
    {
        RemoveAllSkillSlots();
        passiveMode = true;

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
        slot.AddListener((Skill slotSkill) =>
        {
            skillInfoWindow.OpenSkillWindow(slotSkill, currentSkill==null);
            skillInfoWindow.OnCloseSkillWindow = null;
            skillInfoWindow.OnCloseSkillWindow += () => currentSkill = null;
            currentSkill = slotSkill;

        });
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


   
}
