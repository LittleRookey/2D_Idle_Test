using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.AI
{
    public class Player_ChaseState : Player_BaseState
    {
        private bool isRunning = false;
        public Player_ChaseState(PlayerController Player, Animator Anim, string stateName) : base(Player, Anim, stateName)
        {
        }

        public override void OnEnter()
        {
            anim.CrossFade(EnterRunHash, crossFadeDuration);
            Debug.Log("Player Entered Chase State");
            //player.EnableAIPath();
            //player.EnableMovement();
        }

        public override void FixedUpdate()
        {
            if (player.TargetWithinAttackRange())
            {
                SetIdle();
            } else
            {
                SetRun();
                player.ChaseEnemy();
            }
        }

        private void SetIdle()
        {
            if (isRunning)
            {
                anim.CrossFade(IdleHash, crossFadeDuration);
                isRunning = false;
                player.DisableMovement();
            }
        }

        private void SetRun()
        {
            if (!isRunning)
            {
                anim.CrossFade(EnterRunHash, crossFadeDuration);
                isRunning = true;
                player.EnableMovement();
            }
        }

        public override void OnExit()
        {
            SetIdle();
        }
    }
}
