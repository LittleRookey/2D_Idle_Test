using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SwordMastery", menuName = "Litkey/Skills/SwordMastery")]
public class SwordMastery : PassiveSkillDecorator
{
    public Dictionary<eSkillRank, SwordMasteryUpgrade> skillUpgrades => _skillUpgrades;
    [SerializeField] private Dictionary<eSkillRank, SwordMasteryUpgrade> _skillUpgrades;
    private void Awake()
    {

        
    }
    public void RankUp()
    {
        //if (currentRank = skillUpgrades)
        //{
        //    Initialize(rankUpDecorators[newRank - 1]);
        //}

    }

    protected override void AddPassiveEffect(StatContainer ally, StatContainer target)
    {
        throw new System.NotImplementedException();
    }
}

