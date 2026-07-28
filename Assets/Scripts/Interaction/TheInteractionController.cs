using UnityEngine;

public class TheInteractionController : MonoBehaviour
{
    public SelectorType selectorType = SelectorType.RAYCASTFROMCAMERA;
    
    
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
    public bool drawDebugCapsule = true;
    public Color debugCapsuleRayColor = Color.green;
    

    private IInteractable currentInteractable;
    private Camera activeCamera;
    private Renderer currentRenderer;
    private Color[] originalColors;

    [Header("CapsuleCast")] public GameObject capsuleCastPoint;
    public float capsuleCastRadius = 0.2f;
    public float capsuleCastHeight = 0.2f;
    public enum SelectorType {RAYCASTFROMCAMERA, CAPSULEFROMCHARACTER}
    

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
        switch (selectorType)
        {
            case SelectorType.RAYCASTFROMCAMERA:
                RayCastLookAt();
                break;
            case SelectorType.CAPSULEFROMCHARACTER:
                CapsuleCastSelect();
                break;
        }
    }

    private void RayCastLookAt()
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

            IInteractable[] interactable = hit.collider.GetComponents<IInteractable>();
            
            if (interactable == null)
            {
                interactable = hit.collider.GetComponentsInParent<IInteractable>();
            }
            if (interactable.Length > 0 )
            {
                foreach (IInteractable i in interactable)
                {
                    if (i.IsInteractable())
                    {
                        currentInteractable = i;
                        TryHighlight(hit.collider.gameObject);
                        break;
                    }
                }
            }
        }
    }

    private void CapsuleCastSelect()
    {
        // draws a capsule cast infront of character for handling selection
        ResetHighlight();
        currentInteractable = null;
        
        Vector3 point1 = gameObject.transform.position;
        point1.y = point1.y - capsuleCastHeight / 2;
        Vector3 point2= gameObject.transform.position;
        point2.y = point2.y + capsuleCastHeight / 2;
        Vector3 direction = capsuleCastPoint.transform.position - gameObject.transform.position;
        float distance = Vector3.Distance(gameObject.transform.position, capsuleCastPoint.transform.position);
        
        RaycastHit[] hits = Physics.CapsuleCastAll(point1, point2, capsuleCastRadius, direction,distance,interactableMask);
            //Physics.RaycastAll(ray, interactionDistance, interactableMask);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (ignorePlayerColliders && IsSelfCollider(hit.collider))
                continue;

            IInteractable[] interactable = hit.collider.GetComponents<IInteractable>();
            
            if (interactable == null)
            {
                interactable = hit.collider.GetComponentsInParent<IInteractable>();
            }
            if (interactable.Length > 0 )
            {
                foreach (IInteractable i in interactable)
                {
                    if (i.IsInteractable())
                    {
                        currentInteractable = i;
                        TryHighlight(hit.collider.gameObject);
                        break;
                    }
                }
            }
        }
    }
    private void HandleInteractionInput()
    {
        if (currentInteractable == null)
            return;

        if (Input.GetKeyDown(interactKey))
        {
            // check if current interactable is of extended type
            if (currentInteractable is IInteractableExtended castInteractable)
            {
                castInteractable.Interact(gameObject);
            }
            else
                currentInteractable.Interact();
        }
    }

    private void OnDrawGizmos()
    {
        if (drawDebugRay)
        {
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

        if (drawDebugCapsule && capsuleCastPoint != null)
        {
            Camera debugCamera = playerCamera != null ? playerCamera : Camera.main;
            if (debugCamera == null)
                return;
            Gizmos.color = debugCapsuleRayColor;

            Vector3 point1 = capsuleCastPoint.transform.position;
            point1.y = point1.y - capsuleCastHeight / 2;
            Vector3 point2 = capsuleCastPoint.transform.position;
            point2.y = point2.y + capsuleCastHeight / 2;
            Gizmos.DrawSphere(point1, capsuleCastRadius);
            Gizmos.DrawSphere(point2, capsuleCastRadius);

            Vector3 drawLinePoint1 = point1;
            Vector3 drawLinePoint2 = point2;
            drawLinePoint1.x = drawLinePoint1.x - capsuleCastRadius;
            drawLinePoint2.x = drawLinePoint2.x - capsuleCastRadius;
            Gizmos.DrawLine(drawLinePoint1, drawLinePoint2);
            drawLinePoint1.x = point1.x + capsuleCastRadius;
            drawLinePoint2.x = point2.x + capsuleCastRadius;
            Gizmos.DrawLine(drawLinePoint1, drawLinePoint2);
            drawLinePoint1 = new Vector3(point1.x, point1.y, point1.z+capsuleCastRadius);
            drawLinePoint2 = new Vector3(point2.x, point2.y, point2.z+capsuleCastRadius);
            Gizmos.DrawLine(drawLinePoint1, drawLinePoint2);
            drawLinePoint1 = new Vector3(point1.x, point1.y, point1.z-capsuleCastRadius);
            drawLinePoint2 = new Vector3(point2.x, point2.y, point2.z-capsuleCastRadius);
            Gizmos.DrawLine(drawLinePoint1, drawLinePoint2);
            
        }

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

    private void UIInteraction()
    {
       
    }
}
