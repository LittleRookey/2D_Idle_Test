using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.AI
{
    public class Player_ChaseState : Player_BaseState
    {
        private bool isRunning = false;
        private float idleDuration = 1f; // Duration to stay idle
        private float idleTimer = 0f;

        public Player_ChaseState(PlayerController Player, Animator Anim, string stateName) : base(Player, Anim, stateName)
        {
        }

        public override void OnEnter()
        {
            //anim.CrossFade(EnterRunHash, crossFadeDuration);
            Debug.Log("Player Entered Chase State");
            isRunning = false;

        }

        public override void FixedUpdate()
        {
            if (player.TargetWithinAttackRange())
            {
                SetIdle();
                idleTimer = 0f; // Reset the timer when entering idle
            }
            else
            {
                if (isRunning)
                {
                    player.ChaseEnemy();
                }
                else
                {
                    // If we're idle, increment the timer
                    idleTimer += Time.fixedDeltaTime;

                    // If we've been idle long enough, start running again
                    if (idleTimer >= idleDuration)
                    {
                        SetRun();
                    }
                }
            }
        }

        private void SetIdle()
        {
            if (isRunning)
            {
                anim.Play(IdleHash);
                isRunning = false;
                player.DisableMovement();
            }
        }

        private void SetRun()
        {
            if (!isRunning)
            {
                anim.Play(EnterRunHash);
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
