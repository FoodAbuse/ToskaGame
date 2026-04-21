using System.Collections;
using UnityEngine;

public class BaseZone : MonoBehaviour
{
    public static bool IsPlayerSafe { get; private set; } = false;

    [SerializeField] private float healingAmount = 10f;
    [SerializeField] private float healingInterval = 1f;

    private HealthSystem playerHealth;
    private Coroutine healingCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                IsPlayerSafe = true;
                playerHealth.IsInvulnerable = true;
                healingCoroutine = StartCoroutine(HealOverTime());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && playerHealth != null)
        {
            IsPlayerSafe = false;
            playerHealth.IsInvulnerable = false;
            if (healingCoroutine != null)
            {
                StopCoroutine(healingCoroutine);
                healingCoroutine = null;
            }
            playerHealth = null;
        }
    }

    private IEnumerator HealOverTime()
    {
        while (true)
        {
            playerHealth.Heal(healingAmount);
            yield return new WaitForSeconds(healingInterval);
        }
    }
}