using UnityEngine;
using UnityEngine.AI;
using Pathfinding;

namespace Litkey.AI
{
    public enum eAttackType
    {
        근거리, 원거리, 마법
    }

    public class EnemyAttackState : EnemyBaseState {


        public CountdownTimer attackTimer;
        float attackInterval;
        float attackAnimationDuration;

        public int currentAttackHash;
        eAttackType attackType;
        public EnemyAttackState(EnemyAI enemy, Animator animator, float attackInterval, eAttackType attackType, string stateName) : base(enemy, animator, stateName) 
        {
            this.attackInterval = attackInterval;

            this.attackType = attackType;

            if (attackType == eAttackType.근거리) currentAttackHash = NormalAttackHash;
            else if (attackType == eAttackType.원거리) currentAttackHash = RangeAttackHash;
            else if (attackType == eAttackType.마법) currentAttackHash = MagicAttackHash;

            attackTimer = new CountdownTimer(attackInterval);
            
            //attackTimer.OnTimerStop += Attack;
            //attackTimer.OnTimerStart += AttackTrue;

            // Get the attack animation duration

        }

        void AttackTrue() => enemy.isAttacking = true;
        public override void OnEnter() {
            //Debug.Log("Entered AttackState");
            SetIdle();
            enemy.StopMovement();
            enemy.DisableAIPath();
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
            animator.Play(currentAttackHash);
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