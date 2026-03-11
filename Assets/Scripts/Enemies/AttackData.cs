using UnityEngine;

namespace EnemyAI
{
    /// <summary>
    /// Data container for a single attack; can be used by the inspector to create
    /// combos or ranged behaviours.
    /// </summary>
    [System.Serializable]
    public class AttackData
    {
    public string attackName;
    public float windUpTime = 0.5f;
    public float activeTime = 0.3f;
    public float recoveryTime = 0.7f;
    public int damage = 10;
    public float range = 1.5f;
    public float cooldown = 1f;

    // optional fields for projectiles
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
}
} // namespace EnemyAI