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

    protected float attackTimer;

    protected Health Target;

    protected eEnemyBehavior currentBehavior;

    protected EnemyAnimationHook anim;

    public UnityEvent<Health> OnIdle= new();
    public UnityEvent<Health> OnAttack = new();
    [HideInInspector] public UnityEvent<Health> OnChase = new();
    [HideInInspector] public UnityEvent<Health> OnDead = new();
    [HideInInspector] public UnityEvent<Health> OnHit = new();
    [HideInInspector] public UnityEvent<Health> OnStun = new();

    [HideInInspector] public UnityEvent<Health> OnIdleExit = new();
    [HideInInspector] public UnityEvent<Health> OnAttackExit = new();
    [HideInInspector] public UnityEvent<Health> OnChaseExit = new();
    [HideInInspector] public UnityEvent<Health> OnDeadExit = new();
    [HideInInspector] public UnityEvent<Health> OnHitExit = new();
    [HideInInspector] public UnityEvent<Health> OnStunExit = new();

    protected Dictionary<eEnemyBehavior, UnityEvent<Health>> onStateEnterBeahviors;
    protected Dictionary<eEnemyBehavior, UnityEvent<Health>> onStateExitBeahviors;

    protected StatContainer _statContainer;

    protected bool stopAttackTimer;

    protected DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> currentBarTween;

    protected Health health;

    protected Material enemyMat;

    protected Vector2 moveDir;

    private AIPath aiPath;

    private StateMachine stateMachine;
    Vector3 right = Vector3.one;
    Vector3 left = new Vector3(-1f, 1f, 1f);

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

        onStateExitBeahviors = new Dictionary<eEnemyBehavior, UnityEvent<Health>>()
        {
            { eEnemyBehavior.idle, OnIdleExit},
            { eEnemyBehavior.attack, OnAttackExit },
            {eEnemyBehavior.chase, OnChaseExit },
            { eEnemyBehavior.hit, OnHitExit },
            { eEnemyBehavior.dead, OnDeadExit },
            { eEnemyBehavior.stun, OnStunExit },
        };

        _statContainer = GetComponent<StatContainer>();
        health = GetComponent<Health>();
        attackInterval = _statContainer.GetBaseStat().Attack_Interval;
        boxCollider2D = GetComponentInChildren<BoxCollider2D>();
        aiPath = GetComponent<AIPath>();
    }

    protected void OnEnable()
    {
        Init();
        OnStun.AddListener(onStunEnter);
        OnStunExit.AddListener(onStunExit);

        UpdateAttackSpeed();
        health.OnDeath.AddListener(OnDeath);
        health.onTakeDamage += OnHitEnter;
    }

    protected void OnDisable()
    {

        OnStun.RemoveListener(onStunEnter);
        OnStunExit.RemoveListener(onStunExit);
        health.OnDeath.RemoveListener(OnDeath);
        health.onTakeDamage -= OnHitEnter;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentBehavior = eEnemyBehavior.idle;
        //SwitchState(eEnemyBehavior.idle);
        stateMachine = new StateMachine();

        
    }
    void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

    public void SetMovePosition(Vector3 movePosition, UnityAction onReachedMovePosition=null)
    {
        aiPath.destination = movePosition;
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

    protected void SwitchState(eEnemyBehavior behavior)
    {
        onStateExitBeahviors[currentBehavior]?.Invoke(Target);
        currentBehavior = behavior;
        onStateEnterBeahviors[behavior]?.Invoke(Target);
    }
    protected bool isAttacking;
    protected virtual void Action()
    {
        switch(currentBehavior)
        {
            case eEnemyBehavior.idle:
                // 적을 찾기
                SearchForTarget();
                //Debug.Log($"HasNoTarget: {!HasNoTarget()}\nTargetWithinAttackRange: {TargetWithinAttackRange()}\nisAttacking: {!isAttacking}");

                if (!HasNoTarget() && TargetWithinAttackRange() && !isAttacking)
                {
                    Debug.Log("On Attack Enter");
                    onAttackEnter();
                }
                // 적 찾으면 
                break;
            case eEnemyBehavior.attack:
                //attack 1 0.8 / 2 0.6 / 3 0.4 / 4 0.2 / 5  
                //if (!stopAttackTimer)
                //    attackTimer += Time.deltaTime;

                //if (attackTimer >= final_attackInterval)
                //{

                //    attackTimer = 0f;
                //}
                
                SwitchState(eEnemyBehavior.idle);
                
                break;
            case eEnemyBehavior.dead:
                break;
            case eEnemyBehavior.hit:
                break;
            case eEnemyBehavior.stun:
                break;

        }
    }

    private void Init()
    {
        isAttacking = false;
    }

    private BarTemplate currentBar;
    public bool canParry;
    private bool isParried;
    [SerializeField] private float parryTime = 0.5f;
    private BoxCollider2D boxCollider2D;
    // 패리 로직: 공격떄 패리박스를 넣음, 타겟이 패리박스가 열렸을떄 isParried를 트루로 만들면 데미지 공식 빗겨가고 패링당한 애니메이션 실행
    // 만약 방어를 그 전에하고 유지하면 데미지 반감, 패링박스 열리고 닫히기전에 하면 패링, 이후에 패링하면 아무것도 없고 데미지공식 그대로 실행
    protected virtual void onAttackEnter()
    {
        // 몇초후에 공격 속행

        isAttacking = true;

        currentBar = BarCreator.CreateFillBar(transform.position - Vector3.down * 1.5f, transform, false);

        currentBar.SetOuterColor(Color.black);
        currentBar.SetInnerColor(Color.green);

        var parryActivateTime = final_attackInterval - parryTime; 

        attackTimer = 0f;

        currentBarTween = currentBar.StartFillBar(final_attackInterval, () => 
        {

            isAttacking = false;

            if (!isParried) DamageAction();


            //onAttackExit();
            SwitchState(eEnemyBehavior.attack);

            BarCreator.ReturnBar(currentBar);

            currentBarTween = null;

        });
        StartCoroutine(ActivateParrying(1));
    }


    SpriteRenderer shape;
    private IEnumerator ActivateParrying(int targetNum)
    {
        yield return new WaitForSeconds(final_attackInterval - parryTime);
        canParry = true;
        shape = ShapeCreator.CreateCircle(transform.position + Vector3.up * 1.2f, Color.white);
        health.OnDeath.AddListener((LevelSystem lvl) => ShapeCreator.ReturnShape(shape));

        yield return new WaitForSeconds(parryTime);
        ShapeCreator.ReturnShape(shape);
        health.OnDeath.RemoveListener((LevelSystem lvl) => ShapeCreator.ReturnShape(shape));
        isParried = false;
        canParry = false;


    }


    protected void onStunEnter(Health targ)
    {
        stopAttackTimer = true;
    }

    protected void onStunExit(Health targ)
    {
        stopAttackTimer = false;
    }

    protected void OnDeath(LevelSystem targ)
    {
        Target = null;
        SwitchState(eEnemyBehavior.idle);
        stopAttackTimer = false;
        //isAttacking = false;
        currentBarTween.Pause();
        // 죽음 애니메이션플레이
        onStateEnterBeahviors[eEnemyBehavior.dead]?.Invoke(health);

        ShapeCreator.ReturnShape(shape);
        if (currentBar != null && currentBar.gameObject.activeInHierarchy)
            BarCreator.ReturnBar(currentBar);
    }

    // called from animation
    public void OnDeathExit()
    {
        ShapeCreator.ReturnShape(shape);
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

    protected virtual bool SearchForTarget()
    {
        var raycastHit = Physics2D.Raycast(transform.position, Vector2.left, scanDistance, enemyLayer);
        Debug.DrawRay(transform.position, Vector2.right * scanDistance, Color.red, 0.3f);
        if (raycastHit)
        {
            if (Target == null)
            {
                var target = raycastHit.transform.GetComponent<Health>();
                if (!target.IsDead)
                {
                    SetTarget(target);
                    return true;
                }
                return false;
            }
        }
        return false;
    }

    protected void SetTarget(Health enemy)
    {
        Target = enemy;
        //Debug.Log("Target set: " + enemy.name);
    }

    public void DamageAction()
    {
        if (isParried)
        {
            isParried = false;
            return;
        }

        if (basicAttack == null)
        {
            basicAttack = Resources.Load<EnemyBasicAttack>("ScriptableObject/Skills/EnemyBasicAttack");
        }
        if (Target == null) return;
        // 데미지 계산

        basicAttack.ApplyEffect(_statContainer, Target.GetComponent<StatContainer>());

        if (Target.IsDead)
        {
            Target = null;
            SwitchState(eEnemyBehavior.idle);
        }


    }

    public bool TryParry()
    {
        Debug.Log("canparry: " + canParry);
        Debug.Log("isParried: " + isParried);
        if (canParry && !isParried)
        {
            isParried = true;
            // Handle successful parry
            //onStateEnterBeahviors[eEnemyBehavior.parry]?.Invoke(health);
            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        Action();
    }
}
