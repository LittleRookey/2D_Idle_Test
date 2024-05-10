using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonDecorator : PassiveSkillDecorator
{
    
    public CinemachineTrack poisonDamagePerTick;
    public int poisonDuration;

    


    protected override void AddPassiveEffect(Health target)
    {
        
        ApplyPoison(target);
    }

    private void ApplyPoison(Health target)
    {
        // 포이즌 로직
    }
}
