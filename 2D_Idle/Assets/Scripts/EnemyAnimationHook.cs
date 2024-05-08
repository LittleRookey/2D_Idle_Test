using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class EnemyAnimationHook : MonoBehaviour
{

    [SerializeField] private DOTweenAnimation dotweenAnimation;

    private EnemyAI enemyAI;

    private void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();
    }

    private void OnEnable()
    {
        enemyAI.OnIdle.AddListener(Idle);
        enemyAI.OnHit.AddListener(Hit);
        enemyAI.OnAttack.AddListener(RandAttack);
        enemyAI.OnDead.AddListener(Dead);

    }


    private void OnDisable()
    {
        enemyAI.OnIdle.RemoveListener(Idle);
        enemyAI.OnHit.RemoveListener(Hit);
        enemyAI.OnAttack.RemoveListener(RandAttack);
        enemyAI.OnDead.RemoveListener(Dead);
    }

    public void Idle(Health targ)
    {
        dotweenAnimation.DORestartAllById("Idle");
    }

    public void Hit(Health targ)
    {
        dotweenAnimation.DORestartAllById("Hit");   
    }
    
    private void RandAttack(Health targ)
    {
        var randNum = Random.Range(0f, 2f);

        if (randNum >= 1f) JumpAttack(targ);
        else FrontAttack(targ);
    }

    private void JumpAttack(Health target) => dotweenAnimation.DORestartAllById("JumpAttack");

    private void FrontAttack(Health target) => dotweenAnimation.DORestartAllById("FrontAttack");


    private void Dead(Health targ)
    {
        dotweenAnimation.DORestartAllById("Dead");
    }
}
