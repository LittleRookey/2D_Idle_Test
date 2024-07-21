using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Litkey.Stat;
using Litkey.Skill;

public class SkillDescriptionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI reachRankText;
    [SerializeField] private TextMeshProUGUI reachText;
    [SerializeField] private TextMeshProUGUI statRankReachText;
    [SerializeField] private RarityColor rarityColor;

    private readonly string �޼��� = " �޼���";
    private readonly string �޼� = "(�޼�)";
    private readonly string �̴޼� = "(�̴޼�)";
    public void UpdateDescription(Skill skill, eSkillRank rank)
    {
        reachRankText.SetText(TMProUtility.GetColorText( rank.ToString() + �޼���, rarityColor.GetSkillColor(rank)));
        if (skill.currentRank >= rank)
        {
            // �̹� �޼��ߴٸ�
            reachText.SetText(�޼�);
        }
        else
        {
            reachText.SetText(�̴޼�);
        }
        var stats = skill.rankUpgrades[rank];

        string statText = string.Empty;

        foreach (var upgradeInfo in stats)
        {
            statText += $"{upgradeInfo.upgradeExplanation}\n";
        }
        statRankReachText.SetText(statText);
    }


}
