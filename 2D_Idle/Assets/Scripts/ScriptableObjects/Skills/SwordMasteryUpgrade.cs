using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SwordMasteryUpgrade", menuName = "Litkey/SkillUpgrades/SwordMasteryUpgrade")]
public class SwordMasteryUpgrade : SkillUpgrade
{
    public eSkillRank rankToApply;
    public PassiveSkill rankDecorator;

    // ���׷��̵� ��Ҹ� ���� �����ִٰ�, �ҵ帶���͸��� �ִ� ����Ʈ�� ���� ȿ���� �߰��ϱ�
    public override void ApplyUpgrade(Skill skill)
    {
        if (skill is SwordMastery swordMastery)
        {

            //swordMastery.skillUpgrades.Add(this);
            
        }
        
    }
}