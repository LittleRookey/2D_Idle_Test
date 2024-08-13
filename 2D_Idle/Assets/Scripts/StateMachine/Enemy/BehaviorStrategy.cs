using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.AI
{
    public interface IStrategy
    {
        void Execute(EnemyAI enemy);
    }
    [System.Serializable]
    public abstract class BehaviorStrategy : IStrategy
    {
        public abstract void Execute(EnemyAI enemy);
    }

    // Chase strategies


    [System.Serializable]
    // Attack strategies
    public class AttackStrategy : BehaviorStrategy
    {
        protected List<IAttackDecorator> decorators = new List<IAttackDecorator>();
        public override void Execute(EnemyAI enemy)
        {
            
        }
        public AttackStrategy AddDecorator(IAttackDecorator decorator)
        {
            decorators.Add(decorator);
            return this;
        }
    }
    [System.Serializable]
    public class SimpleAttackStrategy : AttackStrategy
    {
        public override void Execute(EnemyAI enemy)
        {
            enemy.Attack();
        }

    }
    [System.Serializable]
    public class StabStrategy : AttackStrategy
    {

        public override void Execute(EnemyAI enemy)
        {
            // play stab animation
            // Perform base stab attack
            enemy.UseAttackOrSkill();

            // Apply decorators
            foreach (var decorator in decorators)
            {
                decorator.Apply(enemy.GetTarget());
            }
        }
    }

    // Attack decorators
    public interface IAttackDecorator
    {
        void Apply(Health target);
    }

    public class BleedingDecorator : IAttackDecorator
    {
        private float damagePerSecond;
        private float duration;

        public BleedingDecorator(float damagePerSecond, float duration)
        {
            this.damagePerSecond = damagePerSecond;
            this.duration = duration;
        }

        public void Apply(Health target)
        {
            // Apply bleeding effect to target
            // This could be implemented as a coroutine or using a timer system
        }
    }

    public class StunDecorator : IAttackDecorator
    {
        private float stunDuration;

        public StunDecorator(float stunDuration)
        {
            this.stunDuration = stunDuration;
        }

        public void Apply(Health target)
        {
            var player = target.GetComponent<PlayerController>();
            // Apply stun effect to target
            // This could set a flag on the target or use a timer system
            if (player != null)
            {
                player.Stun(stunDuration);
            }
        }
    }
}

