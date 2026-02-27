using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Attack : MonoBehaviour
{
    [Header("Melee Attack")]
    public int damage = 10;
    [Tooltip("Width of the melee box (X)")]
    public float boxWidth = 1f;
    [Tooltip("Height of the melee box (Y)")]
    public float boxHeight = 1f;
    [Tooltip("Depth/length of the melee box (Z, forward)")]
    public float boxDepth = 1.5f;
    [Tooltip("Time in seconds between allowed attacks")]
    public float cooldown = 0.5f;
    [Tooltip("Optional transform to originate the attack from (e.g., hand). If null, this object transform is used")]
    public Transform attackOrigin;
    [Tooltip("Layers that can be hit by the melee attack")]
    public LayerMask hitMask = ~0;
    [Tooltip("Log hit results for debugging")]
    public bool debug = false;
    [Header("Impact Feedback")]
    [Tooltip("Duration of camera shake in seconds")]
    public float shakeDuration = 0.12f;
    [Tooltip("Magnitude of camera shake (units)")]
    public float shakeMagnitude = 0.18f;
    [Tooltip("Short freeze (seconds, realtime) at hit start")]
    public float hitStopDuration = 0.04f;
    [Tooltip("Optional slow-motion after hit (timeScale) e.g. 0.2f")]
    [Range(0f, 1f)]
    public float slowMoScale = 0.25f;
    [Tooltip("Duration of slow-mo (seconds, realtime)")]
    public float slowMoDuration = 0.12f;
    [Header("Pushback")]
    [Tooltip("Impulse force applied to rigidbody targets")]
    public float pushForce = 5f;
    [Tooltip("Upward impulse applied together with push (for a bit of arc)")]
    public float pushUpForce = 0.5f;
    [Tooltip("Distance to push NavMeshAgent targets (world units)")]
    public float pushBackDistance = 1f;
    [Tooltip("Duration over which to move NavMeshAgent targets")]
    public float pushDuration = 0.18f;
    [Header("Hit VFX")]
    [Tooltip("Particle system prefab to spawn when a hit lands")]
    public ParticleSystem hitEffectPrefab;
    [Tooltip("Local offset applied to the hit effect spawn position")]
    public Vector3 hitEffectOffset = Vector3.zero;
    [Tooltip("How long to keep the spawned effect alive (seconds)")]
    public float hitEffectLifetime = 2f;

    float lastAttackTime = -999f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }
    }

    void TryAttack()
    {
        if (Time.time < lastAttackTime + cooldown) return;
        lastAttackTime = Time.time;

        Vector3 origin = attackOrigin != null ? attackOrigin.position : transform.position;
        Vector3 boxSize = new Vector3(boxWidth, boxHeight, boxDepth);
        Vector3 center = origin + transform.forward * (boxDepth * 0.5f);
        Quaternion orientation = transform.rotation;

        Collider[] hits = Physics.OverlapBox(center, boxSize * 0.5f, orientation, hitMask, QueryTriggerInteraction.Collide);

        if (debug) Debug.Log($"[Attack] OverlapBox center={center} size={boxSize} hits={hits.Length}");

        for (int i = 0; i < hits.Length; i++)
        {
            Collider col = hits[i];
            if (debug) Debug.Log($"[Attack] Hit collider: {col.name} on {col.gameObject.name}");

            // closest point on collider to the attack center
            Vector3 hitPoint = col.ClosestPoint(center) + hitEffectOffset;

            IDamageable d = col.GetComponentInParent<IDamageable>();
            if (d != null)
            {
                d.TakeDamage(damage);
                SpawnHitEffect(hitPoint, col.transform);
                TriggerImpactFeedback();
                ApplyPushback(col);
                continue;
            }

            // Fallback: try common component named Enemy on parent
            var enemy = col.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                SpawnHitEffect(hitPoint, col.transform);
                TriggerImpactFeedback();
                ApplyPushback(col);
            }
        }
    }

    void TriggerImpactFeedback()
    {
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(shakeDuration, shakeMagnitude);
        }

        if (hitStopDuration > 0f || slowMoDuration > 0f)
        {
            StartCoroutine(HitStopThenSlow());
        }
    }

    System.Collections.IEnumerator HitStopThenSlow()
    {
        float previous = Time.timeScale;
        float previousFixed = Time.fixedDeltaTime;

        if (hitStopDuration > 0f)
        {
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(hitStopDuration);
        }

        if (slowMoDuration > 0f)
        {
            Time.timeScale = Mathf.Clamp01(slowMoScale);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return new WaitForSecondsRealtime(slowMoDuration);
        }

        Time.timeScale = previous;
        Time.fixedDeltaTime = previousFixed;
    }

    void ApplyPushback(Collider col)
    {
        if (col == null) return;

        Vector3 origin = attackOrigin != null ? attackOrigin.position : transform.position;
        Vector3 center = origin + transform.forward * (boxDepth * 0.5f);
        Vector3 hitPoint = col.ClosestPoint(center);
        Vector3 dir = (col.transform.position - origin).normalized;
        if (dir.sqrMagnitude < 0.001f) dir = (col.transform.position - center).normalized;
        if (dir.sqrMagnitude < 0.001f) dir = transform.forward;

        // Try rigidbody first
        Rigidbody rb = col.GetComponentInParent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(dir * pushForce + Vector3.up * pushUpForce, ForceMode.Impulse);
            return;
        }

        // If this is an Enemy using NavMeshAgent, move it back smoothly
        Enemy enemy = col.GetComponentInParent<Enemy>();
        if (enemy != null && enemy.navMeshAgent != null)
        {
            StartCoroutine(ApplyAgentPush(enemy.navMeshAgent, dir, pushBackDistance, pushDuration));
            return;
        }

        // Fallback: try to nudge the transform
        Transform t = col.GetComponentInParent<Transform>();
        if (t != null)
        {
            t.position += dir * pushBackDistance;
        }
    }

    System.Collections.IEnumerator ApplyAgentPush(NavMeshAgent agent, Vector3 dir, float distance, float duration)
    {
        if (agent == null) yield break;
        bool prevStopped = agent.isStopped;
        agent.isStopped = true;

        float elapsed = 0f;
        float moved = 0f;
        while (elapsed < duration && moved < distance)
        {
            float step = Mathf.Min(distance - moved, (distance / duration) * Time.deltaTime);
            agent.Move(dir * step);
            moved += step;
            elapsed += Time.deltaTime;
            yield return null;
        }

        agent.isStopped = prevStopped;
    }

    void SpawnHitEffect(Vector3 position, Transform parent = null)
    {
        if (hitEffectPrefab == null) return;
        ParticleSystem ps = Instantiate(hitEffectPrefab, position, Quaternion.identity, parent);
        ps.Play();
        Destroy(ps.gameObject, hitEffectLifetime);
    }

    void OnDrawGizmosSelected()
    {
        Vector3 origin = attackOrigin != null ? attackOrigin.position : transform.position;
        Vector3 boxSize = new Vector3(boxWidth, boxHeight, boxDepth);
        Vector3 center = origin + transform.forward * (boxDepth * 0.5f);

        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
        Gizmos.matrix = old;
    }
}
