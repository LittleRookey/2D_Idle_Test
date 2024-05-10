using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonDecorator : PassiveSkillDecorator
{
    
    public float poisonDamagePerTick;
    public int poisonDuration;

    


    protected override void AddPassiveEffect(StatContainer target)
    {
        
        ApplyPoison(target);
    }

    private void ApplyPoison(StatContainer target)
    {
        // 포이즌 로직
    }
}
