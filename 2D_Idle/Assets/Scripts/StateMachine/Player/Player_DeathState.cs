using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.AI
{

    public class Player_DeathState : Player_BaseState
    {
        public Player_DeathState(PlayerController Player, Animator Anim) : base(Player, Anim)
        {

        }

        public override void OnEnter()
        {
            anim.CrossFade(DeathHash, crossFadeDuration);
            // run death logic
            player.SetTargetNull();
            player.DisableMovement();
        }

        public override void OnExit()
        {
            player.EnableMovement();
        }
    }
}
