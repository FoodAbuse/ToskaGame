using System.Collections.Generic;
using UnityEngine;

namespace EnemyAI
{
    /// <summary>
    /// Slow, tanky enemy with big wind‑ups and huge recovery windows.  Hard to
    /// stagger.
    /// </summary>
    public class HeavyBruteEnemy : EnemyController
    {
    protected override void Awake()
    {
        base.Awake();

        speed = 2f;
        detectionRadius = 10f;
        attackRange = 2.5f;
        staggerThreshold = 50;

        attacks = new List<AttackData>
        {
            new AttackData
            {
                attackName = "HeavySmash",
                windUpTime = 1.2f,
                activeTime = 0.5f,
                recoveryTime = 1.5f,
                damage = 30,
                range = 2f,
                cooldown = 3f
            }
        };

        maxHealth = 300;
        currentHealth = maxHealth;
    }
}
} // namespace EnemyAI
