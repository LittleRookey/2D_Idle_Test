using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Litkey.AI
{
    public class EnemyChaseState : EnemyBaseState
    {
        
        readonly Transform player;

        public EnemyChaseState(EnemyAI enemy, Animator animator, string stateName) : base(enemy, animator, stateName)
        {

        }

        public override void OnEnter()
        {

            animator.CrossFade(RunHash, crossFadeDuration);
            this.enemy.ChaseEnemy();
            enemy.StartMovement();
            enemy.EnableAIPath();
        }

        public override void Update()
        {
            //agent.SetDestination(player.position);
            this.enemy.ChaseEnemy();
        }
    }
}
