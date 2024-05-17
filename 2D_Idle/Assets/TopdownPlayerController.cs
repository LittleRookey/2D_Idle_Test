using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopdownPlayerController : PlayerController
{
    public Vector2 moveDir;

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

        // ���� ��ȯ
        if (moveDir.x > 0) this.Turn(true);
        else if (moveDir.x < 0) this.Turn(false);

        rb2D.velocity = moveDir * runSpeed;
        //transform.position += dir * runSpeed * Time.deltaTime;
    }

    protected override void Move()
    {
        moveDir = (Target.transform.position - transform.position).normalized;

        // ���� ��ȯ
        if (moveDir.x > 0) this.Turn(true);
        else if (moveDir.x < 0) this.Turn(false);

        rb2D.velocity = moveDir * runSpeed;
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
            case eBehavior.chase: // Ÿ���� �������� ���´�
                // ���� ã���� �������� �޷�����, ���ݹ�������
                if (canMove)
                    Move();

                // ���� ���ݹ��� �ȱ��� ���� ����
                if (TargetWithinAttackRange())
                {
                    SwitchState(eBehavior.attack);
                }

                break;
            case eBehavior.run: //  �ַ� �����ϋ� ���
                if (canMove)
                    RunWithNoTarget();

                // �÷��̾ �ڵ��� �������� Idle�� �ż� ���� ã�´�
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
