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
        enemyAI.OnAttack.AddListener(Attack);
        enemyAI.OnDead.AddListener(Dead);

    }


    private void OnDisable()
    {
        enemyAI.OnIdle.RemoveListener(Idle);
        enemyAI.OnHit.RemoveListener(Hit);
        enemyAI.OnAttack.RemoveListener(Attack);
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
    
    public void Attack(Health targ)
    {
        dotweenAnimation.DORestartAllById("Attack");
    }

    public void Dead(Health targ)
    {
        dotweenAnimation.DORestartAllById("Dead");
    }
}
