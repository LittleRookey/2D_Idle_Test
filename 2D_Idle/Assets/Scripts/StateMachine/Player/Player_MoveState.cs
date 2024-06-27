using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.AI
{

    public class Player_MoveState : Player_BaseState
    {
        public Player_MoveState(PlayerController Player, Animator Anim) : base(Player, Anim)
        {
        }

        public override void OnEnter()
        {
            player.SetTargetNull();
            player.DisableAIPath();
            anim.CrossFade(EnterRunHash, crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            player.MoveWithJoystick();
        }
    }
}
