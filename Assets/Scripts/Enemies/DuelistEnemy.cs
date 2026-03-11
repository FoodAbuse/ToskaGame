using System.Collections.Generic;
using UnityEngine;

namespace EnemyAI
{
    /// <summary>
    /// Medium‑speed melee fighter.  Short combos, built for 1v1 encounters.
    /// </summary>
    public class DuelistEnemy : EnemyController
    {
    protected override void Awake()
    {
        base.Awake();

        speed = 4f;
        detectionRadius = 12f;
        attackRange = 2f;
        staggerThreshold = 15;

        attacks = new List<AttackData>
        {
            new AttackData
            {
                attackName = "Slash1",
                windUpTime = 0.3f,
                activeTime = 0.2f,
                recoveryTime = 0.5f,
                damage = 15,
                range = 1.5f,
                cooldown = 1f
            },
            new AttackData
            {
                attackName = "Slash2",
                windUpTime = 0.25f,
                activeTime = 0.15f,
                recoveryTime = 0.4f,
                damage = 18,
                range = 1.5f,
                cooldown = 1f
            }
        };
    }
}
} // namespace EnemyAI
