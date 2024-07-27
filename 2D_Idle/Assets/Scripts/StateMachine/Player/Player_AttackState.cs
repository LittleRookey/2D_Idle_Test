using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.AI
{
    public class Player_AttackState : Player_BaseState
    {
        public CountdownTimer attackTimer;

        private bool isAttackAnimationFinished = true;
        private float attackAnimationDuration;
        public Player_AttackState(PlayerController Player, Animator Anim, string stateName) : base(Player, Anim, stateName)
        {
            attackTimer = new CountdownTimer(1.5f);

            // Get the duration of the attack animation
            AnimationClip[] clips = Anim.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (clip.name == "downslash") // Replace with your actual attack animation name
                {
                    attackAnimationDuration = clip.length;
                    break;
                }
            }
        }

        public override void OnEnter()
        {
            //player.EnableAIPath();
            Debug.Log("Player Entered Attack State");
            SetIdle();
            player.DisableMovement();
            Attack();

        }

        public override void Update()
        {
            player.UseSkill();
            if (attackTimer.IsFinished && isAttackAnimationFinished)
            {
                Attack();
            }
        }

        void SetIdle()
        {
            anim.CrossFade(IdleHash, crossFadeDuration);
        }

        void Attack()
        {
            if (!isAttackAnimationFinished) return;

            player.TurnToTarget();
            anim.CrossFadeInFixedTime(DownSlashHash, crossFadeDuration);
            player.Attack();

            //attackTimer.Reset(attackInterval);
            attackTimer.Start();

            isAttackAnimationFinished = false;
            player.StartCoroutine(WaitForAttackAnimationToFinish());
        }

        private IEnumerator WaitForAttackAnimationToFinish()
        {
            yield return new WaitForSeconds(attackAnimationDuration);
            isAttackAnimationFinished = true;
        }

        public override void OnExit()
        {
            base.OnExit();
            player.StopAllCoroutines(); // Stop the attack animation coroutine if it's still running
            isAttackAnimationFinished = true;
        }

        public override bool CanTransition()
        {
            return isAttackAnimationFinished;
        }
    }
}
