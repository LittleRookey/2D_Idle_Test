using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // 애니메이션
    private int isJumping = Animator.StringToHash("isJumping");
    private int isRunning = Animator.StringToHash("isRunning");
    private int isWalking = Animator.StringToHash("isWalking");
    private int isGround = Animator.StringToHash("isGround");
    private int AttackState = Animator.StringToHash("AttackState");
    private int Attack = Animator.StringToHash("Attack");
    private eBehavior currentBehavior;
    private bool isGrounded;

    private Health Target;
    private Rigidbody2D rb2D;

    private bool canMove;

    private StatContainer _statContainer;
    private LevelSystem _levelSystem;
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
    }
    // Start is called before the first frame update
    void Start()
    {
        
        EnableMovement();
        SwitchState(eBehavior.run);
    }

    private void MoveRight()
    {
        //rb2D.MovePosition(rb2D.position + Vector2.right * moveSpeed * Time.deltaTime);
        //rb2D.velocity = Vector2.right * moveSpeed;
    }

    private void RunRight()
    {
        //rb2D.velocity += new Vector2(1 * runSpeed, 0) * Time.deltaTime;
        //rb2D.MovePosition(rb2D.position + Vector2.right * runSpeed * Time.deltaTime);
        //rb2D.velocity = Vector2.right * runSpeed;
        transform.position += Vector3.right * runSpeed * Time.deltaTime;
    }

    private void CheckGrounded()
    {
        var raycastHit = Physics2D.Raycast(transform.position, Vector2.down, 0.55f, groundLayer);
        Debug.DrawRay(transform.position, Vector2.down * 0.55f, Color.red, 0.3f);
        
        isGrounded = raycastHit;
        anim.SetBool(isGround, isGrounded);
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
                    RunRight();
                    //MoveRight();
                // 적을 찾으면 적을향해 달려간다, 공격범위까지
                if (SearchForTarget())
                {
                    SwitchState(eBehavior.run);
                }
                break;
            case eBehavior.run:
                if (canMove)
                    RunRight();

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
                anim.SetBool(isJumping, true);
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
                anim.SetBool(isWalking, false);
                break;
            case eBehavior.run:
                anim.SetBool(isRunning, false);

                break;
            case eBehavior.jump:
 
                anim.SetBool(isJumping, false);
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
                anim.SetBool(isWalking, true);
                break;
            case eBehavior.run:

                anim.SetBool(isRunning, true);

                break;
            case eBehavior.jump:

                anim.SetBool(isJumping, true);
                break;
            case eBehavior.attack:
                canMove = false;
                //anim.SetBool(isRunning, true);
                break;
            case eBehavior.ability:

                break;
        }
    }

    public void SmoothWalk()
    {
        EnableMovement();
        SwitchState(eBehavior.walk);
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

    private void AttackAction()
    {
       
        if (Target.IsDead)
        {
            Target = null;
            SwitchState(eBehavior.run);
        } else
        {
            anim.SetFloat(AttackState, Random.Range(0, 1f));
            anim.SetTrigger(Attack);
        }
    }

    public void DamageAction()
    {
        // 데미지 계산
        var dmg = _statContainer.GetFinalDamage();
        //Target.GetComponent<StatContainer>().Defend(dmg.damage);
        Target.TakeDamage(_levelSystem, new List<Damage> { dmg });
        
    }

}
