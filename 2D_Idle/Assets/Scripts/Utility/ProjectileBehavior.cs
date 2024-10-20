using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Litkey.Projectile 
{

    public class ProjectileBehavior : MonoBehaviour
    {
        [SerializeField] public string projectileID;
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
        [SerializeField] private bool faceDirection = false;
        private EndOfLifeStrategy endOfLifeStrategy;
        [SerializeField] private SpriteRenderer innerRange;
        [SerializeField] private SpriteRenderer outerRange;

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

        public ProjectileBehavior HideInnerSkillRange()
        {
            innerRange.enabled = false;
            return this;
        }

        public ProjectileBehavior HideOuterSkillRange()
        {
            outerRange.enabled = false;
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

        public ProjectileBehavior SetDirection(Vector2 direction)
        {
            this.direction = direction;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Create a quaternion representing the z-axis rotation
            transform.rotation = Quaternion.Euler(0, 0, angle);

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
            ProjectileCreator.ReturnProjectile(projectileID, this);
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
            
            outerRange.enabled = true;
            innerRange.enabled = true;

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
                //if (faceDirection)
                //{
                //    UpdateRotation();
                //}
            }
        }

        private void UpdateRotation()
        {
            if (direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }

        public ProjectileBehavior SetFaceDirection(bool shouldFace)
        {
            faceDirection = shouldFace;
            return this;
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
                ProjectileCreator.ReturnProjectile(projectileID, this);
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