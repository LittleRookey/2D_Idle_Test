using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.AI
{
    public class Player_ChaseState : Player_BaseState
    {
        public Player_ChaseState(PlayerController Player, Animator Anim, string stateName) : base(Player, Anim, stateName)
        {
        }

        public override void OnEnter()
        {
            anim.CrossFade(EnterRunHash, crossFadeDuration);
            player.ChaseEnemy();
            player.EnableAIPath();
            player.EnableMovement();
        }

        public override void Update()
        {
            player.ChaseEnemy();
        }
    }
}
