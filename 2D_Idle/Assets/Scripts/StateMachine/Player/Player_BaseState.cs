using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.AI
{
    public abstract class Player_BaseState : IState
    {
        protected PlayerController player;
        protected Animator anim;

        protected Player_BaseState(PlayerController Player, Animator Anim)
        {
            this.player = Player;
            this.anim = Anim;
        }
        public virtual void FixedUpdate()
        {
            // noop
        }

        public virtual void OnEnter()
        {
            // noop
        }

        public virtual void OnExit()
        {
            // noop
        }

        public virtual void Update()
        {
            // noop
        }
    }
}
