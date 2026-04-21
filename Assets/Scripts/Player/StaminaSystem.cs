using System;
using UnityEngine;

public class StaminaSystem : MonoBehaviour
{
    [Header("Stamina")]
    public float maxStamina = 5f;
    public float drainRate = 1.5f;
    public float regenRate = 1f;
    public float minStaminaToUse = 0.01f;

    public float currentStamina { get; private set; }

    public event Action<float, float> OnStaminaChanged;
    public event Action OnStaminaDepleted;
    public event Action OnStaminaRecovered;

    private void Awake()
    {
        currentStamina = maxStamina;
    }

    public bool CanUseStamina()
    {
        return currentStamina > minStaminaToUse;
    }

    public bool UseStamina()
    {
        return UseStamina(drainRate * Time.deltaTime);
    }

    public bool UseStamina(float amount)
    {
        if (amount <= 0f)
            return true;

        if (!CanUseStamina())
            return false;

        float previousStamina = currentStamina;
        SetStamina(currentStamina - amount);

        return currentStamina > 0f || previousStamina > 0f;
    }

    public void Regenerate(float deltaTime)
    {
        if (deltaTime <= 0f)
            return;

        SetStamina(currentStamina + regenRate * deltaTime);
    }

    private void SetStamina(float value)
    {
        float clamped = Mathf.Clamp(value, 0f, maxStamina);

        if (Mathf.Approximately(clamped, currentStamina))
            return;

        float previousStamina = currentStamina;
        currentStamina = clamped;
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);

        if (currentStamina <= 0f && previousStamina > 0f)
            OnStaminaDepleted?.Invoke();

        if (currentStamina > 0f && previousStamina <= 0f)
            OnStaminaRecovered?.Invoke();
    }
}
