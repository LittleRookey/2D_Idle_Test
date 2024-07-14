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

        private ProjectileStrategy strategy;
        private List<IProjectileDecorator> decorators = new List<IProjectileDecorator>();

        CountdownTimer timer;

        private void Awake()
        {
            timer = new CountdownTimer(1f);
            
        }
        #region Setters 
        public ProjectileBehavior SetStrategy(ProjectileStrategy strategy)
        {
            this.strategy = strategy;
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
            timer.OnTimerStop += OnTimerEnd;
            timer.OnTimerStop += ReturnToPool;
            timer.Start();
            return this;
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
            this.ownerStat = null;

        }

        private void Update()
        {
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
                Destroy(gameObject);
            }
        }
    }

   
}