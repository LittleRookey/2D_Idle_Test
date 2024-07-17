using Litkey.Interface;
using Litkey.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Litkey.Stat;
using Litkey.Skill;
using Pathfinding;
using Litkey.AI;
using System.Linq;
using Sirenix.OdinInspector;
using Litkey.Projectile;

public class EnemyAI : MonoBehaviour
{
    protected enum eEnemyBehavior
    {
        idle,
        chase,
        attack,
        dead,
        hit,
        stun,
    };
    [SerializeField] protected GameObject orientation;

    [SerializeField] protected float scanDistance = 3f;
    [SerializeField] protected float attackRange = 2.5f;

    [SerializeField] protected LayerMask enemyLayer;

    [SerializeField] protected EnemyBasicAttack basicAttack;

    protected float attackInterval;
    protected float final_attackInterval;

    protected Health Target;

    protected eEnemyBehavior currentBehavior;

    [SerializeField] protected Animator anim;

    public UnityEvent<Health> OnIdle= new();
    public UnityEvent<Health> OnAttack = new();
    [HideInInspector] public UnityEvent<Health> OnChase = new();
    [HideInInspector] public UnityEvent<Health> OnDead = new();
    [HideInInspector] public UnityEvent<Health> OnHit = new();
    [HideInInspector] public UnityEvent<Health> OnStun = new();



    protected Dictionary<eEnemyBehavior, UnityEvent<Health>> onStateEnterBeahviors;
    //protected Dictionary<eEnemyBehavior, UnityEvent<Health>> onStateExitBeahviors;

    protected StatContainer _statContainer;

    protected Health health;

    [SerializeField] protected Material enemyMat;
    [SerializeField] protected SpriteRenderer enemySprite;

    protected Vector2 moveDir;

    private AIPath aiPath;

    [Header("StateMachines")]
    [SerializeField]
    public StateMachine stateMachine;

    Vector3 right = Vector3.one;
    Vector3 left = new Vector3(-1f, 1f, 1f);
    AIDestinationSetter aiDestinationSetter;

    private BoxCollider2D boxCollider2D;
    private Rigidbody2D rb;
    private SkillContainer _skillContainer;

    private EnemyAttackState attackState;
    CountdownTimer attackStateTimer;
    EnemyWanderState wanderState;

    [Header("Attack Type")]
    public bool attackOnSearched; // ����, �񼱰�
    [SerializeField, EnumToggleButtons] private eAttackType attackType = eAttackType.�ٰŸ�;
    public enum eProjectileStrategy
    {
        Straight,
        Homing,
        Slerped,
    }

    [ShowInInspector, ShowIfGroup("attackType", eAttackType.���Ÿ�)] public eProjectileStrategy _strategy;
    [SerializeField, Range(0, 2f),ShowIf("_strategy", eProjectileStrategy.Homing)] private float homingStrength = 0.5f;
    [SerializeField, ShowIf("attackType", eAttackType.���Ÿ�)] private int _targetCountPerProjectile = 1;
    [SerializeField, ShowIf("attackType", eAttackType.���Ÿ�)] private int _damageCount = 1;
    [SerializeField, ShowIf("attackType", eAttackType.���Ÿ�)] private float _disappearTime=4f;
    [SerializeField, ShowIf("attackType", eAttackType.���Ÿ�)] private float _damagePercent=1f;
    [SerializeField, ShowIf("attackType", eAttackType.���Ÿ�)] private float _projectileSpeed=1f;
    [SerializeField, ShowIf("attackType", eAttackType.���Ÿ�)] private bool enableKnockback;
    [SerializeField, Range(0, 50f), ShowIf("enableKnockback", true)] private float knockbackForce = 0.5f;

    [Header("Projectile End Settings")]
    [SerializeField, EnumToggleButtons, ShowIf("attackType", eAttackType.���Ÿ�)] private eEndOfLifeStrategy _endOfLifeStrategy;
    [SerializeField, ShowIf("_endOfLifeStrategy", eEndOfLifeStrategy.Explosion)] private float explosionRadius = 1f;
    [SerializeField, ShowIf("_endOfLifeStrategy", eEndOfLifeStrategy.Explosion)] private float explosionDamage = 10f;
    [SerializeField, ShowIf("_endOfLifeStrategy", eEndOfLifeStrategy.SpawnProjectiles)] private int spawnProjectileCount = 3;
    [SerializeField, ShowIf("_endOfLifeStrategy", eEndOfLifeStrategy.SpawnProjectiles)] private float spawnProjectileSpreadAngle = 90f;

    public enum eEndOfLifeStrategy
    {
        Default,
        Explosion,
        SpawnProjectiles
    }


    protected void Awake()
    {
        if (enemySprite == null)
        {
            enemySprite = GetComponentInChildren<SpriteRenderer>();
            if (enemySprite != null)
            {
                enemyMat = enemySprite.material;
            }
        }
        onStateEnterBeahviors = new Dictionary<eEnemyBehavior, UnityEvent<Health>>()
        {
            { eEnemyBehavior.idle, OnIdle},
            { eEnemyBehavior.attack, OnAttack },
            { eEnemyBehavior.hit, OnHit },
            {eEnemyBehavior.chase, OnChase },
            { eEnemyBehavior.dead, OnDead },
            { eEnemyBehavior.stun, OnStun },
        };
        _skillContainer = GetComponent<SkillContainer>();
        _statContainer = GetComponent<StatContainer>();
        health = GetComponent<Health>();
        attackInterval = _statContainer.GetBaseStat().Attack_Interval;
        aiPath = GetComponent<AIPath>();
        aiDestinationSetter = GetComponent<AIDestinationSetter>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        currentBehavior = eEnemyBehavior.idle;
        //SwitchState(eEnemyBehavior.idle);
        stateMachine = new StateMachine();
        float attackANimDuration = 0.5f;
        var chaseState = new EnemyChaseState(this, anim, "chase");
        attackState = new EnemyAttackState(this, anim, 1f, attackType,"attack");
        attackStateTimer = attackState.attackTimer;
        wanderState = new EnemyWanderState(this, anim, aiPath, 1.5f, 3f, "wander");
        var battleState = new EnemyBattleState(this, anim, "battle");
        var battleIdleState = new BattleIdleState(this, anim, "idle");
        var deathState = new EnemyDeathState(this, anim, "death");

        At(wanderState, battleState, new FuncPredicate(() => !HasNoTarget()));
        At(battleState, wanderState, new FuncPredicate(() => HasNoTarget()));

        At(battleState, chaseState, new FuncPredicate(() => !TargetWithinAttackRange()));
        At(battleState, attackState, new FuncPredicate(() => TargetWithinAttackRange() && !AttackCooldown()));

        At(attackState, battleState, new FuncPredicate(() => HasNoTarget()), true, attackANimDuration);
        At(chaseState, battleState, new FuncPredicate(() => HasNoTarget()));

        At(attackState, chaseState, new FuncPredicate(() => !TargetWithinAttackRange() && AttackCooldown()), true, attackANimDuration);
        At(chaseState, attackState, new FuncPredicate(() => TargetWithinAttackRange() && !AttackCooldown()));

        At(attackState, battleIdleState, new FuncPredicate(() => TargetWithinAttackRange() && AttackCooldown()), true, attackANimDuration);
        At(chaseState, battleIdleState, new FuncPredicate(() => AttackCooldown() && TargetWithinAttackRange()));

        At(battleIdleState, attackState, new FuncPredicate(() => !AttackCooldown() && TargetWithinAttackRange()));
        At(battleIdleState, chaseState, new FuncPredicate(() => !TargetWithinAttackRange()));

        Any(deathState, new FuncPredicate(() => IsDead()));
        stateMachine.SetState(wanderState);
    }

    protected void OnEnable()
    {

        //OnStunExit.AddListener(onStunExit);

        UpdateAttackSpeed();
        health.OnDeath.AddListener(OnDeath);
        health.onTakeDamage += OnHitEnter;
        health.OnReturnFromPool += SetToBaseState;

        health.OnHit.AddListener(OnFirstAttackedByPlayer);
    }

    protected void OnDisable()
    {

        //OnStunExit.RemoveListener(onStunExit);
        health.OnDeath.RemoveListener(OnDeath);
        health.onTakeDamage -= OnHitEnter;
        health.OnReturnFromPool -= SetToBaseState;
        health.OnHit.RemoveListener(OnFirstAttackedByPlayer);
    }

    // Start is called before the first frame update
    void Start()
    {
        stateMachine.SetToBaseState();
    }

    void At(IState from, IState to, IPredicate condition, bool hasExitTime = false, float exitTime = 0.0f) => stateMachine.AddTransition(from, to, condition, hasExitTime, exitTime);
    void Any(IState to, IPredicate condition, bool hasExitTime = false, float exitTime = 0.0f) => stateMachine.AddAnyTransition(to, condition, hasExitTime, exitTime);


    public void Init()
    {
        SpawnPoint = transform.position;
        //SetTarget(null);

        //aiPath.endReachedDistance = attackRange;
        //anim.Play("0_idle");
        StartMovement();
        stateMachine.SetState(wanderState);
        FadeIn();
    }

    public bool IsDead()
    {
        return health.IsDead;
    }

    private void OnFirstAttackedByPlayer(LevelSystem lvl)
    {
        if (HasNoTarget() && !IsDead())
        {
            SetTarget(lvl.GetComponent<Health>());
        }
    }

    public void ChaseEnemy()
    {
        //SetMovePosition(Target.transform.position);

        TurnBasedOnDirection(aiPath.desiredVelocity);
    }

    public void SetMovePosition(Vector3 movePosition, UnityAction onReachedMovePosition=null)
    {
        aiPath.destination = movePosition;
    }

    public void TurnBasedOnDirection(Vector3 direction)
    {
        if (direction.x > 0)
        {
            Turn(true);
        }
        else if (direction.x < 0)
        {
            Turn(false);
        }
    }

    Vector2 SpawnPoint;
    public Vector2 SetWanderPoint(float wanderRadius)
    {
        var randomDirection = Random.insideUnitCircle * wanderRadius;
        randomDirection += SpawnPoint;
        return randomDirection;
    }

    public bool AttackCooldown()
    {
        return !attackStateTimer.IsFinished;
    }
    protected void UpdateAttackSpeed()
    {
        attackInterval = _statContainer.GetBaseStat().Attack_Interval;

        //final_attackInterval = Mathf.Max(attackInterval * (1f - (_statContainer.AttackSpeed.FinalValue / attackInterval)), 2f);
    }

    public Health GetTarget()
    {
        return Target;
    }

    public void Turn(bool turnRight)
    {
        orientation.transform.localScale = turnRight? left : right;
    }


    protected void OnDeath(LevelSystem targ)
    {
        Target = null;
        boxCollider2D.isTrigger = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        FadeOut();
        //float startValue = -0.1f;

        //DOTween.To(() => startValue, x => {
        //    startValue = x;
        //    enemyMat.SetFloat("_FadeAmount", startValue);
        //}, 1f, 0.5f).SetDelay(1f).SetEase(Ease.OutQuad);


        //isAttacking = false;
        // ���� �ִϸ��̼��÷���
        //onStateEnterBeahviors[eEnemyBehavior.dead]?.Invoke(health);

    }

    private void FadeOut()
    {

        foreach (var sprite in GetComponentsInChildren<SpriteRenderer>()) 
        {
            var mat = sprite.material;

            float startValue = -0.1f;

            DOTween.To(() => startValue, x => {
                startValue = x;
                mat.SetFloat("_FadeAmount", startValue);
            }, 1f, 1f).SetDelay(0.5f).SetEase(Ease.OutQuad);
        }
    }

    private void FadeIn()
    {

        foreach (var sprite in GetComponentsInChildren<SpriteRenderer>())
        {
            var mat = sprite.material;

            float startValue = -0.1f;
            mat.SetFloat("_FadeAmount", startValue);
            //DOTween.To(() => startValue, x => {
            //    startValue = x;
            //    mat.SetFloat("_FadeAmount", startValue);
            //}, 1f, 0.5f).SetDelay(1f).SetEase(Ease.OutQuad);
        }
    }

    public void SetToBaseState()
    {
        stateMachine.SetToBaseState();
    }

    // called from animation
    public void OnDeathExit()
    {
        SpawnManager.Instance.TakeToPool(health);
    }

    private float startValue = 0f;
    private void OnHitEnter(float current, float max)
    {
        onStateEnterBeahviors[eEnemyBehavior.hit]?.Invoke(health);
        var hitSequence = DOTween.Sequence();

        hitSequence.Append(DOTween.To(() => startValue, x => startValue = x, 0.5f, 0.1f)
            .OnUpdate(() =>
            {
                enemyMat.SetFloat("_HitEffectBlend", startValue);
            }))
            .AppendInterval(0.1f) // Optional: Delay before the hit effect fades out
            .Append(DOTween.To(() => startValue, x => startValue = x, 0f, 0.1f)
            .OnUpdate(() =>
            {
                enemyMat.SetFloat("_HitEffectBlend", startValue);
            }));
    }

    protected bool HasNoTarget()
    {
        return Target == null;
    }

    protected bool TargetWithinAttackRange()
    {
        if (Target == null)
        {
            return false;
        }
        return Vector2.Distance(Target.transform.position, transform.position) <= attackRange;
    }

    private RaycastHit2D[] raycastHits = new RaycastHit2D[10]; // Adjust size as needed
    public virtual bool SearchForTarget()
    {
        // Create a circle around the transform's position with the specified scanDistance radius
        Vector2 circleCenter = transform.position;
        float circleRadius = scanDistance;

        // Perform a CircleCastAll to detect any colliders within the circle
        int hitCount = Physics2D.CircleCastNonAlloc(circleCenter, circleRadius, Vector2.zero, raycastHits, 0f, enemyLayer);

        if (hitCount > 0 && Target == null)
        {
            float closestDistance = float.MaxValue;
            int closestIndex = -1;

            // Find the closest valid target
            for (int i = 0; i < hitCount; i++)
            {
                float distance = Vector2.SqrMagnitude(raycastHits[i].transform.position - transform.position);
                if (distance < closestDistance)
                {
                    Health target = raycastHits[i].transform.GetComponent<Health>();
                    if (target != null && !target.IsDead)
                    {
                        closestDistance = distance;
                        closestIndex = i;
                    }
                }
            }

            // Set the target if a valid one was found
            if (closestIndex != -1)
            {
                SetTarget(raycastHits[closestIndex].transform.GetComponent<Health>());
                return true;
            }
        }

        return false;
    }

    public void SetTarget(Health enemy)
    {
        if (Target != null)
        {
            Target.OnDeath.RemoveListener(SetTargetNullOnDead);
        }
        Target = enemy;
        if (enemy == null)
        {   
            aiDestinationSetter.target = null;
        } else
        {
            aiDestinationSetter.target = Target.transform;
            enemy.OnDeath.AddListener(SetTargetNullOnDead);
        }
        //Debug.Log("Target set: " + enemy.name);
    }
    public bool isAttacking;

    private void SetTargetNullOnDead(LevelSystem levelSystem)
    {
        Debug.Log("Target set to null on death");
        SetTarget(null);
    }

    DamageDecorator damageDecorator;
    private void Attack()
    {

        if (basicAttack == null)
        {
            basicAttack = Resources.Load<EnemyBasicAttack>("ScriptableObject/Skills/EnemyBasicAttack");
        }
        if (Target == null) return;
        // ������ ���
        if (attackType == eAttackType.�ٰŸ�)
        {
            basicAttack.ApplyEffect(_statContainer, Target.GetComponent<StatContainer>());
        }
        else if (attackType == eAttackType.���Ÿ�)
        {
            var projectile = ProjectileCreator.CreateProjectile(transform.position, _statContainer, enemyLayer);

            if (enableKnockback) projectile.AddDecorator(new KnockBackDecorator()
                                                                .SetKnockbackForce(knockbackForce));

            if (damageDecorator == null) damageDecorator = new DamageDecorator();

            EndOfLifeStrategy endOfLifeStrategy = GetEndOfLifeStrategy();

            projectile.SetDamagePercent(_damagePercent)
                    .SetDirection((Target.transform.position - transform.position).normalized)
                    .SetTargetCount(_targetCountPerProjectile)
                    .SetSpeed(_projectileSpeed)
                    .SetStrategy(_strategy, _disappearTime, Target.transform.position, OnProjectileReachDestination)
                    .AddDecorator(damageDecorator
                        .ShowDamagePopup(false)
                        .SetDamageCount(_damageCount))
                    .SetDisappearTimer(_disappearTime)
                    .SetEndOfLifeStrategy(endOfLifeStrategy);

        }
        else if (attackType == eAttackType.����)
        {

        }
        
    }

    private void OnProjectileReachDestination()
    {
        // Implement any logic for when the projectile reaches its destination
        Debug.Log("Projectile reached destination");
    }

    private EndOfLifeStrategy GetEndOfLifeStrategy()
    {
        switch (_endOfLifeStrategy)
        {
            case eEndOfLifeStrategy.Explosion:
                return new ExplosionEndOfLifeStrategy(explosionRadius, explosionDamage).SetEnemyLayer(enemyLayer);
            case eEndOfLifeStrategy.SpawnProjectiles:
                return new SpawnProjectilesEndOfLifeStrategy(spawnProjectileCount, spawnProjectileSpreadAngle);
            default:
                return new DefaultEndOfLifeStrategy();
        }
    }

    public void UseAttackOrSkill()
    {
        ActiveSkill usableSkill = _skillContainer.FindUsableSkill();
        if (usableSkill != null && TargetWithinAttackRange())
        {
            _skillContainer.UseActiveSkill(usableSkill, Target);
        }
        else
        {
            // Trigger normal attack animations or mechanics
            Attack();
        }
    }

    public void StopMovement() => aiPath.canMove = false;

    public void StartMovement() => aiPath.canMove = true;

    public void DisableAIPath() => aiPath.enabled = false;
    public void EnableAIPath() => aiPath.enabled = true;

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }
    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
        attackStateTimer.Tick(Time.deltaTime);

        //Debug.Log($"Current State {stateMachine.current.State} :\nTarget Exist? {!HasNoTarget()} Target Within AttackRange? {TargetWithinAttackRange()} Attack Cooldown? {AttackCooldown()}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, scanDistance);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
