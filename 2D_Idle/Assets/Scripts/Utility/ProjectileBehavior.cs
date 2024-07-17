using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Litkey.Projectile 
{

    public class ProjectileBehavior : MonoBehaviour
    {
        public float Speed => speed;
        public Vector3 Direction => direction;
        public float DamagePercent => damagePercent;
        public int TargetCount => targetCount;
        public LayerMask EnemyLayer => enemyLayer;
        public StatContainer OwnerStat => ownerStat;

        [SerializeField] private float speed;
        [SerializeField] private Vector3 direction;
        [SerializeField] private float damagePercent;
        [SerializeField] private int targetCount;

        [SerializeField] private LayerMask enemyLayer;

        [SerializeField] private StatContainer ownerStat;

        [ShowInInspector] private ProjectileStrategy strategy;
        [ShowInInspector] private List<IProjectileDecorator> decorators = new List<IProjectileDecorator>();

        private EndOfLifeStrategy endOfLifeStrategy;

        CountdownTimer timer;

        StraightProjectileStrategy straight;
        HomingProjectileStrategy homing;
        SlerpedProjectileStrategy slerped;

        CircleCollider2D collider2D;

        private void Awake()
        {
            collider2D = GetComponent<CircleCollider2D>();
            timer = new CountdownTimer(1f);
            straight = new StraightProjectileStrategy();
            homing = new HomingProjectileStrategy(transform, enemyLayer);
            slerped = new SlerpedProjectileStrategy(1.5f, 2f);
        }
        #region Setters 
        public ProjectileBehavior SetStrategy(EnemyAI.eProjectileStrategy strategy, float duration=default, Vector3 destination=default, UnityAction OnDestination=null)
        {
            this.strategy = straight;
            if (strategy == EnemyAI.eProjectileStrategy.Homing)
            {
                homing.SetEnemyLayer(this.enemyLayer)
                        .SetHomingStrength(1.5f)
                        .SetSelfTransform(transform);
                this.strategy = homing;
            }
            else if (strategy == EnemyAI.eProjectileStrategy.Slerped)
            {
                slerped.SetDestination(destination);
                slerped.SetDuration(duration);

                this.strategy = slerped;
            }
            
            return this;
        }

        public ProjectileBehavior SetEnemyLayer(LayerMask enemyLayer)
        {
            this.enemyLayer = enemyLayer;
            return this;
        }

        public ProjectileBehavior SetStatContainer(StatContainer stat)
        {
            this.ownerStat = stat;
            return this;
        }

        public ProjectileBehavior SetSpeed(float speed)
        {
            this.speed = speed;
            return this;
        }

        public ProjectileBehavior SetDirection(Vector3 direction)
        {
            this.direction = direction;
            return this;
        }

        public ProjectileBehavior SetDamagePercent(float damagePercent)
        {
            this.damagePercent = damagePercent;
            return this;
        }

        public ProjectileBehavior SetTargetCount(int targetCount)
        {
            this.targetCount = targetCount;
            return this;
        }

        public ProjectileBehavior SetDisappearTimer(float disappearTime, System.Action OnTimerEnd=null)
        {
            timer.OnTimerStop = null;
            timer.Reset(disappearTime);
            timer.OnTimerStop += () => {
                OnTimerEnd?.Invoke();
                PerformEndOfLifeBehavior();
                ReturnToPool();
            };
            timer.Start();
            return this;
        }

        public ProjectileBehavior SetEndOfLifeStrategy(EndOfLifeStrategy strategy)
        {
            endOfLifeStrategy = strategy;
            return this;
        }

        private void PerformEndOfLifeBehavior()
        {
            //if (!collider2D.enabled) // If the collider is disabled, it means the projectile hasn't hit anything
            //{
            endOfLifeStrategy?.Execute(this);
            //}
        }

        #endregion
        private void ReturnToPool()
        {
            ProjectileCreator.ReturnProjectile(this);
        }

        public ProjectileBehavior AddDecorator(IProjectileDecorator decorator)
        {
            decorators.Add(decorator);
            return this;
        }

        public void Initialize()
        {
            if (decorators == null) decorators = new List<IProjectileDecorator>();
            decorators.Clear();
            this.strategy = null;

            SetEndOfLifeStrategy(null);

            this.ownerStat = null;
            EnableCollisionWithEnemy();

        }

        public void EnableCollisionWithEnemy() => collider2D.enabled = true;
        public void DisableCollisionWithEnemy() => collider2D.enabled = false;


        private void Update()
        {
            timer.Tick(Time.deltaTime);
            if (strategy != null)
            {
                strategy.Move(this);
            }
        }

        public void OnHit(GameObject target)
        {
            foreach (var decorator in decorators)
            {
                decorator.OnHit(this, target);
            }
            targetCount--;
            if (targetCount <= 0)
            {
                DisableCollisionWithEnemy();
                //PerformEndOfLifeBehavior();
                ProjectileCreator.ReturnProjectile(this);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (((1 << collision.gameObject.layer) & enemyLayer) != 0)
            {
                OnHit(collision.gameObject);
            }

        }

        
    }

   
}