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
            // ��ų�� �� ��ũ���� Dscription�� ���� 

            reachRankText.SetText($"{TMProUtility.GetColorText(rank.ToString(), rarityColor.GetSkillColor(rank))} �޼���");
            if (passiveSkill.currentRank >= rank)
            {
                // �̹� �޼��ߴٸ�
                reachText.SetText("(�޼�)");
            }
            else
            {
                reachText.SetText("(�̴޼�)");
            }
            // �ش� ��ũ�� ������ ������ ��ųʸ��� ��� ������� ����

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
            reachRankText.SetText($"{TMProUtility.GetColorText(rank.ToString(), rarityColor.GetSkillColor(rank))} �޼���");
            if (activeSkill.currentRank >= rank)
            {
                // �̹� �޼��ߴٸ�
                reachText.SetText("(�޼�)");
            }
            else
            {
                reachText.SetText("(�̴޼�)");
            }
            // �ش� ��ũ�� ������ ������ ��ųʸ��� ��� ������� ����

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
    //            // ��ų�� �� ��ũ���� Dscription�� ���� 

    //            reachRankText.SetText($"{rank} �޼���");
    //        if (passiveSkill.currentRank >= rank)
    //        {
    //            // �̹� �޼��ߴٸ�
    //            reachText.SetText("(�޼�)");
    //        } else
    //        {
    //            reachText.SetText("(�̴޼�)");
    //        }
    //        // �ش� ��ũ�� ������ ������ ��ųʸ��� ��� ������� ����

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
