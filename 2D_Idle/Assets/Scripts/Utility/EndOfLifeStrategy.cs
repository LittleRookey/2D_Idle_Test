using Litkey.Skill;
using UnityEngine;
using UnityEngine.Events;

namespace Litkey.Projectile
{
    public abstract class EndOfLifeStrategy
    {
        public abstract void Execute(ProjectileBehavior projectile);
    }

    public class DefaultEndOfLifeStrategy : EndOfLifeStrategy
    {
        public override void Execute(ProjectileBehavior projectile)
        {
            // Default behavior: do nothing
        }
    }

    public class ExplosionEndOfLifeStrategy : EndOfLifeStrategy
    {
        private float explosionRadius;
        private float explosionDamage;
        private float delayTime= 2f;
        private LayerMask enemyLayer;
        public ExplosionEndOfLifeStrategy(float radius, float damagePercent)
        {
            explosionRadius = radius;
            explosionDamage = damagePercent;
        }

        public ExplosionEndOfLifeStrategy SetDelayTime(float delay)
        {
            this.delayTime = delay;
            return this;
        }

        // 50 - 150
        public ExplosionEndOfLifeStrategy SetDamage(float dmg)
        {
            this.explosionDamage = dmg;
            return this;
        }

        public ExplosionEndOfLifeStrategy SetRadius(float radius)
        {
            this.explosionRadius = radius;
            return this;
        }

        public ExplosionEndOfLifeStrategy SetEnemyLayer(LayerMask enemyLayer)
        {
            this.enemyLayer = enemyLayer;
            return this;
        }

        public override void Execute(ProjectileBehavior projectile)
        {
            // Create explosion effect
            // Apply damage to nearby enemies
            var skillRange = SkillRangeCreator.CreateCircleSkillRange(projectile.transform.position, explosionRadius, delayTime);

            skillRange
                .SetAttackNumber(1)
                .SetTargetNumber(20)
                .SetDamagePercent(explosionDamage / 100f)
                .SetSkillRadius(explosionRadius)
                .SetEnemyLayer(enemyLayer)
                .SetAlly(projectile.OwnerStat)
                .AddListenerOnEnd(() => skillRange.ExecuteAttack(true))
                .StartRange();


        }
    }

    public class SpawnProjectilesEndOfLifeStrategy : EndOfLifeStrategy
    {
        private int projectileCount;
        private float spreadAngle;

        public SpawnProjectilesEndOfLifeStrategy(int count, float angle)
        {
            projectileCount = count;
            spreadAngle = angle;
        }

        public override void Execute(ProjectileBehavior projectile)
        {
            for (int i = 0; i < projectileCount; i++)
            {
                float angle = (i / (float)(projectileCount - 1) - 0.5f) * spreadAngle;
                Vector3 direction = Quaternion.Euler(0, 0, angle) * projectile.Direction;

                ProjectileCreator.CreateProjectile(projectile.transform.position, projectile.OwnerStat, projectile.EnemyLayer)
                    .SetDirection(direction)
                    .SetSpeed(projectile.Speed)
                    .SetDamagePercent(projectile.DamagePercent * 0.5f)
                    .SetDisappearTimer(2f);
            }
        }
    }
}