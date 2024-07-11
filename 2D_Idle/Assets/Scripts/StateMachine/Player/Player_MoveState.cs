using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.AI
{

    public class Player_MoveState : Player_BaseState
    {
        public Player_MoveState(PlayerController Player, Animator Anim, string stateName) : base(Player, Anim, stateName)
        {
        }

        public override void OnEnter()
        {
            player.SetTargetNull();
            player.DisableAIPath();
            player.EnableMovement();
            anim.CrossFade(EnterRunHash, crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            if (!player.CanMove) return;
            player.MoveWithJoystick();
        }
    }
}
