using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Litkey.Utility;
using DG.Tweening;


public class StatDisplayUI : MonoBehaviour
{
    [SerializeField] private Image statIcon;
    [SerializeField] private TextMeshProUGUI statNameText;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Slider frontBarSlider;
    [SerializeField] private Image frontBarImage;
    [SerializeField] private Slider backBarSlider;
    [SerializeField] private Image BGBar;
    [SerializeField] private RectTransform backbarFill;
    [SerializeField] private RectTransform frontbarFill;


    private readonly string rightArrow = "¡æ";
    private float maxValue;
    private StatContainer playerStat;
    private eSubStatType statType;

    public void SetStatDisplay(StatContainer playerStat, eSubStatType statType, Sprite statIcon, Color frontBarColor, float maxVal)
    {
        this.playerStat = playerStat;
        this.statType = statType;
        this.statIcon.sprite = statIcon;
        this.maxValue = maxVal;
        var t_subStat = playerStat.subStats[statType];
        frontBarImage.color = frontBarColor;
        statNameText.SetText(t_subStat.DisplayName);
        valueText.SetText($"{playerStat.subStats[statType].FinalValue}");
        frontBarSlider.value = t_subStat.FinalValue / maxVal;
        backBarSlider.value = t_subStat.FinalValue / maxVal;
    }

    public void PreviewStat(float extraValue)
    {
        var subStat = playerStat.subStats[statType];
        var finalValue = subStat.FinalValue + extraValue;
        valueText.SetText(TMProUtility.GetColorText($"{subStat.FinalValue } {rightArrow} {finalValue}", Color.green));
        
        backBarSlider.value = finalValue / this.maxValue;
        if (backBarSlider.value > frontBarSlider.value)
        {
            backbarFill.transform.SetSiblingIndex(1);
        } 
        else if (backBarSlider.value < frontBarSlider.value)
        {
            frontbarFill.transform.SetSiblingIndex(1);
        }
    }

    public void UpdateStat()
    {
        var stat = playerStat.subStats[statType];
        valueText.SetText($"{stat.FinalValue}");
        frontBarSlider.value = stat.FinalValue / this.maxValue;
        backBarSlider.value = stat.FinalValue / this.maxValue;
    }
    
    public void UpdatePreviewedStat()
    {
        var stat = playerStat.subStats[statType];
        valueText.SetText($"{stat.FinalValue}");
        //backBarSlider.value = stat.FinalValue / this.maxValue;
        float current = frontBarSlider.value;
        DOTween.To(() => current, x => current = x, backBarSlider.value, 0.5f)
            .OnUpdate(() => frontBarSlider.value = current)
            .OnComplete(()=> frontBarSlider.value = backBarSlider.value);
        //frontBarSlider.value = stat.FinalValue / this.maxValue;
    }
}
