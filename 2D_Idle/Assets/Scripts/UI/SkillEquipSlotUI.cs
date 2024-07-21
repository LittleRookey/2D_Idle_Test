using Litkey.Skill;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillEquipSlotUI : MonoBehaviour
{
    [SerializeField] private Image lockedImage;
    [SerializeField] private Image iconBGImage;
    [SerializeField] private Image skillIconImage;
    [SerializeField] private Image plusImage;

    
    public void SetEmpty()
    {
        plusImage.gameObject.SetActive(true);
        iconBGImage.gameObject.SetActive(false);
        skillIconImage.sprite = null;
        lockedImage.gameObject.SetActive(false);
    }

    public void SetLocked()
    {
        plusImage.gameObject.SetActive(false);
        iconBGImage.gameObject.SetActive(false);
        skillIconImage.sprite = null;
        lockedImage.gameObject.SetActive(true);
    }

    public void Unlock()
    {
        plusImage.gameObject.SetActive(true);
        iconBGImage.gameObject.SetActive(false);
        skillIconImage.sprite = null;
        lockedImage.gameObject.SetActive(false);
    }

    public void EquipSkill(ActiveSkill skill)
    {
        SetEmpty();
        skillIconImage.sprite = skill._icon;
    }

    public void UnEquipSkill()
    {
        SetEmpty();
    }
}
