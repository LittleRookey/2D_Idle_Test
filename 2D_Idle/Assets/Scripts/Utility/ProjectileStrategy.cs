using DG.Tweening;
using Litkey.Skill;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Litkey.Projectile
{
    // Strategy pattern for movement
    [System.Serializable]
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

    public class SlerpedProjectileStrategy : ProjectileStrategy
    {
        private Vector3 destination;
        private float arcHeight;
        private float duration;
        private Ease easeType;
        private Tween currentTween;

        public SlerpedProjectileStrategy(float arcHeight, float duration, Ease easeType = Ease.Linear)
        {
            this.arcHeight = arcHeight;
            this.duration = duration;
            this.easeType = easeType;
        }
        private bool playedOnce = false;
        public SlerpedProjectileStrategy SetDestination(Vector3 newDest)
        {
            this.destination = newDest;
            playedOnce = false;
            return this;
        }

        public SlerpedProjectileStrategy SetDuration(float duration)
        {
            this.duration = duration;
            return this;
        }

        public override void Move(ProjectileBehavior projectile)
        {
            if ((currentTween == null || !currentTween.IsActive()) && !playedOnce)
            {
                playedOnce = true;
                currentTween = projectile.transform.DOJump(destination, arcHeight, 1, duration)
                    .SetEase(easeType)
                    .OnComplete(() => projectile.EnableCollisionWithEnemy());
            }
        }

    }
    public class HomingProjectileStrategy : ProjectileStrategy
    {
        private float homingStrength = 0.5f; // 0 to 1
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

        public HomingProjectileStrategy SetSelfTransform(Transform self)
        {
            this.self = self;
            return this;
        }

        public HomingProjectileStrategy SetHomingStrength(float strength)
        {
            this.homingStrength = strength;
            return this;
        }

        public HomingProjectileStrategy SetEnemyLayer(LayerMask enemyLayer)
        {
            this.enemyLayer = enemyLayer;
            return this;
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
        private float _knockBackForce = 1f;
        private float _knockBackDuration = 0.2f;
        private Ease _knockBackEase = Ease.OutQuad;
        public void OnHit(ProjectileBehavior projectile, GameObject target)
        {
            Vector3 knockbackDirection = projectile.Direction.normalized;
            Vector3 targetEndPosition = target.transform.localPosition + (knockbackDirection * _knockBackForce);

            target.transform.DOLocalMove(targetEndPosition, _knockBackDuration)
                .SetEase(_knockBackEase);
        }

     

        public KnockBackDecorator SetKnockbackForce(float knockbackForce)
        {
            this._knockBackForce = knockbackForce;
            return this;
        }
        public KnockBackDecorator SetKnockbackDuration(float knockbackDuration)
        {
            this._knockBackDuration = knockbackDuration;
            return this;
        }
    }
}
