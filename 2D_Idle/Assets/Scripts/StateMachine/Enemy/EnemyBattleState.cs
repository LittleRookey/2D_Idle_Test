using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.AI
{
    public class EnemyBattleState : EnemyBaseState
    {
        public EnemyBattleState(EnemyAI enemy, Animator animator) : base(enemy, animator)
        {

        }

        public override void OnEnter()
        {
            //Debug.Log("Entered BattleState");
        }
    }
}

