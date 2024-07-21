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

    private readonly string 달성시 = " 달성시";
    private readonly string 달성 = "(달성)";
    private readonly string 미달성 = "(미달성)";
    public void UpdateDescription(Skill skill, eSkillRank rank)
    {
        reachRankText.SetText(TMProUtility.GetColorText( rank.ToString() + 달성시, rarityColor.GetSkillColor(rank)));
        if (skill.currentRank >= rank)
        {
            // 이미 달성했다면
            reachText.SetText(달성);
        }
        else
        {
            reachText.SetText(미달성);
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
