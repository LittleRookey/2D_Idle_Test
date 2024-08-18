using UnityEngine;

public class FlyingSwordAI : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackSpeed = 5f;
    public float returnSpeed = 3f;
    public float idleDistance = 2f;
    public float idleFloatAmplitude = 0.5f;
    public float idleFloatFrequency = 1f;

    private Vector3 idlePosition;
    private Quaternion idleRotation;
    private Transform target;
    private bool isAttacking = false;
    private Vector3 attackStartPosition;
    private float attackStartTime;
    public LayerMask enemyLayer;

    private enum SwordState { Idle, Attacking, Returning }
    private SwordState currentState = SwordState.Idle;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        CalculateIdlePosition();
    }

    void Update()
    {
        CalculateIdlePosition();

        switch (currentState)
        {
            case SwordState.Idle:
                UpdateIdleState();
                break;
            case SwordState.Attacking:
                UpdateAttackingState();
                break;
            case SwordState.Returning:
                UpdateReturningState();
                break;
        }
    }

    void CalculateIdlePosition()
    {
        idlePosition = player.position + player.right * idleDistance;
        idleRotation = Quaternion.LookRotation(player.forward);
    }

    void UpdateIdleState()
    {
        float verticalOffset = Mathf.Sin(Time.time * idleFloatFrequency) * idleFloatAmplitude;
        Vector3 floatingPosition = idlePosition + Vector3.up * verticalOffset;
        transform.position = floatingPosition;
        transform.rotation = idleRotation;

        // Detect enemies
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange, enemyLayer);
        
        if (colliders.Length > 0)
        {
            target = colliders[0].transform;
            StartAttack();

        }
        
            
        
    }

    void StartAttack()
    {
        currentState = SwordState.Attacking;
        attackStartPosition = transform.position;
        attackStartTime = Time.time;
    }

    void UpdateAttackingState()
    {
        float journeyLength = Vector3.Distance(attackStartPosition, target.position);
        float distanceCovered = (Time.time - attackStartTime) * attackSpeed;
        float fractionOfJourney = distanceCovered / journeyLength;

        transform.position = Vector3.Slerp(attackStartPosition, target.position, fractionOfJourney);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position), fractionOfJourney);

        if (fractionOfJourney >= 1.0f)
        {
            // Attack logic here
            Debug.Log("Enemy attacked!");
            currentState = SwordState.Returning;
            attackStartPosition = transform.position;
            attackStartTime = Time.time;
        }
    }

    void UpdateReturningState()
    {
        float journeyLength = Vector3.Distance(attackStartPosition, idlePosition);
        float distanceCovered = (Time.time - attackStartTime) * returnSpeed;
        float fractionOfJourney = distanceCovered / journeyLength;

        transform.position = Vector3.Slerp(attackStartPosition, idlePosition, fractionOfJourney);
        transform.rotation = Quaternion.Slerp(transform.rotation, idleRotation, fractionOfJourney);

        if (fractionOfJourney >= 1.0f)
        {
            currentState = SwordState.Idle;
        }
    }
}