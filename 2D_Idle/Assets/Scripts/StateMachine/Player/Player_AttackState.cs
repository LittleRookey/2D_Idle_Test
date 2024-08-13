using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.AI
{
    public class Player_AttackState : Player_BaseState
    {
        public CountdownTimer attackTimer;

        private bool isAttackAnimationFinished = true;
        private float downslashAnimationDuration;
        private float upslashAnimationDuration;
        int attackIndex = 0;
        public Player_AttackState(PlayerController Player, Animator Anim, string stateName) : base(Player, Anim, stateName)
        {
            attackTimer = new CountdownTimer(1f);

            // Get the duration of the attack animation
            AnimationClip[] clips = Anim.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (clip.name == "downslash") // Replace with your actual attack animation name
                {
                    downslashAnimationDuration = clip.length;
                    continue;
                }
                else if (clip.name == "upslash")
                {
                    upslashAnimationDuration = clip.length;
                    continue;
                }
            }
        }

        public override void OnEnter()
        {
            //player.EnableAIPath();
            Debug.Log("Player Entered Attack State");
            SetIdle();
            attackIndex = 0;
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

            if (attackIndex == 0)
            {
                anim.CrossFadeInFixedTime(DownSlashHash, crossFadeDuration);
                attackIndex++;
            }
            else if (attackIndex == 1)
            {
                anim.CrossFadeInFixedTime(UpSlashHash, crossFadeDuration);
                attackIndex = 0;
                attackTimer.Start();
            }

            player.TurnToTarget();
            player.Attack();

            //attackTimer.Reset(attackInterval);
            

            isAttackAnimationFinished = false;
            player.StartCoroutine(WaitForAttackAnimationToFinish());
        }

        private IEnumerator WaitForAttackAnimationToFinish()
        {
            yield return new WaitForSeconds(downslashAnimationDuration*0.1f);
            Attack();
            yield return new WaitForSeconds(upslashAnimationDuration* 0.8f);
            
            isAttackAnimationFinished = true;
        }

        public override void OnExit()
        {
            base.OnExit();
            player.StopAllCoroutines(); // Stop the attack animation coroutine if it's still running
            isAttackAnimationFinished = true;
            attackTimer.Start();
        }

        public override bool CanTransition()
        {
            return isAttackAnimationFinished;
        }
    }
}
