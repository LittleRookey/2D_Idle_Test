using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Litkey.AI;

public class StateMachineDebugger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stateText;
    EnemyAI enemyAI;
    PlayerController player;
    private void Awake()
    {
        enemyAI = transform.parent.GetComponent<EnemyAI>();
        player = transform.parent.GetComponent<PlayerController>();
        if (enemyAI != null)
            enemyAI.stateMachine.OnStateChanged += SetState;
        if (player != null)
        {
            Debug.Log("State Changed");
            Debug.Log("State Changed " + player.stateMachine == null);
            Debug.Log("State Changed " + player.stateMachine.OnStateChanged == null);
            player.stateMachine.OnStateChanged += SetState;
        }
    }

    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        if (enemyAI != null)
            enemyAI.stateMachine.OnStateChanged -= SetState;
        if (player != null)
        {
            Debug.Log("State Changed");
            player.stateMachine.OnStateChanged -= SetState;
        }
    }
    public void SetState(IState state)
    {
        if (player != null) Debug.Log("State Changed111");
        stateText.SetText(state.StateName);
    }
}
