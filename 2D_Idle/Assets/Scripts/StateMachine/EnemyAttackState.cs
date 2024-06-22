using UnityEngine;
using UnityEngine.AI;
using Pathfinding;

namespace Litkey.AI
{
    public class EnemyAttackState : EnemyBaseState {
        readonly NavMeshAgent agent;
        readonly Transform player;
        
        public EnemyAttackState(EnemyAI enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator) {
            this.agent = agent;
            this.player = player;
        }
        
        public override void OnEnter() {
            Debug.Log("Attack");
            animator.CrossFade(AttackHash, crossFadeDuration);
        }
        
        public override void Update() {
            agent.SetDestination(player.position);
            //enemy.Attack();
        }
    }
}