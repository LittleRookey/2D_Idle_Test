using Litkey.Stat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // �÷��̾� AI
    // �޷����� ��Ÿ�� ����
    [SerializeField] private Animator anim;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float scanDistance = 2f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private SpriteRenderer playerSprite;
    // �ִϸ��̼�
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

    public void Turn(bool turnRight)
    {
        playerSprite.flipX = !turnRight;
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
        CheckGrounded(); // ���� ����մ����� üũ
        
        
        Action(); // �⺻ AI
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
                // �����鼭 ���� ã�´�
                if (canMove)
                    Move();
                    //MoveRight();
                // ���� ã���� �������� �޷�����, ���ݹ�������
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
            anim.SetFloat(AttackState, Random.Range(0, 1f));
            anim.SetTrigger(Attack);
        }
    }

    public void DamageAction()
    {
        // ������ ���
        var dmg = _statContainer.GetFinalDamage();
        //Target.GetComponent<StatContainer>().Defend(dmg.damage);
        Target.TakeDamage(_levelSystem, new List<Damage> { dmg });
        
    }

}
