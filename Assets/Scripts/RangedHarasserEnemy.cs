using System.Collections.Generic;
using UnityEngine;

namespace EnemyAI
{
    /// <summary>
    /// Keeps distance from the player, fires projectiles and retreats when too
    /// close.
    /// </summary>
    public class RangedHarasserEnemy : EnemyController
    {
        [Header("Ranged")] 
        [Tooltip("Projectile prefab that will be fired during the Shoot attack")]
        public GameObject projectilePrefab;

        public float retreatDistance = 5f;

    protected override void Awake()
    {
        base.Awake();

        speed = 3f;
        detectionRadius = 15f;
        attackRange = 10f;
        staggerThreshold = 10;

        attacks = new List<AttackData>
        {
            new AttackData
            {
                attackName = "Shoot",
                windUpTime = 0.5f,
                activeTime = 0.1f,
                recoveryTime = 1f,
                damage = 10,
                range = attackRange,
                cooldown = 2f,
                projectilePrefab = projectilePrefab, // use inspector-assigned prefab
                projectileSpeed = 20f
            }
        };
    }

    public override void MoveTowardsPlayer()
    {
        if (player == null || agent == null)
            return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist < retreatDistance)
        {
            Vector3 dir = (transform.position - player.position).normalized;
            agent.isStopped = false;
            agent.SetDestination(transform.position + dir * retreatDistance);
        }
        else
        {
            base.MoveTowardsPlayer();
        }
    }

    public override void OnAttackHit(AttackData attack)
    {
        if (attack.projectilePrefab != null && player != null)
        {
            Vector3 spawnPos = transform.position + transform.forward * 1f;
            GameObject proj = GameObject.Instantiate(attack.projectilePrefab, spawnPos, Quaternion.LookRotation((player.position - spawnPos).normalized));
            Projectile projectile = proj.GetComponent<Projectile>();
            if (projectile != null)
                projectile.Initialize(attack.damage, attack.projectileSpeed, playerLayer);
        }
        else
        {
            base.OnAttackHit(attack);
        }
    }
}
} // namespace EnemyAI
