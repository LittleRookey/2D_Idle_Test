using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Litkey.Skill;

public class SkillSlotUI : MonoBehaviour
{
    [SerializeField] private Image slotIcon;
    [SerializeField] private Button slotClickBtn;
    //public UnityEvent<Skill> OnClickSlot;

    private Skill skill;
    public void SetSlot(Skill skill)
    {
        slotIcon.sprite = skill._icon;
        this.skill = skill;
        
    }

    public void AddListener(UnityAction<Skill> onClick)
    {
        slotClickBtn.onClick.AddListener(() => onClick(skill));
    }

    public void ClearSlot()
    {
        slotIcon.sprite = null;
        slotClickBtn.onClick.RemoveAllListeners();
    }

    
}
