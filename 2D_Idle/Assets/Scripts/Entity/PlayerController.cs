using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Interface;
using UnityEngine.Events;
using DG.Tweening;
using Sirenix.OdinInspector;
using DarkTonic.MasterAudio;
using Litkey.Utility;
using Litkey.Skill;

public class PlayerController : MonoBehaviour
{
    // 플레이어 AI
    // 달려가서 평타로 시작
    [SerializeField] protected Animator anim;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float runSpeed;
    [SerializeField] protected float scanDistance = 2f;
    [SerializeField] protected float attackRange = 1.5f;
    [SerializeField] protected LayerMask enemyLayer;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected SpriteRenderer playerSprite;

    [SerializeField] protected PlayerBasicAttack basicAttack;
    // 애니메이션
    protected readonly int _isJumping = Animator.StringToHash("isJumping");
    protected readonly int _isRunning = Animator.StringToHash("isRunning");
    protected readonly int _isWalking = Animator.StringToHash("isWalking");
    protected readonly int _isGround = Animator.StringToHash("isGround");
    protected readonly int _AttackState = Animator.StringToHash("AttackState");
    protected readonly int _Attack = Animator.StringToHash("Attack");

    protected readonly int _AttackSpeed = Animator.StringToHash("AttackSpeed");

    protected readonly int _Dead = Animator.StringToHash("Death");
    protected readonly int _Revive = Animator.StringToHash("Revive");
    protected readonly int _Hit = Animator.StringToHash("Hit");

    protected readonly int _EnterCounter = Animator.StringToHash("EnterCounter");
    protected readonly int _Parry = Animator.StringToHash("Parry");
    protected eBehavior currentBehavior;
    protected bool isGrounded;

    protected Health Target;
    protected Rigidbody2D rb2D;

    protected bool canMove;

    protected StatContainer _statContainer;
    protected LevelSystem _levelSystem;
    protected Health _health;
    public bool isDead;

    protected Material playerMat;
    public UnityEvent OnRevive;

    protected float startValue = 0f;
    protected Sequence hitSequence;

    private DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> currentBarTween;

    
    public bool isAuto;
    protected enum eBehavior
    {
        idle,
        walk, // search for enemy
        run,
        chase,
        jump,
        attack,
        ability
    }

    protected virtual void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        _statContainer = GetComponent<StatContainer>();
        _levelSystem = GetComponent<LevelSystem>();
        _health = GetComponent<Health>();
        playerMat = playerSprite.material;

        
    }

    protected virtual void OnEnable()
    {
        _health.OnDeath.AddListener(Death);
        _health.OnHit.AddListener(HitAnim);
    }

    protected virtual void OnDisable()
    {
        _health.OnHit.RemoveListener(HitAnim);
        _health.OnDeath.RemoveListener(Death);
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
        EnableMovement();
        SwitchState(eBehavior.run);
    }

    protected virtual void Move()
    {
        int dir = playerSprite.flipX ? -1 : 1;
        transform.position += Vector3.right * dir * moveSpeed * Time.deltaTime;
    }



    protected virtual void Run()
    {
        //rb2D.velocity += new Vector2(1 * runSpeed, 0) * Time.deltaTime;
        //rb2D.MovePosition(rb2D.position + Vector2.right * runSpeed * Time.deltaTime);
        //rb2D.velocity = Vector2.right * runSpeed;
        int dir = playerSprite.flipX ? -1 : 1;
        transform.position += Vector3.right * dir * runSpeed * Time.deltaTime;
    }

    protected void Death(LevelSystem levelSystem)
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

    protected void HitAnim()
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
        
        // 애니메이션
        anim.Play(this._Revive);
        //Invoke(nameof(), 2f);
        var currentBar = BarCreator.CreateFillBar(transform.position + Vector3.up * 1.2f, transform, false);

        currentBar.SetOuterColor(Color.black);
        currentBar.SetInnerColor(Color.red);

        currentBarTween = currentBar.StartFillBar(2f, () =>
        {
            // 부활시간 끝나면 리바이브하기
            isDead = false;
            //onAttackExit();

            MoveFromReviveToBattleMode();
            currentBarTween = null;
            BarCreator.ReturnBar(currentBar);

            OnRevive?.Invoke();
        });
        // HP
        
        // 스킬 쿨다운 등등
    }

    public void MoveFromReviveToBattleMode()
    {
        anim.SetTrigger(this._Revive);
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

    protected virtual void Update()
    {
        CheckGrounded(); // 땅에 닿아잇는지를 체크
        
        
        Action(); // 기본 AI
    }


    protected void SetTarget(Health enemy)
    {
        Target = enemy;
        //Debug.Log("Target set: " + enemy.name);
        
    }

    public Health GetTarget()
    {
        return Target;
    }

    protected virtual void Action()
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

    protected virtual void SwitchState(eBehavior behavior)
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
            case eBehavior.chase:
                anim.SetBool(_isRunning, false);
                break;
            case eBehavior.jump:
 
                anim.SetBool(_isJumping, false);
                break;
            case eBehavior.attack:
                //if (Target.IsDead)
                //{
                //    Target = null;
                //    SwitchState(eBehavior.idle);
                //    return;
                //}
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
            case eBehavior.chase:
                anim.SetBool(_isRunning, true);
                //canMove = true;
                break;
            case eBehavior.jump:

                anim.SetBool(_isJumping, true);
                break;
            case eBehavior.attack:
                canMove = false;
                anim.SetBool(_isRunning, false);
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
    public void DOSmoothRun()
    {
        EnableMovement();
        SwitchState(eBehavior.run);
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
                MasterAudio.PlaySound("패링성공");
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

    protected bool TargetWithinAttackRange()
    {
        if (Target == null)
        {
            return false;
        }
        return Vector2.Distance(Target.transform.position, transform.position) <= attackRange;
    }

    protected bool HasNoTarget()
    {
        return Target == null;
    }


    protected virtual bool SearchForTarget()
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

    public virtual void DisableMovement()
    {
        canMove = false;
    }

    protected virtual void AttackAction()
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
            basicAttack = Resources.Load<PlayerBasicAttack>("ScriptableObject/Skills/PlayerBasicAttack");
        }
        // 데미지 계산
        //var dmg = _statContainer.GetFinalDamage();
        //var dmg = _statContainer.GetDamageAgainst(Target.GetComponent<StatContainer>());
        if (Target == null) return;
        if (TargetWithinAttackRange())
            basicAttack.ApplyEffect(_statContainer, Target.GetComponent<StatContainer>());
        

        //Target.GetComponent<StatContainer>().Defend(dmg.damage);
        //Target.TakeDamage(_levelSystem, new List<Damage> { dmg });

    }


}
