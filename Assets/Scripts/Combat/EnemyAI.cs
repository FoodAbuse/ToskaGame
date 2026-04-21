using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    public enum AIState { Idle, Chase, Attack }

    [Header("AI Settings")]
    public Transform player;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float roamRadius = 5f;
    public float roamInterval = 3f;

    [Header("Movement")]
    public float chaseSpeed = 5f;
    public float idleSpeed = 2f;

    [Header("Combat")]
    public int attackDamage = 10;
    public float attackCooldown = 1f;

    private float lastAttackTime;

    private NavMeshAgent agent;
    private AIState currentState;
    private float lastRoamTime;
    private Vector3 roamTarget;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = AIState.Idle;
        SetRoamTarget();
    }

    private void Update()
    {
        UpdateState();
        ExecuteState();
    }

    private void UpdateState()
    {
        if (BaseZone.IsPlayerSafe)
        {
            currentState = AIState.Idle;
            return;
        }

        if (player == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            currentState = AIState.Attack;
        }
        else if (distanceToPlayer <= detectionRange)
        {
            currentState = AIState.Chase;
        }
        else
        {
            currentState = AIState.Idle;
        }
    }

    private void ExecuteState()
    {
        switch (currentState)
        {
            case AIState.Idle:
                IdleBehavior();
                break;
            case AIState.Chase:
                ChaseBehavior();
                break;
            case AIState.Attack:
                AttackBehavior();
                break;
        }
    }

    private void IdleBehavior()
    {
        agent.speed = idleSpeed;

        if (Time.time - lastRoamTime > roamInterval)
        {
            SetRoamTarget();
            lastRoamTime = Time.time;
        }

        if (Vector3.Distance(transform.position, roamTarget) < 1f)
        {
            SetRoamTarget();
        }

        agent.SetDestination(roamTarget);
    }

    private void ChaseBehavior()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    private void AttackBehavior()
    {
        agent.isStopped = true;

        if (Time.time - lastAttackTime > attackCooldown)
        {
            HealthSystem playerHealth = player.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log($"{gameObject.name} attacked player for {attackDamage} damage!");
                lastAttackTime = Time.time;
            }
        }
    }

    private void SetRoamTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, NavMesh.AllAreas))
        {
            roamTarget = hit.position;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}