using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonDecorator : PassiveSkill
{
    
    public float poisonDamagePerTick;
    public int poisonDuration;

    protected override void OnLevelUp()
    {
        throw new NotImplementedException();
    }

    protected override void OnRankUp()
    {
        throw new NotImplementedException();
    }

    public void ApplyPoison(StatContainer target)
    {
        // Æ÷ÀÌÁð ·ÎÁ÷
    }

    public override void ApplyEffect(StatContainer allyStat, StatContainer target)
    {
        throw new NotImplementedException();
    }

    public override void EquipPassiveStat(StatContainer statContainer)
    {
        throw new NotImplementedException();
    }
}
