using UnityEngine;

public class TheInteractionController : MonoBehaviour
{
    [Header("Raycast")]
    public Camera playerCamera;
    public float interactionDistance = 3f;
    public KeyCode interactKey = KeyCode.E;
    public LayerMask interactableMask = Physics.DefaultRaycastLayers;
    public bool ignorePlayerColliders = true;

    [Header("Highlight")]
    public Color highlightColor = Color.yellow;

    [Header("Debug")]
    public bool drawDebugRay = true;
    public Color debugRayColor = Color.green;

    private IInteractable currentInteractable;
    private Camera activeCamera;
    private Renderer currentRenderer;
    private Color[] originalColors;

    private void Awake()
    {
        activeCamera = playerCamera != null ? playerCamera : Camera.main;
    }

    private void Update()
    {
        UpdateLookingAtObject();
        HandleInteractionInput();
    }

    private void UpdateLookingAtObject()
    {
        ResetHighlight();
        currentInteractable = null;

        activeCamera = playerCamera != null ? playerCamera : Camera.main;
        if (activeCamera == null)
        {
            Debug.LogWarning("InteractionController needs a camera reference or Camera.main.");
            return;
        }

        Ray ray = new Ray(activeCamera.transform.position, activeCamera.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, interactionDistance, interactableMask);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (ignorePlayerColliders && IsSelfCollider(hit.collider))
                continue;

            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable == null)
            {
                interactable = hit.collider.GetComponentInParent<IInteractable>();
            }
            if (interactable != null)
            {
                currentInteractable = interactable;
                TryHighlight(hit.collider.gameObject);
                break;
            }
        }
    }

    private void HandleInteractionInput()
    {
        if (currentInteractable == null)
            return;

        if (Input.GetKeyDown(interactKey))
        {
            currentInteractable.Interact();
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawDebugRay)
            return;

        Camera debugCamera = playerCamera != null ? playerCamera : Camera.main;
        if (debugCamera == null)
            return;

        Gizmos.color = debugRayColor;
        Vector3 origin = debugCamera.transform.position;
        Vector3 direction = debugCamera.transform.forward;
        Vector3 end = origin + direction * interactionDistance;
        Gizmos.DrawLine(origin, end);
        Gizmos.DrawSphere(end, 0.05f);
    }

    private void TryHighlight(GameObject hitObject)
    {
        currentRenderer = hitObject.GetComponent<Renderer>();
        if (currentRenderer == null)
            return;

        originalColors = new Color[currentRenderer.materials.Length];
        for (int i = 0; i < currentRenderer.materials.Length; i++)
        {
            originalColors[i] = currentRenderer.materials[i].color;
            currentRenderer.materials[i].color = highlightColor;
        }
    }

    private bool IsSelfCollider(Collider collider)
    {
        if (collider == null)
            return false;

        return collider.transform.IsChildOf(transform);
    }

    private void ResetHighlight()
    {
        if (currentRenderer == null || originalColors == null || originalColors.Length == 0)
            return;

        for (int i = 0; i < currentRenderer.materials.Length && i < originalColors.Length; i++)
        {
            currentRenderer.materials[i].color = originalColors[i];
        }

        currentRenderer = null;
        originalColors = null;
    }
}
