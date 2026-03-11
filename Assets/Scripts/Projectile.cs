using UnityEngine;

namespace EnemyAI
{
    /// <summary>
    /// Simple projectile used by ranged enemies.  Moves forward each frame and
    /// damages whatever it hits on the specified layer.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Projectile : MonoBehaviour
    {
    private int damage;
    private float speed;
    private LayerMask targetLayer;

    public void Initialize(int dmg, float spd, LayerMask layer)
    {
        damage = dmg;
        speed = spd;
        targetLayer = layer;
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((targetLayer.value & (1 << other.gameObject.layer)) == 0)
            return;

        IDamageable d = other.GetComponent<IDamageable>();
        if (d != null)
            d.TakeDamage(damage);

        Destroy(gameObject);
    }
}
} // namespace EnemyAI