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

    protected DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> currentBarTween;

    protected Health health;

    protected Material enemyMat;

    protected Vector2 moveDir;

    private AIPath aiPath;
    [SerializeField]
    private StateMachine stateMachine;


    Vector3 right = Vector3.one;
    Vector3 left = new Vector3(-1f, 1f, 1f);
    AIDestinationSetter aiDestinationSetter;
    protected void Awake()
    {
        onStateEnterBeahviors = new Dictionary<eEnemyBehavior, UnityEvent<Health>>()
        {
            { eEnemyBehavior.idle, OnIdle},
            { eEnemyBehavior.attack, OnAttack },
            { eEnemyBehavior.hit, OnHit },
            {eEnemyBehavior.chase, OnChase },
            { eEnemyBehavior.dead, OnDead },
            { eEnemyBehavior.stun, OnStun },
        };

        _statContainer = GetComponent<StatContainer>();
        health = GetComponent<Health>();
        attackInterval = _statContainer.GetBaseStat().Attack_Interval;
        aiPath = GetComponent<AIPath>();
        aiDestinationSetter = GetComponent<AIDestinationSetter>();
    }

    protected void OnEnable()
    {
        Init();
        //OnStunExit.AddListener(onStunExit);

        UpdateAttackSpeed();
        health.OnDeath.AddListener(OnDeath);
        health.onTakeDamage += OnHitEnter;
    }

    protected void OnDisable()
    {

        //OnStunExit.RemoveListener(onStunExit);
        health.OnDeath.RemoveListener(OnDeath);
        health.onTakeDamage -= OnHitEnter;
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnPoint = transform.position;

        currentBehavior = eEnemyBehavior.idle;
        //SwitchState(eEnemyBehavior.idle);
        stateMachine = new StateMachine();

        var chaseState = new EnemyChaseState(this, anim);
        var attackState = new EnemyAttackState(this, anim, 1f);
        var wanderState = new EnemyWanderState(this, anim, aiPath, 1.5f, 3f);
        var battleState = new EnemyBattleState(this, anim);
        At(wanderState, battleState, new FuncPredicate(() => !HasNoTarget()));
        At(battleState, wanderState, new FuncPredicate(() => HasNoTarget()));
        
        At(battleState, chaseState, new FuncPredicate(() => !TargetWithinAttackRange()));
        At(battleState, attackState, new FuncPredicate(() => TargetWithinAttackRange()));

        At(attackState, battleState, new FuncPredicate(() => HasNoTarget()));
        At(chaseState, battleState, new FuncPredicate(() => HasNoTarget()));

        At(attackState, chaseState, new FuncPredicate(() => !TargetWithinAttackRange()));
        At(chaseState, attackState, new FuncPredicate(() => TargetWithinAttackRange()));

        stateMachine.SetState(wanderState);
    }

    void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

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

    protected void UpdateAttackSpeed()
    {
        attackInterval = _statContainer.GetBaseStat().Attack_Interval;

        final_attackInterval = Mathf.Max(attackInterval * (1f - (_statContainer.AttackSpeed.FinalValue / attackInterval)), 2f);
    }

    public Health GetTarget()
    {
        return Target;
    }

    public void Turn(bool turnRight)
    {
        orientation.transform.localScale = turnRight? left : right;
    }

    private void Init()
    {
        //isAttacking = false;
    }

    protected void OnDeath(LevelSystem targ)
    {
        Target = null;
        
        //isAttacking = false;
        currentBarTween.Pause();
        // 죽음 애니메이션플레이
        onStateEnterBeahviors[eEnemyBehavior.dead]?.Invoke(health);

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

    public bool SearchForTarget()
    {
        // Create a circle around the transform's position with the specified scanDistance radius
        Vector2 circleCenter = transform.position;
        float circleRadius = scanDistance;

        // Perform a CircleCastAll to detect any colliders within the circle
        RaycastHit2D[] raycastHits = Physics2D.CircleCastAll(circleCenter, circleRadius, Vector2.zero, 0f, enemyLayer);

        if (raycastHits.Length > 0)
        {
            if (Target == null)
            {
                // Sort the raycastHits array by distance from the current transform's position
                raycastHits = raycastHits.OrderBy(hit => Vector2.Distance(hit.transform.position, transform.position)).ToArray();

                // Iterate through the sorted colliders within the circle
                foreach (RaycastHit2D hit in raycastHits)
                {
                    Health target = hit.transform.GetComponent<Health>();
                    if (target != null && !target.IsDead)
                    {
                        SetTarget(target);
                        return true;
                    }
                }
            }
        }

        return false;
    }

    protected void SetTarget(Health enemy)
    {
        Target = enemy;
        aiDestinationSetter.target = Target.transform;
        //Debug.Log("Target set: " + enemy.name);
    }
    public bool isAttacking;
    public void Attack()
    {

        if (basicAttack == null)
        {
            basicAttack = Resources.Load<EnemyBasicAttack>("ScriptableObject/Skills/EnemyBasicAttack");
        }
        if (Target == null) return;
        // 데미지 계산
        basicAttack.ApplyEffect(_statContainer, Target.GetComponent<StatContainer>());
        //playerDetector.PlayerHealth.TakeDamage(10);
    }

    public void StopMovement() => aiPath.canMove = false;

    public void StartMovement() => aiPath.canMove = true;
    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }
    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, scanDistance);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
