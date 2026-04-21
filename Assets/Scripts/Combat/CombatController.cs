using UnityEngine;

public class CombatController : MonoBehaviour
{
    [Header("Attack")]
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    public int attackDamage = 10;
    public KeyCode attackKey = KeyCode.Mouse0;

    [Header("Detection")]
    public LayerMask enemyLayer;
    public float detectionRadius = 2.5f;

    private float lastAttackTime;

    private void Update()
    {
        if (Input.GetKeyDown(attackKey) && CanAttack())
        {
            PerformAttack();
        }
    }

    private bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }

    private void PerformAttack()
    {
        lastAttackTime = Time.time;

        // Detect enemies in range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        foreach (Collider collider in hitColliders)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null && Vector3.Distance(transform.position, enemy.transform.position) <= attackRange)
            {
                enemy.TakeDamage(attackDamage);
                Debug.Log("Attacked enemy for " + attackDamage + " damage.");
                break; // Attack only one enemy per swing
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}