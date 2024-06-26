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
using System.Linq;
using Pathfinding;
using Litkey.AI;

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

    protected eBehavior currentBehavior;
    protected bool isGrounded;

    protected Health Target;
    protected Rigidbody2D rb2D;

    protected bool canMove;

    public bool CanMove => canMove;

    protected StatContainer _statContainer;
    protected LevelSystem _levelSystem;
    protected Health _health;
    public bool isDead;

    protected Material playerMat;
    public UnityEvent OnRevive;

    public UnityEvent OnAutoOn;
    public UnityEvent OnAutoOff;
    
    protected float startValue = 0f;

    public Vector2 moveDir;

    protected Sequence hitSequence;

    private DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> currentBarTween;

    protected SkillContainer _skillContainer;
    private PlayerInput _playerInput;

    private Player_AttackState playerAttackState;

    private StateMachine stateMachine;
    private CountdownTimer attackTimer;

    private AIPath _aiPath;
    private AIDestinationSetter _destinationSetter;

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
        _skillContainer = GetComponent<SkillContainer>();
        _playerInput = GetComponent<PlayerInput>();
        _aiPath = GetComponent<AIPath>();
        _destinationSetter = GetComponent<AIDestinationSetter>();
    }

    protected virtual void OnEnable()
    {
        _health.OnDeath.AddListener(Death);
        _health.OnHit.AddListener(HitAnim);
        _statContainer.OnStatSetupComplete.AddListener(UpdateMoveSpeed);
        _statContainer.OnStatSetupComplete.AddListener(UpdateAttackSpeedOnStart);
        _statContainer.MoveSpeed.OnValueChanged.AddListener(UpdateSpeed);
        _statContainer.AttackSpeed.OnValueChanged.AddListener(UpdateAttackSpeedOnChange);
    }

    protected virtual void OnDisable()
    {
        _health.OnHit.RemoveListener(HitAnim);
        _health.OnDeath.RemoveListener(Death);
        _statContainer.OnStatSetupComplete.RemoveListener(UpdateMoveSpeed);
        _statContainer.OnStatSetupComplete.RemoveListener(UpdateAttackSpeedOnStart);
        _statContainer.MoveSpeed.OnValueChanged.RemoveListener(UpdateSpeed);
        _statContainer.AttackSpeed.OnValueChanged.RemoveListener(UpdateAttackSpeedOnChange);
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        EnableMovement();
        stateMachine = new StateMachine();

        var idleState = new Player_IdleState();
    }

    public void MoveWithJoystick()
    {
        moveDir = _playerInput.JoystickDirection;
        moveSpeed = _statContainer.MoveSpeed.FinalValue;
        transform.position += (Vector3)moveDir * moveSpeed * Time.deltaTime;
        Turn(moveDir.x > 0);
        
    }

    #region StateMachine Condition Checks
    public bool IsDead()
    {
        return _health.IsDead;
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

    public bool Auto()
    {
        return isAuto;
    }

    public bool JoystickMoving()
    {
        return _playerInput.IsMovingJoystick;
    }

    public bool AttackCooldown()
    {
        return !attackTimer.IsFinished;
    }
    #endregion

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

    public virtual void Turn(bool turnRight)
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
        //CheckGrounded(); // 땅에 닿아잇는지를 체크
        
        

    }

    private void UseAttackOrSkill()
    {
        ActiveSkill usableSkill = _skillContainer.FindUsableSkill();
        if (usableSkill != null && TargetWithinAttackRange())
        {
            _skillContainer.UseActiveSkill(usableSkill, Target);
        }
        else
        {
            // Trigger normal attack animations or mechanics
            anim.SetFloat(_AttackState, Random.Range(0, 1f));
            anim.SetTrigger(_Attack);
        }
    }

    protected void SetTarget(Health enemy)
    {
        Target = enemy;
        //Debug.Log("Target set: " + enemy.name);
        _destinationSetter.target = Target.transform;
    }

    public Health GetTarget()
    {
        return Target;
    }

    public void DOSmoothWalk()
    {
        EnableMovement();
    }
    public void DOSmoothRun()
    {
        EnableMovement();
    }
    public void DoIdle()
    {
        DisableMovement();
    }
    



    protected virtual bool SearchForTarget()
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

    public void EnableMovement()
    {
        //Debug.Log("Can move now");
        canMove = true;
        _aiPath.canMove = canMove;
    }

    public virtual void DisableMovement()
    {
        canMove = false;
        _aiPath.canMove = canMove;
    }
    private void UpdateMoveSpeed(StatContainer statContainer)
    {
        this.runSpeed = statContainer.MoveSpeed.FinalValue;
    }

    private void UpdateSpeed(float speed)
    {
        this.runSpeed = speed;
    }

    private void UpdateAttackSpeedOnStart(StatContainer statContainer)
    {
        UpdateAS(statContainer.AttackSpeed.FinalValue);
    }

    private void UpdateAttackSpeedOnChange(float _as)
    {
        UpdateAS(_as);
    }

    public void RemoveTarget()
    {
        Target = null;
    }
    private void UpdateAS(float _as)
    {
        anim.SetFloat(_AttackSpeed, _as);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, scanDistance);
    }

    public void ToggleAuto()
    {
        isAuto = !isAuto;
        if (isAuto)
        {
            OnAutoOn?.Invoke();
        }
        else
        {
            OnAutoOff?.Invoke();
        }
    }

    public void ToggleAutoMode(bool toggle)
    {
        isAuto = toggle;
        if (isAuto)
        {
            OnAutoOn?.Invoke();
        }
        else
        {
            OnAutoOff?.Invoke();
        }
    }


    public void ChaseEnemy()
    {

    }

    public void Attack()
    {

    }

    //protected virtual void AttackAction()
    //{

    //    if (Target.IsDead)
    //    {
    //        Target = null;
    //        SwitchState(eBehavior.run);
    //    } else
    //    {
    //        anim.SetFloat(_AttackState, Random.Range(0, 1f));
    //        anim.SetTrigger(_Attack);
    //    }
    //    MasterAudio.PlaySound("일반검베기");
    //}

    //public void DamageAction()
    //{
    //    if (basicAttack == null)
    //    {
    //        basicAttack = Resources.Load<PlayerBasicAttack>("ScriptableObject/Skills/PlayerBasicAttack");
    //    }
    //    // 데미지 계산
    //    //var dmg = _statContainer.GetFinalDamage();
    //    //var dmg = _statContainer.GetDamageAgainst(Target.GetComponent<StatContainer>());
    //    if (Target == null) return;
    //    if (TargetWithinAttackRange())
    //        basicAttack.ApplyEffect(_statContainer, Target.GetComponent<StatContainer>());


    //    //Target.GetComponent<StatContainer>().Defend(dmg.damage);
    //    //Target.TakeDamage(_levelSystem, new List<Damage> { dmg });

    //}


}
