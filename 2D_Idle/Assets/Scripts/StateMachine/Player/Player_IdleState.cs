using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.AI
{

    public class Player_IdleState : Player_BaseState
    {
        public Player_IdleState(PlayerController Player, Animator Anim, string stateName) : base(Player, Anim, stateName)
        {

        }

        public override void OnEnter()
        {
            Debug.Log("Player Entered Idle State");
            anim.CrossFade(IdleHash, crossFadeDuration);
            if (!player.HasNoTarget())
            {
                //player.EnableAIPath();

            }
        }

        public override void Update()
        {
            if (player.Auto())
            {
                if (player.HasNoTarget() && !player.IsInteracting() && !player.IsStunned())
                    player.SearchForTarget();
            }
        }
    }
}
