using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Litkey.Skill;
using DG.Tweening;

public class SkillCooldownSlot : MonoBehaviour
{
    [SerializeField] private Image cooldownImage;
    [SerializeField] private Image skillIcon;
    [SerializeField] private RectTransform iconBG;
    [SerializeField] private Button plusIcon;
    ActiveSkill activeSkill;
    public bool IsSameSkill(ActiveSkill active)
    {
        if (activeSkill == null) return false;
        return active.ID == activeSkill.ID && active.skillName == activeSkill.skillName;
    }

    public void SetSlot(ActiveSkill activeSkill)
    {
        plusIcon.gameObject.SetActive(false);

        iconBG.gameObject.SetActive(true);
        skillIcon.sprite = activeSkill._icon;
        cooldownImage.fillAmount = 0f;
        this.activeSkill = activeSkill;
    }

    public void SetEmpty()
    {
        plusIcon.gameObject.SetActive(true);

        iconBG.gameObject.SetActive(false);
    }
    public void SetLocked()
    {
        plusIcon.gameObject.SetActive(false);
        iconBG.gameObject.SetActive(false);
    }

    public void ClearSlot()
    {
        skillIcon.sprite = null;
        iconBG.gameObject.SetActive(false);
        plusIcon.gameObject.SetActive(true);
        this.activeSkill = null;
    }

    public void PutOnCooldown(float time)
    {
        float startAmount = 1f;
        DOTween.To(() => startAmount, x => startAmount = x, 0f, time)
            .OnUpdate(() =>
            {
                cooldownImage.fillAmount = startAmount;
            });
        
    }
}
