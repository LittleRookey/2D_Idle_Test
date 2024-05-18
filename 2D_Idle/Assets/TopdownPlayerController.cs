using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    //protected override void SwitchState(eBehavior behavior)
    //{
    //    switch (currentBehavior)
    //    {
    //        case eBehavior.idle:

    //            break;
    //        case eBehavior.walk:
    //            anim.SetBool(_isWalking, false);
    //            break;
    //        case eBehavior.run:
    //            anim.SetBool(_isRunning, false);

    //            break;
    //        case eBehavior.chase:
    //            anim.SetBool(_isRunning, false);
    //            break;
    //        case eBehavior.jump:

    //            anim.SetBool(_isJumping, false);
    //            break;
    //        case eBehavior.attack:
    //            //DisableMovement();
    //            //anim.SetBool(isRunning, true);
    //            break;
    //        case eBehavior.ability:

    //            break;
    //    }
    //}

    public void RunWithNoTarget()
    {
        //Vector2 dir = (Target.transform.position - transform.position).normalized;

        // 방향 전환
        if (moveDir.x > 0) this.Turn(true);
        else if (moveDir.x < 0) this.Turn(false);

        //rb2D.velocity = moveDir * runSpeed;
        transform.position += (Vector3)moveDir * runSpeed * Time.deltaTime;
    }

    protected override bool SearchForTarget()
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, scanDistance);
    }
    protected override void Move()
    {
        moveDir = (Target.transform.position - transform.position).normalized;

        // 방향 전환
        if (moveDir.x > 0) this.Turn(true);
        else if (moveDir.x < 0) this.Turn(false);
        Debug.Log("Player Running to Target");
        //rb2D.velocity = moveDir * runSpeed;
        transform.position += (Vector3)moveDir * runSpeed * Time.deltaTime;
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
        Debug.Log("Player in action");
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
                    } else
                    {
                        SearchForTarget();
                    }
                    
                }
                break;
            case eBehavior.chase: // 타겟이 있을떄만 들어온다
                // 적을 찾으면 적을향해 달려간다, 공격범위까지
                if (canMove)
                    Move();

                Debug.Log(TargetWithinAttackRange());
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
