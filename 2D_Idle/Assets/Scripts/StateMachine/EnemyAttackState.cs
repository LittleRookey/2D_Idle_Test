using UnityEngine;
using UnityEngine.AI;
using Pathfinding;

namespace Litkey.AI
{
    public class EnemyAttackState : EnemyBaseState {


        CountdownTimer attackTimer;
        float attackInterval;
        public EnemyAttackState(EnemyAI enemy, Animator animator, float attackInterval) : base(enemy, animator) 
        {
            attackTimer = new CountdownTimer(attackInterval);
            attackTimer.OnTimerStop += Attack;
            attackTimer.OnTimerStart += AttackTrue;
        }

        void AttackTrue() => enemy.isAttacking = true;
        public override void OnEnter() {
            Debug.Log("Entered AttackState");
            SetIdle();
            enemy.StopMovement();
            
        }

        void SetIdle()
        {
            animator.CrossFade(IdleHash, crossFadeDuration);
        }

        public void ResetTimerTime(float time)
        {
            attackTimer.Reset(time);
        }

        public override void Update() {
            attackTimer.Tick(Time.deltaTime);

            if (!attackTimer.IsRunning)
            {
                attackTimer.Start();
            }
        }

        void Attack()
        {
            animator.CrossFadeInFixedTime(NormalAttackHash, crossFadeDuration);
            enemy.Attack();
        }
    }
}