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


    protected readonly int _Interacting = Animator.StringToHash("Interacting");

    protected readonly int _AttackSpeed = Animator.StringToHash("AttackSpeed");

    protected readonly int _Dead = Animator.StringToHash("Death");
    protected readonly int _Revive = Animator.StringToHash("Revive");
    protected readonly int _Hit = Animator.StringToHash("Hit");

    protected eBehavior currentBehavior;
    protected bool isGrounded;

    protected Health Target;

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
    [SerializeField]
    public StateMachine stateMachine;
    private CountdownTimer attackTimer;

    private AIPath _aiPath;
    private AIDestinationSetter _destinationSetter;

    private ResourceInteractor _interactor;
    public bool isAuto;
    public bool isStunned;

    public Transform goldTarget;
    public Transform bagTarget;

    List<IAttackDecorator> attackDecorators;

    Vector3 one;
    Vector3 minusOne;
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

    protected void Awake()
    {
        attackDecorators = new List<IAttackDecorator>();

        one = Vector3.one;
        minusOne = new Vector3(-1f, 1f, 1f);

        _statContainer = GetComponent<StatContainer>();
        _levelSystem = GetComponent<LevelSystem>();
        _health = GetComponent<Health>();
        playerMat = playerSprite.material;
        _skillContainer = GetComponent<SkillContainer>();
        _playerInput = GetComponent<PlayerInput>();
        _aiPath = GetComponent<AIPath>();
        _destinationSetter = GetComponent<AIDestinationSetter>();
        _interactor = GetComponent<ResourceInteractor>();

        stateMachine = new StateMachine();

        var stunState = new Player_StunState(this, anim, "stun");
        var idleState = new Player_IdleState(this, anim, "idle");
        var moveState = new Player_MoveState(this, anim, "move");
        var attackState = new Player_AttackState(this, anim, "attack");
        var chaseState = new Player_ChaseState(this, anim, "chase");
        var deathState = new Player_DeathState(this, anim, "death");

        attackTimer = attackState.attackTimer;

        Any(stunState, new FuncPredicate(() => !IsDead() && IsStunned()));
        At(stunState, idleState, new FuncPredicate(() => !IsDead() && !IsStunned()));
        Any(moveState, new FuncPredicate(() => !IsStunned() && JoystickMoving() && !IsDead() && !IsInteracting()));
        At(moveState, idleState, new FuncPredicate(() => !JoystickMoving()));

        At(idleState, chaseState, new FuncPredicate(() => !HasNoTarget() && !TargetWithinAttackRange() && Auto()));
        At(chaseState, attackState, new FuncPredicate(() => !HasNoTarget() && TargetWithinAttackRange() && !AttackCooldown() && Auto()));
        At(attackState, idleState, new FuncPredicate(() => HasNoTarget() || !Auto()));

        At(idleState, attackState, new FuncPredicate(() => !HasNoTarget() && TargetWithinAttackRange() && !AttackCooldown() && Auto()));
        At(attackState, chaseState, new FuncPredicate(() => !HasNoTarget() && !TargetWithinAttackRange() && Auto() && AttackCooldown()));
        At(chaseState, idleState, new FuncPredicate(() => HasNoTarget() || !Auto()));

        Any(deathState, new FuncPredicate(() => IsDead()));
        At(deathState, idleState, new FuncPredicate(() => !IsDead()));

        stateMachine.SetState(idleState);
    }

    protected void OnEnable()
    {
        _health.OnDeath.AddListener(Death);
        _health.OnHit.AddListener(HitAnim);
        _statContainer.OnStatSetupComplete.AddListener(UpdateMoveSpeed);
        _statContainer.OnStatSetupComplete.AddListener(UpdateAttackSpeedOnStart);
       
    }

    protected void OnDisable()
    {
        _health.OnHit.RemoveListener(HitAnim);
        _health.OnDeath.RemoveListener(Death);
        _statContainer.OnStatSetupComplete.RemoveListener(UpdateMoveSpeed);
        _statContainer.OnStatSetupComplete.RemoveListener(UpdateAttackSpeedOnStart);
        //_statContainer.MoveSpeed.OnValueChanged.RemoveListener(UpdateSpeed);
        //_statContainer.AttackSpeed.OnValueChanged.RemoveListener(UpdateAttackSpeedOnChange);
    }

    // Start is called before the first frame update
    protected void Start()
    {
        EnableMovement();
        //_aiPath.endReachedDistance = attackRange;

        _statContainer.MoveSpeed.OnValueChanged.AddListener(UpdateSpeed);
        _statContainer.AttackSpeed.OnValueChanged.AddListener(UpdateAttackSpeedOnChange);


        stateMachine.SetToBaseState();


        //UpdateMoveSpeed();
        ToggleAuto();
    }

    void At(IState from, IState to, IPredicate condition, bool hasExitTime = false, float exitTime = 0.0f) => stateMachine.AddTransition(from, to, condition, hasExitTime, exitTime);
    void Any(IState to, IPredicate condition, bool hasExitTime = false, float exitTime = 0.0f) => stateMachine.AddAnyTransition(to, condition, hasExitTime, exitTime);

    #region StateMachine Condition Checks

    public bool IsInteracting()
    {
        return _interactor.IsInteracting;
    }

    public bool IsDead()
    {
        return _health.IsDead;
    }

    public bool TargetWithinAttackRange()
    {
        if (Target == null)
        {
            return false;
        }
        return Vector2.Distance(Target.transform.position, transform.position) <= attackRange;
    }

    public bool IsStunned()
    {
        return isStunned;
    }

    public bool HasNoTarget()
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


    #region AttackDecorators
    public List<IAttackDecorator> GetAttackDecorators()
    {
        return attackDecorators;
    }

    public void AddAttackDecorator(IAttackDecorator deco)
    {
        attackDecorators.Add(deco);
    }


    #endregion
    public void Stun(float stunTime)
    {
        StartCoroutine(StartStun(stunTime));
    }

    private IEnumerator StartStun(float stunTime)
    {
        isStunned = true;
        yield return new WaitForSeconds(stunTime);
        isStunned = false;
    }

    public void PlayMineInteract()
    {
        anim.CrossFade(_Interacting, 0.1f);
    }

    public void OnAutoTurnsOff()
    {
        SetTargetNull();
        //DisableAIPath();
        stateMachine.SetToBaseState();
    }

    protected void Death(LevelSystem levelSystem)
    {
        if (isDead) return;
        isDead = true;
        //Debug.Log("Played Dead animation");
        PlayDeath();
        //anim.SetTrigger(this._Dead);
    }

    [Button("PlayDead")]
    public void PlayDeath()
    {
        anim.Play(this._Dead);
    }

    protected void HitAnim(LevelSystem lvl)
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
            //BarCreator.ReturnBar(currentBar);

            OnRevive?.Invoke();
        });
        // HP
        
        // 스킬 쿨다운 등등
    }

    public void Revive(bool hasReviveTime=true)
    {

        // 애니메이션
        //Invoke(nameof(), 2f);
        if (hasReviveTime) 
        {
            anim.Play(this._Revive);
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
                //BarCreator.ReturnBar(currentBar);

                OnRevive?.Invoke();
            });
        
        }
        else
        {
            isDead = false;
            //onAttackExit();

            MoveFromReviveToBattleMode();
            currentBarTween = null;
            //BarCreator.ReturnBar(currentBar);

            OnRevive?.Invoke();
        }
        // HP

        // 스킬 쿨다운 등등
    }

    private void SetTargetNullOnDead(LevelSystem levelSystem)
    {
        Debug.Log("Target set to null on death");
        SetTarget(null);
    }

    public void MoveFromReviveToBattleMode()
    {
        anim.SetTrigger(this._Revive);
    }

    public virtual void Turn(bool turnRight)
    {

        playerSprite.transform.localScale = turnRight ? one : minusOne;
        //playerSprite.flipX = !turnRight;
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    protected void Update()
    {
        //CheckGrounded(); // 땅에 닿아잇는지를 체크
        attackTimer.Tick(Time.deltaTime);

        stateMachine.Update();
        

    }

    public void UseSkill()
    {
        ActiveSkill usableSkill = _skillContainer.FindUsableSkill();
        if (usableSkill != null && TargetWithinAttackRange())
        {
            _skillContainer.UseActiveSkill(usableSkill, Target);
        }

    }

    public void Attack()
    {
        if (basicAttack == null)
        {
            basicAttack = Resources.Load<PlayerBasicAttack>("ScriptableObject/Skills/PlayerBasicAttack");
        }
        // 데미지 계산
        if (Target == null) return;
        // 타겟방향으로 조금 나아감
        // Calculate direction to the target
        Vector3 directionToTarget = (Target.transform.position - transform.position).normalized;
        
        StartCoroutine(AttackDelay(0.2f));
       

        // Turn to face the target
        Turn(directionToTarget.x > 0);
    }
    private IEnumerator AttackDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if (Target == null) yield break;
        basicAttack.ApplyEffect(_statContainer, Target.GetComponent<StatContainer>());
    }

    protected void SetTarget(Health enemy)
    {
        if (Target != null)
        {
            Target.OnDeath.RemoveListener(SetTargetNullOnDead);
        }
        Target = enemy;
        //Debug.Log("Target set: " + enemy.name);
        if (Target == null)
        {
            //_destinationSetter.target = null;
        }
        else
        {
            //_aiPath.destination = Target.transform.position;
            //_destinationSetter.target = Target.transform;
            enemy.OnDeath.AddListener(SetTargetNullOnDead);
        }
    }

    public void ChaseEnemy()
    {
        moveDir = (Target.transform.position - transform.position).normalized;
        transform.position += (Vector3)moveDir * runSpeed * Time.fixedDeltaTime;
        TurnToTarget();
    }

    public void TurnToTarget()
    {
        Turn(Target.transform.position.x - transform.position.x > 0);
    }
    private void TurnBasedOnAutoMovement(Vector3 dir)
    {
        if (dir.x > 0)
        {
            Turn(true);
        }
        else if (dir.x < 0)
        {
            Turn(false);
        }
    }
    public void SetTargetNull() => SetTarget(null);

    public Health GetTarget()
    {
        return Target;
    }

    public void EnableAIPath() => _aiPath.enabled = true;
    public void DisableAIPath() => _aiPath.enabled = false;
    public void MoveWithJoystick()
    {
        moveDir = _playerInput.JoystickDirection;
        //_aiPath.Move(moveDir * moveSpeed * Time.fixedDeltaTime);
        //moveDir = _playerInput.JoystickDirection;
        //moveSpeed = _statContainer.MoveSpeed.FinalValue;
        transform.position += (Vector3)moveDir * moveSpeed * Time.fixedDeltaTime;
        Turn(moveDir.x > 0);

    }


    public void DoIdle()
    {
        DisableMovement();
    }



    private RaycastHit2D[] raycastHits = new RaycastHit2D[20]; // Adjust size as needed
    public bool SearchForTarget()
    {
        // Create a circle around the transform's position with the specified scanDistance radius
        Vector2 circleCenter = transform.position;
        float circleRadius = scanDistance;

        // Perform a CircleCastAll to detect any colliders within the circle
        int hitCount = Physics2D.CircleCastNonAlloc(circleCenter, circleRadius, Vector2.zero, raycastHits, 0f, enemyLayer);

        if (hitCount > 0 && Target == null)
        {
            float closestDistance = float.MaxValue;
            int closestIndex = -1;

            // Find the closest valid target
            for (int i = 0; i < hitCount; i++)
            {
                float distance = Vector2.SqrMagnitude(raycastHits[i].transform.position - transform.position);
                if (distance < closestDistance)
                {
                    Health target = raycastHits[i].transform.GetComponent<Health>();
                    if (target != null && !target.IsDead)
                    {
                        closestDistance = distance;
                        closestIndex = i;
                    }
                }
            }

            // Set the target if a valid one was found
            if (closestIndex != -1)
            {
                SetTarget(raycastHits[closestIndex].transform.GetComponent<Health>());
                return true;
            }
        }

        return false;
    }

    public void EnableMovement()
    {
        //Debug.Log("Can move now");
        canMove = true;
        //_aiPath.canMove = canMove;
    }

    public void DisableMovement()
    {
        canMove = false;
        //_aiPath.canMove = canMove;
    }
    private void UpdateMoveSpeed(StatContainer statContainer)
    {
        this.runSpeed = statContainer.MoveSpeed.FinalValue;
        this.moveSpeed = this.runSpeed; 
        //_aiPath.maxSpeed = this.runSpeed;
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
            OnAutoTurnsOff();
        }
    }
}
