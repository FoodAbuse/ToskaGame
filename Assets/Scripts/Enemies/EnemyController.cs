using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace EnemyAI
{
    /// <summary>
    /// Core component placed on every enemy.  Owns the finite state machine and
    /// exposes configuration data used by the various states and archetypes.
    /// </summary>
    public class EnemyController : MonoBehaviour, IDamageable
    {
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Movement")]
    public float speed = 3f;
    public NavMeshAgent agent;

    [Header("Detection")]
    public float detectionRadius = 10f;
    public float attackRange = 2f;
    public LayerMask playerLayer;

    [Header("Stagger / Poise")]
    public int staggerThreshold = 20;
    public float staggerDuration = 1f;

    [Header("Attack definitions")]
    public List<AttackData> attacks;
    [HideInInspector] public int attackIndex;
    [HideInInspector] public float LastAttackRecovery;
    [HideInInspector] public float NextAttackTime;

    [HideInInspector] public Transform player;
    public bool PlayerInDetectionRange { get; private set; }
    public bool PlayerInAttackRange { get; private set; }

    // concrete state instances, created in Awake so transitions can refer directly to them
    [HideInInspector] public EnemyAI.IdleState IdleState;
    [HideInInspector] public EnemyAI.ChaseState ChaseState;
    [HideInInspector] public EnemyAI.AttackState AttackState;
    [HideInInspector] public EnemyAI.RecoverState RecoverState;
    [HideInInspector] public EnemyAI.StaggerState StaggerState;
    [HideInInspector] public EnemyAI.DeadState DeadState;

    public EnemyStateMachine StateMachine;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("EnemyTarget")?.transform;
        currentHealth = maxHealth;

        // build state machine
        StateMachine = new EnemyStateMachine();
        IdleState = new EnemyAI.IdleState(this, StateMachine);
        ChaseState = new EnemyAI.ChaseState(this, StateMachine);
        AttackState = new EnemyAI.AttackState(this, StateMachine);
        RecoverState = new EnemyAI.RecoverState(this, StateMachine);
        StaggerState = new EnemyAI.StaggerState(this, StateMachine);
        DeadState = new EnemyAI.DeadState(this, StateMachine);

        StateMachine.Initialize(IdleState); // already fully typed field
    }

    protected virtual void Update()
    {
        UpdateSenses();
        StateMachine.CurrentState.Tick();

        // keep navmesh velocity in sync with desired speed
        if (agent != null)
            agent.speed = speed;
    }

    public void UpdateSenses()
    {
        if (player == null) return;
        float dist = Vector3.Distance(transform.position, player.position);
        PlayerInDetectionRange = dist <= detectionRadius;
        PlayerInAttackRange = dist <= attackRange;
    }

    public virtual void MoveTowardsPlayer()
    {
        if (player != null && agent != null)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
    }

    public virtual void StopMovement()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            agent.SetDestination(transform.position);
        }
    }

    public AttackData GetNextAttack()
    {
        if (attacks == null || attacks.Count == 0)
            return null;

        var a = attacks[attackIndex];
        attackIndex = (attackIndex + 1) % attacks.Count;
        return a;
    }

    /// <summary>
    /// Default "hit" behaviour for an attack: a short overlap sphere that
    /// damages whatever on the player layer.
    /// Archetypes can override to fire projectiles, apply status effects, etc.
    /// </summary>
    public virtual void OnAttackHit(AttackData attack)
    {
        Vector3 origin = transform.position + transform.forward * (attack.range * .5f);
        Collider[] hits = Physics.OverlapSphere(origin, attack.range, playerLayer);
        foreach (var c in hits)
        {
            IDamageable d = c.GetComponent<IDamageable>();
            if (d != null)
                d.TakeDamage(attack.damage);
        }
    }

    public virtual void TakeDamage(int damageAmount)
    {
        if (StateMachine.CurrentState == DeadState) return;

        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            StateMachine.ChangeState(DeadState);
            return;
        }

        if (damageAmount >= staggerThreshold && StateMachine.CurrentState != StaggerState)
        {
            StaggerState.SetDuration(staggerDuration);
            StateMachine.ChangeState(StaggerState);
        }
    }

    public virtual void OnDeath()
    {
        // any death logic (animation, loot drop, etc.)
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
} // namespace EnemyAI
