using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Litkey.AI
{
    public class EnemyChaseState : EnemyBaseState
    {
        
        readonly Transform player;

        public EnemyChaseState(EnemyAI enemy, Animator animator, Transform target) : base(enemy, animator)
        {
            //this.agent = agent;

            //this.player = player;
        }

        public override void OnEnter()
        {
            Debug.Log("Chase");
            animator.CrossFade(RunHash, crossFadeDuration);
            this.enemy.SetMovePosition()
        }

        //public override void Update()
        //{
        //    agent.SetDestination(player.position);
        //}
    }
}
