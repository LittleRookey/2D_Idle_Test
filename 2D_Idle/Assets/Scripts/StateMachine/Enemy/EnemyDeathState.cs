using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.AI
{
    public class EnemyDeathState : EnemyBaseState
    {
        public EnemyDeathState(EnemyAI enemy, Animator animator, string stateName) : base(enemy, animator, stateName)
        {

        }

        public override void OnEnter()
        {
            animator.CrossFade(DeathHash, crossFadeDuration);
            // run death logic
            enemy.SetTarget(null);
            
        }
    }
}
