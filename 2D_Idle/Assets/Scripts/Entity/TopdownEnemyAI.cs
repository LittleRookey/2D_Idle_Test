using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TopdownEnemyAI : EnemyAI
{
    [SerializeField] protected float moveSpeed;



    private void UpdateMoveSpeed(StatContainer statContainer)
    {
        this.moveSpeed = statContainer.MoveSpeed.FinalValue;
    }

    private void UpdateSpeed(float speed)
    {
        this.moveSpeed = speed;
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

    protected void Move()
    {
        moveDir = (Target.transform.position - transform.position).normalized;

        // 방향 전환
        if (moveDir.x > 0) this.Turn(true);
        else if (moveDir.x < 0) this.Turn(false);
        Debug.Log("Player Running to Target");
        //rb2D.velocity = moveDir * runSpeed;
        transform.position += (Vector3)moveDir * moveSpeed * Time.deltaTime;

    }
    protected override void Action()
    {
        switch (currentBehavior)
        {
            case eEnemyBehavior.idle:
                // 적을 찾기
                if (!HasNoTarget())
                {
                    if (TargetWithinAttackRange())
                    {
                        SwitchState(eEnemyBehavior.attack);
                    }
                    else
                    {
                        SwitchState(eEnemyBehavior.chase);
                    }
                }
                else
                {
                    SearchForTarget();
                }

                
                // 적 찾으면 
                break;
            case eEnemyBehavior.chase:
                Move();

                if (TargetWithinAttackRange())
                {
                    SwitchState(eEnemyBehavior.attack);
                }
                break;
            case eEnemyBehavior.attack:
                //attack 1 0.8 / 2 0.6 / 3 0.4 / 4 0.2 / 5  
                //if (!stopAttackTimer)
                attackTimer += Time.deltaTime;
                TurnToTarget();
                if (attackTimer >= final_attackInterval)
                {
                    DamageAction();
                    attackTimer = 0f;
                }

                //SwitchState(eEnemyBehavior.idle);

                break;
            case eEnemyBehavior.dead:
                break;
            case eEnemyBehavior.hit:
                break;
            case eEnemyBehavior.stun:
                break;

        }

    }

    private void TurnToTarget()
    {
        if (Target == null) return;
        var dir = Target.transform.position - transform.position;
        if (dir.x > 0) Turn(true);
        else Turn(false);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
