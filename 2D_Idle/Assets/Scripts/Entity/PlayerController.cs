using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Interface;
using UnityEngine.Events;
using DG.Tweening;
using Sirenix.OdinInspector;

public class PlayerController : MonoBehaviour
{
    // 플레이어 AI
    // 달려가서 평타로 시작
    [SerializeField] private Animator anim;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float scanDistance = 2f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private SpriteRenderer playerSprite;

    [SerializeField] private BasicAttack basicAttack;
    // 애니메이션
    private readonly int _isJumping = Animator.StringToHash("isJumping");
    private readonly int _isRunning = Animator.StringToHash("isRunning");
    private readonly int _isWalking = Animator.StringToHash("isWalking");
    private readonly int _isGround = Animator.StringToHash("isGround");
    private readonly int _AttackState = Animator.StringToHash("AttackState");
    private readonly int _Attack = Animator.StringToHash("Attack");
    
    private readonly int _Dead = Animator.StringToHash("Death");
    private readonly int _Revive = Animator.StringToHash("Revive");
    private readonly int _Hit = Animator.StringToHash("Hit");
    
    private readonly int _EnterCounter = Animator.StringToHash("EnterCounter");
    private readonly int _Parry = Animator.StringToHash("Parry");
    private eBehavior currentBehavior;
    private bool isGrounded;

    private Health Target;
    private Rigidbody2D rb2D;

    private bool canMove;

    private StatContainer _statContainer;
    private LevelSystem _levelSystem;
    private Health _health;
    public bool isDead;

    private Material playerMat;
    public UnityEvent OnRevive;

    private float startValue = 0f;
    Sequence hitSequence;
    private enum eBehavior
    {
        idle,
        walk, // search for enemy
        run,
        jump,
        attack,
        ability
    }

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        _statContainer = GetComponent<StatContainer>();
        _levelSystem = GetComponent<LevelSystem>();
        _health = GetComponent<Health>();
        playerMat = playerSprite.material;

        
    }

    private void OnEnable()
    {
        _health.OnDeath.AddListener(Death);
        _health.OnHit.AddListener(HitAnim);
    }

    private void OnDisable()
    {
        _health.OnHit.RemoveListener(HitAnim);
        _health.OnDeath.RemoveListener(Death);
    }

    // Start is called before the first frame update
    void Start()
    {
        
        EnableMovement();
        SwitchState(eBehavior.run);
    }

    private void Move()
    {
        int dir = playerSprite.flipX ? -1 : 1;
        transform.position += Vector3.right * dir * moveSpeed * Time.deltaTime;
    }



    private void Run()
    {
        //rb2D.velocity += new Vector2(1 * runSpeed, 0) * Time.deltaTime;
        //rb2D.MovePosition(rb2D.position + Vector2.right * runSpeed * Time.deltaTime);
        //rb2D.velocity = Vector2.right * runSpeed;
        int dir = playerSprite.flipX ? -1 : 1;
        transform.position += Vector3.right * dir * runSpeed * Time.deltaTime;
    }

    private void Death(LevelSystem levelSystem)
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("Played Dead animation");
        PlayDeath();
        //anim.SetTrigger(this._Dead);
    }

    [Button("PlayDead")]
    public void PlayDeath()
    {
        anim.Play(this._Dead);
    }

    private void HitAnim()
    {
        //anim.Play(_Hit);

        hitSequence = DOTween.Sequence();

        hitSequence.Append(DOTween.To(() => startValue, x => startValue = x, 0.5f, 0.1f)
            .OnUpdate(() =>
            {
                playerMat.SetFloat("_HitEffectBlend", startValue);
            }))
            .AppendInterval(0.1f) // Optional: Delay before the hit effect fades out
            .Append(DOTween.To(() => startValue, x => startValue = x, 0f, 0.1f)
            .OnUpdate(() =>
            {
                playerMat.SetFloat("_HitEffectBlend", startValue);
            }));


        //hitSequence.Restart();
    }

    public void Revive()
    {
        isDead = false;
        // 애니메이션
        anim.SetTrigger(this._Revive);
        // HP
        OnRevive?.Invoke();
        // 스킬 쿨다운 등등
    }

    public void Turn(bool turnRight)
    {
        playerSprite.flipX = !turnRight;
    }

    private void CheckGrounded()
    {
        var raycastHit = Physics2D.Raycast(transform.position, Vector2.down, 0.55f, groundLayer);
        Debug.DrawRay(transform.position, Vector2.down * 0.55f, Color.red, 0.3f);
        
        isGrounded = raycastHit;
        anim.SetBool(_isGround, isGrounded);

    }

    void Update()
    {
        CheckGrounded(); // 땅에 닿아잇는지를 체크
        
        
        Action(); // 기본 AI
    }


    private void SetTarget(Health enemy)
    {
        Target = enemy;
        Debug.Log("Target set: " + enemy.name);
        
    }

    private void Action()
    {
        if (isDead) return;

        //if (HasNoTarget())
        //{
        //    SwitchState(eBehavior.walk);
        //    return;
        //}
        switch(currentBehavior)
        {
            case eBehavior.idle:
                if (!HasNoTarget() && TargetWithinAttackRange())
                {
                    SwitchState(eBehavior.attack);
                }
                break;
            case eBehavior.walk:
                // 걸으면서 적을 찾는다
                if (canMove)
                    Move();
                    //MoveRight();
                // 적을 찾으면 적을향해 달려간다, 공격범위까지
                if (SearchForTarget())
                {
                    SwitchState(eBehavior.run);
                }
                break;
            case eBehavior.run:
                if (canMove)
                    Run();

                if (Target == null)
                {
                    SearchForTarget();

                }
                if (TargetWithinAttackRange())
                {
                    SwitchState(eBehavior.attack);
                }
                break;
            case eBehavior.jump:
                anim.SetBool(_isJumping, true);
                break;
            case eBehavior.attack:
                AttackAction();

                break;
            case eBehavior.ability:
                break;
        }
    }

    private void SwitchState(eBehavior behavior)
    {
        switch (currentBehavior)
        {
            case eBehavior.idle:
                
                break;
            case eBehavior.walk:
                anim.SetBool(_isWalking, false);
                break;
            case eBehavior.run:
                anim.SetBool(_isRunning, false);

                break;
            case eBehavior.jump:
 
                anim.SetBool(_isJumping, false);
                break;
            case eBehavior.attack:
  
                //anim.SetBool(isRunning, true);
                break;
            case eBehavior.ability:

                break;
        }

        currentBehavior = behavior;
        switch (currentBehavior)
        {
            case eBehavior.idle:
                anim.SetBool(_Parry, false);
                break;
            case eBehavior.walk:
                //anim.SetBool(isWalking, true);
                anim.SetBool(_isWalking, true);
                break;
            case eBehavior.run:

                anim.SetBool(_isRunning, true);

                break;
            case eBehavior.jump:

                anim.SetBool(_isJumping, true);
                break;
            case eBehavior.attack:
                canMove = false;
                //anim.SetBool(isRunning, true);
                break;
            case eBehavior.ability:

                break;
        }
    }

    public void DOSmoothWalk()
    {
        EnableMovement();
        SwitchState(eBehavior.walk);
    }

    public void DoIdle()
    {
        DisableMovement();
        SwitchState(eBehavior.idle);
    }

    public void EnterCounter()
    {
        anim.Play(_EnterCounter);
        
    }
    bool isParried = false;
    // call from other script
    public void DoParry(bool doParry)
    {
        if (Target == null) return;
        if (isDead) return;

        var enemyAI = Target.GetComponent<EnemyAI>();
        EnterCounter();
        if (doParry)
        {
            if (enemyAI != null && enemyAI.TryParry())
            {
                anim.SetBool(_Parry, true);
                isParried = true;
                // Handle successful parry for the player
            }
            else
            {
                // Handle failed parry attempt
                anim.SetBool(_Parry, false);
                isParried = false;
            }
        }
        else
        {
            anim.SetBool(_Parry, false);
            isParried = false;
        }
    }

    private bool TargetWithinAttackRange()
    {
        if (Target == null)
        {
            return false;
        }
        return Vector2.Distance(Target.transform.position, transform.position) <= attackRange;
    }

    private bool HasNoTarget()
    {
        return Target == null;
    }


    private bool SearchForTarget()
    {
        var raycastHit = Physics2D.Raycast(transform.position, Vector2.right, scanDistance, enemyLayer);
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

    public void EnableMovement()
    {
        //Debug.Log("Can move now");
        canMove = true;
    }

    public void DisableMovement()
    {
        canMove = false;
    }

    private void AttackAction()
    {
       
        if (Target.IsDead)
        {
            Target = null;
            SwitchState(eBehavior.run);
        } else
        {
            anim.SetFloat(_AttackState, Random.Range(0, 1f));
            anim.SetTrigger(_Attack);
        }
    }

    public void DamageAction()
    {
        if (basicAttack == null)
        {
            basicAttack = Resources.Load<BasicAttack>("ScriptableObject/Skills/PlayerBasicAttack");
        }
        // 데미지 계산
        //var dmg = _statContainer.GetFinalDamage();
        //var dmg = _statContainer.GetDamageAgainst(Target.GetComponent<StatContainer>());
        basicAttack.ApplyEffect(_statContainer, Target.GetComponent<StatContainer>());

        //Target.GetComponent<StatContainer>().Defend(dmg.damage);
        //Target.TakeDamage(_levelSystem, new List<Damage> { dmg });
        
    }


}
