using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.AI
{

    public class Player_IdleState : Player_BaseState
    {
        public Player_IdleState(PlayerController Player, Animator Anim) : base(Player, Anim)
        {

        }

        public override void OnEnter()
        {
            anim.CrossFade(IdleHash, crossFadeDuration);
            if (!player.HasNoTarget())
                player.EnableAIPath();
        }

        public override void Update()
        {
            if (player.Auto())
            {
                if (player.HasNoTarget())
                    player.SearchForTarget();
            }
        }
    }
}
