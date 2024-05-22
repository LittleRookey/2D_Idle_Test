using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SwordMasteryUpgrade", menuName = "Litkey/SkillUpgrades/SwordMasteryUpgrade")]
public class SwordMasteryUpgrade : SkillUpgrade
{
    public eSkillRank rankToApply;
    public PassiveSkill rankDecorator;

    // 업그레이드 요소를 여기 갖고있다가, 소드마스터리에 있는 리스트나 현재 효과에 추가하기
    public override void ApplyUpgrade(Skill skill)
    {
        if (skill is SwordMastery swordMastery)
        {

            //swordMastery.skillUpgrades.Add(this);
            
        }
        
    }
}