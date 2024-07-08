using UnityEngine;
using UnityEngine.AI;
using Pathfinding;

namespace Litkey.AI
{
    public class EnemyAttackState : EnemyBaseState {


        public CountdownTimer attackTimer;
        float attackInterval;
        float attackAnimationDuration;

        public EnemyAttackState(EnemyAI enemy, Animator animator, float attackInterval) : base(enemy, animator) 
        {
            this.attackInterval = attackInterval;
            
            
            attackTimer = new CountdownTimer(attackInterval);
            
            //attackTimer.OnTimerStop += Attack;
            //attackTimer.OnTimerStart += AttackTrue;

            // Get the attack animation duration
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (clip.name == "2_Attack_Normal")
                {
                    attackAnimationDuration = clip.length;
                    break;
                }
            }
        }

        void AttackTrue() => enemy.isAttacking = true;
        public override void OnEnter() {
            Debug.Log("Entered AttackState");
            SetIdle();
            enemy.StopMovement();
            Attack();
            //    attackTimer.Start();
        }

        void SetIdle()
        {
            animator.CrossFade(IdleHash, crossFadeDuration);
        }

        public override void Update() {

            if (attackTimer.IsFinished)
            {
                Attack();
            }
        }

        void Attack()
        {
            enemy.ChaseEnemy();
            animator.Play(NormalAttackHash);
            enemy.UseAttackOrSkill();

            //attackTimer.Reset(attackInterval);
            attackTimer.Start();
        }
        public override void OnExit()
        {
            enemy.isAttacking = false;
            //attackTimer.Stop();
        }
    }
}