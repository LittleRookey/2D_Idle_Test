using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Litkey.Stat;

public class BuffDebuffSlotUI : MonoBehaviour
{
    [SerializeField] private Image buffIcon;
    [SerializeField] private TextMeshProUGUI buffAccumulatedCount; // ¹öÇÁ ÁßÃ¸È½¼ö
    
    
    public void SetSlot(BuffInfo buffInfo, StatContainer statContainer)
    {
        buffIcon.sprite = buffInfo.buff.BuffIcon;
        if (buffInfo.buff.Stackable)
        {
            buffAccumulatedCount.gameObject.SetActive(true);
            buffAccumulatedCount.SetText(buffInfo.stackCount.ToString());
        }
        else
        {
            buffAccumulatedCount.gameObject.SetActive(false);
        }
        gameObject.SetActive(true);
    }

    public void UpdateCount(BuffInfo buffInfo)
    {
        buffAccumulatedCount.SetText(buffInfo.stackCount.ToString());
    }
}
