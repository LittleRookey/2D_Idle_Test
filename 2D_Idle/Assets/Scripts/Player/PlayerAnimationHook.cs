using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHook : MonoBehaviour
{
    private int isJumping = Animator.StringToHash("isJumping");
    private int isRunning = Animator.StringToHash("isRunning");
    private int isWalking = Animator.StringToHash("isWalking");
    private int isGround = Animator.StringToHash("isGround");
    private int AttackState = Animator.StringToHash("AttackState");
    private int Attack = Animator.StringToHash("Attack");
    PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
