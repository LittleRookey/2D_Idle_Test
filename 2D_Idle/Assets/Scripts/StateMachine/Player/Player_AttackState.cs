using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.AI
{
    public class Player_AttackState : Player_BaseState
    {
        public CountdownTimer attackTimer;
        public Player_AttackState(PlayerController Player, Animator Anim) : base(Player, Anim)
        {
            attackTimer = new CountdownTimer(1);

        }

        public override void OnEnter()
        {
            player.EnableAIPath();
            SetIdle();
            player.DisableMovement();
            Attack();

        }

        public override void Update()
        {
            if (attackTimer.IsFinished)
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
            player.TurnToTarget();
            anim.CrossFadeInFixedTime(DownSlashHash, crossFadeDuration);
            player.UseAttackOrSkill();

            //attackTimer.Reset(attackInterval);
            attackTimer.Start();
        }

    }
}
