using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Litkey.Interface;

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
    // 애니메이션
    private int _isJumping = Animator.StringToHash("isJumping");
    private int _isRunning = Animator.StringToHash("isRunning");
    private int _isWalking = Animator.StringToHash("isWalking");
    private int _isGround = Animator.StringToHash("isGround");
    private int _AttackState = Animator.StringToHash("AttackState");
    private int _Attack = Animator.StringToHash("Attack");
    
    private int _Dead = Animator.StringToHash("Death");
    private int _Revive = Animator.StringToHash("Revive");

    private eBehavior currentBehavior;
    private bool isGrounded;

    private Health Target;
    private Rigidbody2D rb2D;

    private bool canMove;

    private StatContainer _statContainer;
    private LevelSystem _levelSystem;
    private Health _health;
    public bool isDead;
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
    }

    private void OnEnable()
    {
        _health.OnDeath += Death;
    }

    private void OnDisable()
    {
        _health.OnDeath -= Death;
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
        isDead = true;
        anim.SetTrigger(this._Dead);
    }

    private void Revive(LevelSystem levelSystem)
    {
        isDead = false;
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
        Debug.Log("Can move now");
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
        // 데미지 계산
        var dmg = _statContainer.GetFinalDamage();
        _statContainer.GetDamageAgainst(Target.GetComponent<StatContainer>());

        //Target.GetComponent<StatContainer>().Defend(dmg.damage);
        Target.TakeDamage(_levelSystem, new List<Damage> { dmg });
        
    }


}
