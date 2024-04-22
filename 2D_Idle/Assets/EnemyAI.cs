using Litkey.Interface;
using Litkey.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class EnemyAI : MonoBehaviour
{
    private enum eEnemyBehavior
    {
        idle,
        attack,
        dead,
        hit,
        stun,
    };
    [SerializeField] private SpriteRenderer allySprite;

    [SerializeField] private float scanDistance = 3f;
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private LayerMask enemyLayer;
    private float attackInterval;
    private float final_attackInterval;

    private float attackTimer;

    private Health Target;

    private eEnemyBehavior currentBehavior;

    private EnemyAnimationHook anim;

    [HideInInspector] public UnityEvent<Health> OnIdle;
    [HideInInspector] public UnityEvent<Health> OnAttack;
    [HideInInspector] public UnityEvent<Health> OnDead;
    [HideInInspector] public UnityEvent<Health> OnHit;
    [HideInInspector] public UnityEvent<Health> OnStun;

    [HideInInspector] public UnityEvent<Health> OnIdleExit;
    [HideInInspector] public UnityEvent<Health> OnAttackExit;
    [HideInInspector] public UnityEvent<Health> OnDeadExit;
    [HideInInspector] public UnityEvent<Health> OnHitExit;
    [HideInInspector] public UnityEvent<Health> OnStunExit;

    Dictionary<eEnemyBehavior, UnityEvent<Health>> onStateEnterBeahviors;
    Dictionary<eEnemyBehavior, UnityEvent<Health>> onStateExitBeahviors;

    private StatContainer _statContainer;

    private bool stopAttackTimer;

    private DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> currentBarTween;

    private Health health;
    

    private void Awake()
    {
        onStateEnterBeahviors = new Dictionary<eEnemyBehavior, UnityEvent<Health>>()
        {
            { eEnemyBehavior.idle, OnIdle},
            { eEnemyBehavior.attack, OnAttack },
            { eEnemyBehavior.hit, OnHit },
            { eEnemyBehavior.dead, OnDead },
            { eEnemyBehavior.stun, OnStun },
        };

        onStateExitBeahviors = new Dictionary<eEnemyBehavior, UnityEvent<Health>>()
        {
            { eEnemyBehavior.idle, OnIdleExit},
            { eEnemyBehavior.attack, OnAttackExit },
            { eEnemyBehavior.hit, OnHitExit },
            { eEnemyBehavior.dead, OnDeadExit },
            { eEnemyBehavior.stun, OnStunExit },
        };

        _statContainer = GetComponent<StatContainer>();
        health = GetComponent<Health>();
        attackInterval = _statContainer.GetBaseStat().Attack_Interval;
    }

    private void OnEnable()
    {
        Init();
        OnStun.AddListener(onStunEnter);
        OnStunExit.AddListener(onStunExit);
        final_attackInterval = Mathf.Max(attackInterval * (1f - (_statContainer.AttackSpeed.FinalValue / attackInterval)), 0.5f);
        health.OnDeath += OnDeath;
        health.onTakeDamage += OnHitEnter;
    }

    private void OnDisable()
    {

        OnStun.RemoveListener(onStunEnter);
        OnStunExit.RemoveListener(onStunExit);
        health.OnDeath -= OnDeath;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentBehavior = eEnemyBehavior.idle;
        //SwitchState(eEnemyBehavior.idle);
        
        
    }

    private void SwitchState(eEnemyBehavior behavior)
    {
        onStateExitBeahviors[currentBehavior]?.Invoke(Target);
        currentBehavior = behavior;
        onStateEnterBeahviors[behavior]?.Invoke(Target);
    }
    private bool isAttacking;
    private void Action()
    {
        switch(currentBehavior)
        {
            case eEnemyBehavior.idle:
                // 적을 찾기
                SearchForTarget();
                if (!HasNoTarget() && TargetWithinAttackRange() && !isAttacking)
                {

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
        allySprite.transform.localPosition = Vector3.zero;
        allySprite.transform.localRotation = Quaternion.Euler(Vector3.zero);
        allySprite.transform.localScale = Vector3.one;
        isAttacking = false;
    }

    private BarTemplate currentBar;
    private void onAttackEnter()
    {
        // 몇초후에 공격 속행
        isAttacking = true;
        currentBar = BarCreator.CreateFillBar(transform.position - Vector3.down * 1.5f, transform, false);
        currentBar.SetOuterColor(Color.black);

        attackTimer = 0f;
        currentBarTween = currentBar.StartFillBar(final_attackInterval, () => 
        { 
            currentBarTween = null;

            //isAttacking = false;

            SwitchState(eEnemyBehavior.attack);

            BarCreator.ReturnBar(currentBar);
        });
    }

    // call from attack dotween animation
    public void onAttackExit()
    {
        isAttacking = false;
        SwitchState(eEnemyBehavior.idle);
    }

    private void onStunEnter(Health targ)
    {
        currentBarTween?.Pause();
        stopAttackTimer = true;
    }

    private void onStunExit(Health targ)
    {
        currentBarTween?.PlayForward();
        stopAttackTimer = false;
    }

    private void OnDeath(LevelSystem targ)
    {
        Target = null;
        SwitchState(eEnemyBehavior.idle);
        stopAttackTimer = false;
        //isAttacking = false;
        currentBarTween.Pause();
        // 죽음 애니메이션플레이
        onStateEnterBeahviors[eEnemyBehavior.dead]?.Invoke(health);
        if (currentBar.gameObject.activeInHierarchy)
            BarCreator.ReturnBar(currentBar);
    }

    // called from animation
    public void OnDeathExit()
    {
        SpawnManager.Instance.TakeToPool(health);
    }

    private void OnHitEnter(float current, float max)
    {
        onStateEnterBeahviors[eEnemyBehavior.hit]?.Invoke(health);
    }

    private bool HasNoTarget()
    {
        return Target == null;
    }

    private bool TargetWithinAttackRange()
    {
        if (Target == null)
        {
            return false;
        }
        return Vector2.Distance(Target.transform.position, transform.position) <= attackRange;
    }

    private bool SearchForTarget()
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

    private void SetTarget(Health enemy)
    {
        Target = enemy;
        Debug.Log("Target set: " + enemy.name);
    }

    // Update is called once per frame
    void Update()
    {
        Action();
    }
}
