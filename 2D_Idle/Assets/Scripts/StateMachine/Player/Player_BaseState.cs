using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.AI
{
    public abstract class Player_BaseState : IState
    {
        protected PlayerController player;
        protected Animator anim;

        protected static readonly int IdleHash = Animator.StringToHash("Idle");
        protected static readonly int EnterRunHash = Animator.StringToHash("EnterRun");
        protected static readonly int DeathHash = Animator.StringToHash("Death");
        protected static readonly int DownSlashHash = Animator.StringToHash("downslash");
        protected static readonly int UpSlashHash = Animator.StringToHash("upslash");
        protected static readonly int FrontAttackHash = Animator.StringToHash("frontAttack");
        protected static readonly int ReviveHash = Animator.StringToHash("Revive");
        protected static readonly int HurtHash = Animator.StringToHash("Hurt");
        //protected static readonly int IdleHash = Animator.StringToHash("Idle");

        protected const float crossFadeDuration = 0.1f;

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
