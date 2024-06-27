﻿using UnityEngine;

namespace Litkey.AI
{
    public abstract class EnemyBaseState : IState {
        protected readonly EnemyAI enemy;
        protected readonly Animator animator;
        
        protected static readonly int IdleHash = Animator.StringToHash("0_idle");
        protected static readonly int RunHash = Animator.StringToHash("1_Run");
        //protected static readonly int WalkHash = Animator.StringToHash("WalkFWD");
        protected static readonly int NormalAttackHash = Animator.StringToHash("2_Attack_Normal");
        protected static readonly int DeathHash = Animator.StringToHash("4_Death");
        
        protected const float crossFadeDuration = 0.1f;

        protected EnemyBaseState(EnemyAI enemy, Animator animator) {
            this.enemy = enemy;
            this.animator = animator;
        }
        
        public virtual void OnEnter() {
            // noop
        }

        public virtual void Update() {
            // noop
        }

        public virtual void FixedUpdate() {
            // noop
        }

        public virtual void OnExit() {
            // noop
        }
    }
}