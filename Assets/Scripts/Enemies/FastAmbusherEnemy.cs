using System.Collections.Generic;
using UnityEngine;

namespace EnemyAI
{
    /// <summary>
    /// Extremely fast enemy that lunges or leaps at the player.  Low health but
    /// high aggression.  Designed to punish slow reactions.
    /// </summary>
    public class FastAmbusherEnemy : EnemyController
    {
    protected override void Awake()
    {
        base.Awake();

        speed = 6f;
        detectionRadius = 12f;
        attackRange = 1.5f;
        staggerThreshold = 10;

        attacks = new List<AttackData>
        {
            new AttackData
            {
                attackName = "Leap",
                windUpTime = 0.2f,
                activeTime = 0.2f,
                recoveryTime = 0.6f,
                damage = 12,
                range = 3f,
                cooldown = 1.5f
            }
        };
    }
}
} // namespace EnemyAI
