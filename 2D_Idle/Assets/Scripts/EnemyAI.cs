using Litkey.Interface;
using Litkey.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Litkey.Stat;

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
        boxCollider2D = GetComponentInChildren<BoxCollider2D>();
    }

    private void OnEnable()
    {
        Init();
        OnStun.AddListener(onStunEnter);
        OnStunExit.AddListener(onStunExit);
        final_attackInterval = Mathf.Max(attackInterval * (1f - (_statContainer.AttackSpeed.FinalValue / attackInterval)), 0.5f);
        health.OnDeath.AddListener(OnDeath);
        health.onTakeDamage += OnHitEnter;
    }

    private void OnDisable()
    {

        OnStun.RemoveListener(onStunEnter);
        OnStunExit.RemoveListener(onStunExit);
        health.OnDeath.RemoveListener(OnDeath);
    }

    // Start is called before the first frame update
    void Start()
    {
        currentBehavior = eEnemyBehavior.idle;
        //SwitchState(eEnemyBehavior.idle);
        
        
    }

    public Health GetTarget()
    {
        return Target;
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
    private bool canParry;
    private bool isParried;
    [SerializeField] private float parryTime = 0.5f;
    private BoxCollider2D boxCollider2D;
    // 패리 로직: 공격떄 패리박스를 넣음, 타겟이 패리박스가 열렸을떄 isParried를 트루로 만들면 데미지 공식 빗겨가고 패링당한 애니메이션 실행
    // 만약 방어를 그 전에하고 유지하면 데미지 반감, 패링박스 열리고 닫히기전에 하면 패링, 이후에 패링하면 아무것도 없고 데미지공식 그대로 실행
    private void onAttackEnter()
    {
        // 몇초후에 공격 속행
        isAttacking = true;
        currentBar = BarCreator.CreateFillBar(transform.position - Vector3.down * 1.5f, transform, false);
        currentBar.SetOuterColor(Color.black);
        var parryActivateTime = final_attackInterval - parryTime; 
        attackTimer = 0f;
        currentBarTween = currentBar.StartFillBar(final_attackInterval, () => 
        { 
            currentBarTween = null;

            isAttacking = false;
            if (!isParried) DamageAction();

            //onAttackExit();
            SwitchState(eEnemyBehavior.attack);
            BarCreator.ReturnBar(currentBar);
        });
    }

    private IEnumerator ActivateParrying(int targetNum)
    {
        yield return new WaitForSeconds(final_attackInterval - parryTime);
        canParry = true;

        yield return new WaitForSeconds(parryTime);
        canParry = false;


    }

    // call from attack dotween animation
    public void onAttackExit()
    {
        //DamageAction();
        //isAttacking = false;
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

    public void DamageAction()
    {
        if (Target == null) return;
        // 데미지 계산
        var dmg = _statContainer.GetFinalDamage();
        _statContainer.GetDamageAgainst(Target.GetComponent<StatContainer>());
        //Target.GetComponent<StatContainer>().Defend(dmg.damage);
        Target.TakeDamage(_statContainer, new List<Damage> { dmg });
    }

    // Update is called once per frame
    void Update()
    {
        Action();
    }
}
