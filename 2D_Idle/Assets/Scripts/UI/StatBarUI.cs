using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Litkey.Stat;
using DG.Tweening;
using Litkey.Utility;
using UnityEngine.Events;

public class StatBarUI : MonoBehaviour
{
    [Header("텍스트들")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI statNameText;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private TextMeshProUGUI apUsedText;

    [Space]
    [Header("버튼들")]
    [SerializeField] public Button minusButton;
    [SerializeField] public Button plusButton;
    [SerializeField] private DOTweenAnimation dotweenAnim;

    public UnityEvent OnMinusClicked;
    public UnityEvent<int> OnPlusClicked;

    // References
    private StatContainer playerStat;
    private eMainStatType mainStatType;
    public void InitMainStat(StatContainer playerStat, eMainStatType mainStat)
    {
        this.playerStat = playerStat;
        //this.playerStat.OnTryIncreaseStat.AddListener(ClickPlus);
        this.mainStatType = mainStat;

    }

    public bool IsEmpty()
    {
        return this.playerStat == null;
    }
    public void SetStatBarUI(MainStat mainStat)
    {
        if (mainStat == null)
        {
            // Handle the case where mainStat is null
            Debug.LogWarning("mainStat is null");
            return;
        }


        var connectedSubstats = mainStat.ChildSubstats;

        titleText.SetText($"{mainStat.StatName} Lv.{mainStat.LevelAddedStats}");
        
        // 스텟이름
        string content = string.Empty;

        for (int i = 0; i< connectedSubstats.Count; i++)
        {
            content += connectedSubstats[i].DisplayName;
            if (i + 1  < connectedSubstats.Count)
            {
                content += '\n';
            }
        }
        statNameText.SetText(content);

        // 스텟 값
        string valueText = string.Empty;
        for (int i = 0; i < connectedSubstats.Count; i++)
        {
            if (connectedSubstats[i].IsPercentage)
                valueText += $"{connectedSubstats[i].GetAddedStatValue().ToString("F2")}%";
            else
                valueText += connectedSubstats[i].GetAddedStatValue().ToString("F0");

            if (i + 1 < connectedSubstats.Count)
            {
                valueText += '\n';
            }
        }

        statValueText.SetText(valueText);

        // AP 텍스트
        apUsedText.SetText($"AP");
    }


    public void SetStatBarUIs(eMainStatType type, int usedAP)
    {
        //var _mainStat = playerStat.mainStats[mainStatType];
        //Debug.Log(this.mainStat.StatName + "///// " + _mainStat.StatName);
        //if (!this.mainStat.Equals(_mainStat)) return;
        if (mainStatType != type) return;
        var mainStat = playerStat.mainStats[mainStatType];
        var connectedSubstats = mainStat.ChildSubstats;

        titleText.SetText($"{mainStat.StatName} Lv.{mainStat.LevelAddedStats} " + TMProUtility.GetColorText("+" + usedAP, Color.green));

        // 스텟이름
        string content = string.Empty;

        for (int i = 0; i < connectedSubstats.Count; i++)
        {
            content += connectedSubstats[i].DisplayName;
            if (i + 1 < connectedSubstats.Count)
            {
                content += '\n';
            }
        }
        statNameText.SetText(content);

        // 스텟 값
        string valueText = string.Empty;
        for (int i = 0; i < connectedSubstats.Count; i++)
        {
            if (connectedSubstats[i].IsPercentage)
                valueText += $"{connectedSubstats[i].GetAddedStatValue().ToString("F2")} " + TMProUtility.GetColorText("+" +connectedSubstats[i].GetFutureStat(mainStat, usedAP), Color.green);
            else
                valueText += connectedSubstats[i].GetAddedStatValue().ToString("F0") + TMProUtility.GetColorText("+" + connectedSubstats[i].GetFutureStat(mainStat, usedAP), Color.green);

            if (i + 1 < connectedSubstats.Count)
            {
                valueText += '\n';
            }
        }

        statValueText.SetText(valueText);

        // AP 텍스트
        apUsedText.SetText($"AP -{usedAP}");

        ClickPlus();
    }

    public void ClickPlus()
    {
        dotweenAnim.DORestart();
        //SetStatBarUI(this.playerStat.TryAddMainStat, val);
    }

    public void ClickMinus()
    {
        dotweenAnim.DORestart();
    }

}
