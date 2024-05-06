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

    private readonly string rightArrow = "��";
    private float maxValue;
    public void SetStatDisplay(StatContainer playerStat, eSubStatType statType, Sprite statIcon, Color frontBarColor, float maxVal)
    {
        this.statIcon.sprite = statIcon;
        this.maxValue = maxVal;
        var t_subStat = playerStat.subStats[statType];
        frontBarImage.color = frontBarColor;
        statNameText.SetText(t_subStat.DisplayName);
        valueText.SetText($"{playerStat.subStats[statType].FinalValue}");
        frontBarSlider.value = t_subStat.FinalValue / maxVal;
        backBarSlider.value = t_subStat.FinalValue / maxVal;
    }
}
