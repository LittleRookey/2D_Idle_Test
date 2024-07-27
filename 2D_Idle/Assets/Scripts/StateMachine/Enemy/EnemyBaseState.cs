using UnityEngine;

namespace Litkey.AI
{
    public abstract class EnemyBaseState : IState {
        protected readonly EnemyAI enemy;
        protected readonly Animator animator;
        
        protected static readonly int IdleHash = Animator.StringToHash("0_idle");
        protected static readonly int RunHash = Animator.StringToHash("1_Run");
        //protected static readonly int WalkHash = Animator.StringToHash("WalkFWD");
        protected static readonly int NormalAttackHash = Animator.StringToHash("2_Attack_Normal");
        protected static readonly int RangeAttackHash = Animator.StringToHash("2_Attack_Bow");
        protected static readonly int MagicAttackHash = Animator.StringToHash("2_Attack_Magic");
        protected static readonly int DeathHash = Animator.StringToHash("4_Death");
        
        protected const float crossFadeDuration = 0.1f;

        protected string _stateName;
        public string StateName => _stateName;

        protected EnemyBaseState(EnemyAI enemy, Animator animator, string stateName) {
            this.enemy = enemy;
            this.animator = animator;
            _stateName = stateName;
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

        public virtual bool CanTransition()
        {
            return true;
        }
    }
}