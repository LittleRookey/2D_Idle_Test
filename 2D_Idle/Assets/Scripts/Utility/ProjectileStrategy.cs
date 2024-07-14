using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Litkey.Projectile
{
    // Strategy pattern for movement
    public abstract class ProjectileStrategy
    {
        public abstract void Move(ProjectileBehavior projectile);
    }

    public class StraightProjectileStrategy : ProjectileStrategy
    {
        public override void Move(ProjectileBehavior projectile)
        {
            projectile.transform.Translate(projectile.Direction * projectile.Speed * Time.deltaTime);
        }
    }

    public class HomingProjectileStrategy : ProjectileStrategy
    {
        public float homingStrength = 0.5f; // 0 to 1
        // while moving, search for enemy within range, if there is, change direction slightly

        private float searchRange = 1.3f;
        Transform self;
        private Transform Target;
        LayerMask enemyLayer;
        public HomingProjectileStrategy(Transform selfProjectile, LayerMask enemyLayer)
        {
            this.self = selfProjectile;
            this.enemyLayer = enemyLayer;
        }

        public HomingProjectileStrategy SetSearchRange(float searchRange)
        {
            this.searchRange = searchRange;
            return this;
        }

        private void SearchForTargetNearby()
        {
            var searched = Physics2D.CircleCast(self.position, searchRange, Vector2.zero, 0f, enemyLayer);
            if (searched.collider != null)
            {
                Target = searched.transform;
            }
        }

        public override void Move(ProjectileBehavior projectile)
        {
            // Simple homing logic, assumes a target is set
            if (Target == null)
                SearchForTargetNearby(); // This should be optimized in a real scenario

            if (Target != null)
            {
                if (Vector2.Distance(Target.position, self.position) < searchRange)
                {
                    Vector3 directionToTarget = (Target.transform.position - projectile.transform.position).normalized;
                    projectile.SetDirection(Vector3.Lerp(projectile.Direction, directionToTarget, homingStrength * Time.deltaTime));
                }
            }
            projectile.transform.Translate(projectile.Direction * projectile.Speed * Time.deltaTime);
        }
    }

    // Decorator pattern for on-hit effects
    public interface IProjectileDecorator
    {
        void OnHit(ProjectileBehavior projectile, GameObject target);
    }

    public class DamageDecorator : IProjectileDecorator
    {
        private bool _showDamagePopup = false;
        private int _damageCount = 1;
        public void OnHit(ProjectileBehavior projectile, GameObject target)
        {
            StatContainer targetStats = target.GetComponent<StatContainer>();
            if (targetStats != null)
            {
                var damages = projectile.OwnerStat.GetDamagesAgainst(targetStats, _damageCount, projectile.DamagePercent);
                targetStats.GetComponent<Health>().TakeDamage(projectile.OwnerStat, damages, _showDamagePopup);
            }
        }

        public DamageDecorator ShowDamagePopup(bool showDMGPop = true)
        {
            _showDamagePopup = showDMGPop;
            return this;
        }
        public DamageDecorator SetDamageCount(int damageCount)
        {
            this._damageCount = damageCount;
            return this;
        }
    }

    public class KnockBackDecorator : IProjectileDecorator
    {
        private float _knockBackForce = 10f;

        public void OnHit(ProjectileBehavior projectile, GameObject target)
        {
            Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
            if (targetRb != null)
            {
                targetRb.AddForce(projectile.Direction * _knockBackForce, ForceMode2D.Impulse);
            }
        }

        public KnockBackDecorator SetKnockbackForce(float knockbackForce)
        {
            this._knockBackForce = knockbackForce;
            return this;
        }
    }
}
