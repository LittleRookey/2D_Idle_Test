using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Litkey.AI;
using Sirenix.OdinInspector;

public class StateMachineDebugger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stateText;
    EnemyAI enemyAI;
    PlayerController player;
    private void Awake()
    {
        enemyAI = transform.parent.GetComponent<EnemyAI>();
        player = transform.parent.GetComponent<PlayerController>();
        
    }

    private void OnEnable()
    {
        if (enemyAI != null)
            enemyAI.stateMachine.OnStateChanged += SetState;
        if (player != null)
        {
            Debug.Log("Debugger state change added to event");
            player.stateMachine.OnStateChanged += SetState;
        }
    }
    private void OnDisable()
    {
        if (enemyAI != null)
            enemyAI.stateMachine.OnStateChanged -= SetState;
        if (player != null)
        {
            player.stateMachine.OnStateChanged -= SetState;
        }
    }
    public void SetState(string state)
    {
        stateText.SetText(state);
    }

    [Button("AddDebugger")]
    public void AddListienerToPlayer()
    {
        if (player != null)
        {
            player.stateMachine.OnStateChanged += SetState;
        }
    }
}
