using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimationHook : MonoBehaviour
{
    [SerializeField] private Animator weaponAnim;
    private EnemyAI enemyAI;

    private void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();    
    }

    private void OnEnable()
    {
        enemyAI.OnAttack.AddListener(Attack);    
    }

    private void OnDisable()
    {
        enemyAI.OnAttack.RemoveListener(Attack);
    }

    public void Idle()
    {

    }

    public void Attack(Health targ)
    {
        weaponAnim.SetTrigger("Attack");
    }

}
