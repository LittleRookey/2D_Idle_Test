using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TopdownPlayerController : PlayerController
{
    public Vector2 moveDir;

    public UnityEvent OnAutoOn;
    public UnityEvent OnAutoOff;


    protected override void Start()
    {

        EnableMovement();

    }

    public override void DisableMovement()
    {
        base.DisableMovement();
        rb2D.velocity = Vector2.zero;
        moveDir = Vector2.zero;
    }
    public void RunWithNoTarget()
    {
        //Vector2 dir = (Target.transform.position - transform.position).normalized;

        // 방향 전환
        if (moveDir.x > 0) this.Turn(true);
        else if (moveDir.x < 0) this.Turn(false);

        rb2D.velocity = moveDir * runSpeed;
        //transform.position += dir * runSpeed * Time.deltaTime;
    }

    protected override void Move()
    {
        moveDir = (Target.transform.position - transform.position).normalized;

        // 방향 전환
        if (moveDir.x > 0) this.Turn(true);
        else if (moveDir.x < 0) this.Turn(false);

        rb2D.velocity = moveDir * runSpeed;
    }

    public void Auto()
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

    protected override void Action()
    {
        if (isDead) return;

        //if (HasNoTarget())
        //{
        //    SwitchState(eBehavior.walk);
        //    return;
        //}
        switch (currentBehavior)
        {
            case eBehavior.idle:
                if (isAuto)
                {
                    if (!HasNoTarget() )
                    {
                        if (TargetWithinAttackRange())
                        {
                            SwitchState(eBehavior.attack);
                        } else
                        {
                            SwitchState(eBehavior.chase);
                        }
                    }
                }
                break;
            case eBehavior.chase: // 타겟이 있을떄만 들어온다
                // 적을 찾으면 적을향해 달려간다, 공격범위까지
                if (canMove)
                    Move();

                // 적이 공격범위 안까지 오면 공격
                if (TargetWithinAttackRange())
                {
                    SwitchState(eBehavior.attack);
                }

                break;
            case eBehavior.run: //  주로 움직일떄 사용
                if (canMove)
                    RunWithNoTarget();

                // 플레이어가 자동을 눌렀을떄 Idle이 돼서 적을 찾는다
                if (isAuto)
                {
                    SwitchState(eBehavior.idle);
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
}
