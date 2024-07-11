using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.AI
{
    public class BattleIdleState : EnemyBaseState
    {
        public BattleIdleState(EnemyAI enemy, Animator animator, string stateName) : base(enemy, animator, stateName)
        {
        }

        public override void OnEnter()
        {
            enemy.StopMovement();
            animator.CrossFade(IdleHash, crossFadeDuration);
        }

        
    }
}

