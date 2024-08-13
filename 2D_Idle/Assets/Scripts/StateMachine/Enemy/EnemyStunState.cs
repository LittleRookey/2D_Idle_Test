using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.AI
{
    public class EnemyStunState : EnemyBaseState
    {
        public EnemyStunState(EnemyAI enemy, Animator animator, string stateName) : base(enemy, animator, stateName)
        {

        }

        public override bool CanTransition()
        {
            return !enemy.IsStunned();
        }

        public override void OnEnter()
        {
            animator.CrossFade(IdleHash, crossFadeDuration);
            // TODO Spawn STun Effect on enemy's head
        }

    }
}
