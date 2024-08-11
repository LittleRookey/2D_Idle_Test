using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.AI
{

    public class Player_StunState : Player_BaseState
    {
        public Player_StunState(PlayerController Player, Animator Anim, string stateName) : base(Player, Anim, stateName)
        {

        }

        public override bool CanTransition()
        {
            return !player.IsStunned();
        }

        public override void OnEnter()
        {
            anim.CrossFade(IdleHash, crossFadeDuration);
            // TODO Spawn STun Effect on player's head
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
