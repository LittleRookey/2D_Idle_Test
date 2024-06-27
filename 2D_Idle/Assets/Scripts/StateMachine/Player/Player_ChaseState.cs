using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.AI
{
    public class Player_ChaseState : Player_BaseState
    {
        public Player_ChaseState(PlayerController Player, Animator Anim) : base(Player, Anim)
        {
        }

        public override void OnEnter()
        {
            anim.CrossFade(EnterRunHash, crossFadeDuration);
            player.ChaseEnemy();
            player.EnableAIPath();
        }

        public override void Update()
        {
            player.ChaseEnemy();
        }
    }
}
