using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Litkey.Stat;

public class SkillDescriptionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI reachRankText;
    [SerializeField] private TextMeshProUGUI reachText;
    [SerializeField] private TextMeshProUGUI statRankReachText;
    [SerializeField] private RarityColor rarityColor;

    public void UpdateDescription(Skill skill, eSkillRank rank)
    {
        if (skill is PassiveSkill passiveSkill)
        {
            // 스킬의 각 랭크마다 Dscription을 만들어서 

            reachRankText.SetText($"{TMProUtility.GetColorText(rank.ToString(), rarityColor.GetSkillColor(rank))} 달성시");
            if (passiveSkill.currentRank >= rank)
            {
                // 이미 달성했다면
                reachText.SetText("(달성)");
            }
            else
            {
                reachText.SetText("(미달성)");
            }
            // 해당 랭크의 스텟의 총합을 딕셔너리에 묶어서 순서대로 나열

            var stats = passiveSkill.rankUpgrades[rank];

            string statText = string.Empty;

            foreach (var upgradeInfo in stats)
            {
                statText += $"{upgradeInfo.upgradeExplanation}\n";
            }
            statRankReachText.SetText(statText);


        }
        else if (skill is ActiveSkill activeSkill)
        {
            reachRankText.SetText($"{TMProUtility.GetColorText(rank.ToString(), rarityColor.GetSkillColor(rank))} 달성시");
            if (activeSkill.currentRank >= rank)
            {
                // 이미 달성했다면
                reachText.SetText("(달성)");
            }
            else
            {
                reachText.SetText("(미달성)");
            }
            // 해당 랭크의 스텟의 총합을 딕셔너리에 묶어서 순서대로 나열

            var stats = activeSkill.rankUpgrades[rank];

            string statText = string.Empty;

            foreach (var upgradeInfo in stats)
            {
                statText += $"{upgradeInfo.upgradeExplanation}\n";
            }
            statRankReachText.SetText(statText);
        }

    }

    //public void UpdateDescription(Skill skill, eSkillRank rank)
    //{
    //    if (skill is PassiveSkill passiveSkill)
    //    {
    //            // 스킬의 각 랭크마다 Dscription을 만들어서 

    //            reachRankText.SetText($"{rank} 달성시");
    //        if (passiveSkill.currentRank >= rank)
    //        {
    //            // 이미 달성했다면
    //            reachText.SetText("(달성)");
    //        } else
    //        {
    //            reachText.SetText("(미달성)");
    //        }
    //        // 해당 랭크의 스텟의 총합을 딕셔너리에 묶어서 순서대로 나열

    //        var stats = StatUtility.GetSumOfStats(passiveSkill.LevelUpgrades[rank]);

    //        string statText = string.Empty;
    //        foreach(var statType in stats.Keys)
    //        {
    //            if (stats[statType] > 0)
    //                statText += $"{statType} +{stats[statType]}\n";
    //            else if (stats[statType] < 0)
    //                statText += $"{statType} {stats[statType]}\n";
    //        }
    //        statRankReachText.SetText(statText);


    //    } else if (skill is ActiveSkill activeSkill)
    //    {

    //    }

    //}


}
