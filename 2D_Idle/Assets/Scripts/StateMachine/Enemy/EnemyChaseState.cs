using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Litkey.AI
{
    public class EnemyChaseState : EnemyBaseState
    {
        
        readonly Transform player;

        public EnemyChaseState(EnemyAI enemy, Animator animator) : base(enemy, animator)
        {

        }

        public override void OnEnter()
        {

            animator.CrossFade(RunHash, crossFadeDuration);
            this.enemy.ChaseEnemy();
            enemy.StartMovement();
        }

        public override void Update()
        {
            //agent.SetDestination(player.position);
            this.enemy.ChaseEnemy();
        }
    }
}
