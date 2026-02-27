using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    Vector3 originalLocalPos;
    Coroutine running;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);
        originalLocalPos = transform.localPosition;
    }

    void OnDisable()
    {
        transform.localPosition = originalLocalPos;
    }

    public void Shake(float duration, float magnitude)
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(DoShake(duration, magnitude));
    }

    IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = (Random.value * 2f - 1f) * magnitude;
            float y = (Random.value * 2f - 1f) * magnitude;
            transform.localPosition = originalLocalPos + new Vector3(x, y, 0f);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        transform.localPosition = originalLocalPos;
        running = null;
    }
}
