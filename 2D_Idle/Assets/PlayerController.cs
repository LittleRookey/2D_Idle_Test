using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // �÷��̾� AI
    // �޷����� ��Ÿ�� ����
    [SerializeField] private Animator anim;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float scanDistance = 2f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask groundLayer;
    // �ִϸ��̼�
    private int isJumping = Animator.StringToHash("isJumping");
    private int isRunning = Animator.StringToHash("isRunning");
    private int isGround = Animator.StringToHash("isGround");

    private eBehavior currentBehavior;
    private bool isGrounded;

    private Health Target;
    private Rigidbody2D rb2D;
    
    private enum eBehavior
    {
        idle,
        walk,
        run,
        jump,
        attack,
        ability
    }
    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        
    }

    private void Move()
    {
        rb2D.velocity += new Vector2(1 * moveSpeed, 0) * Time.deltaTime;

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
        var raycastHit = Physics2D.Raycast(transform.position, Vector2.right, scanDistance, enemyLayer);
        Debug.DrawRay(transform.position, Vector2.right * scanDistance, Color.red, 0.3f);
        if (raycastHit)
        {
            if (Target == null)
                SetTarget(raycastHit.transform.GetComponent<Health>());
        }
        Action(); // �⺻ AI
    }

    private void SetTarget(Health enemy)
    {
        Target = enemy;
        Debug.Log("Target set: " + enemy.name);
        // ���� ������ ���� 
        
    }

    private void Action()
    {
        switch(currentBehavior)
        {
            case eBehavior.idle:
                
                break;
            case eBehavior.walk:
                break;
            case eBehavior.run:
                anim.SetBool(isRunning, true);
                
                break;
            case eBehavior.jump:
                anim.SetBool(isJumping, true);
                break;
            case eBehavior.attack:
                //anim.SetBool(isRunning, true);
                break;
            case eBehavior.ability:
                break;
        }
    }

    private void SwitchState(eBehavior behavior)
    {
        currentBehavior = behavior;
        switch (currentBehavior)
        {
            case eBehavior.idle:
                
                break;
            case eBehavior.walk:

                break;
            case eBehavior.run:

                anim.SetBool(isRunning, true);

                break;
            case eBehavior.jump:
 
                anim.SetBool(isJumping, true);
                break;
            case eBehavior.attack:
  
                //anim.SetBool(isRunning, true);
                break;
            case eBehavior.ability:

                break;
        }
    }
}
