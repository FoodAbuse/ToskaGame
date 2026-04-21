using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    [Header("Speed")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    public float crouchSpeed = 2.5f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 1.7f;
    public float gravity = -24f;

    [Header("Crouch")]
    public float standingHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchTransitionSpeed = 10f;
    public float controllerCenterOffset = 0.08f;

    [Header("Stamina")]
    public StaminaSystem staminaSystem;

    [Header("Rotation")]
    public Transform cameraTransform;
    public float rotationSmoothTime = 0.1f;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector2 moveInput;
    private bool jumpInput;
    private bool sprintInput;
    private bool crouchInput;

    private bool isCrouching;
    private float smoothRotationVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        isCrouching = false;
        UpdateControllerCenter(standingHeight);
    }

    private void Update()
    {
        HandleCrouch();
        HandleStamina();
        HandleGravityAndJump();
        HandleMovement();
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public void SetSprintInput(bool input)
    {
        sprintInput = input;
    }

    public void SetJumpInput(bool input)
    {
        jumpInput = input;
    }

    public void SetCrouchInput(bool input)
    {
        crouchInput = input;
    }

    private void HandleMovement()
    {
        if (cameraTransform == null)
        {
            Debug.LogWarning("Camera Transform is not assigned on PlayerMovement.");
            return;
        }

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 direction = forward * moveInput.y + right * moveInput.x;
        if (direction.sqrMagnitude > 1f)
        {
            direction.Normalize();
        }

        bool hasMovementInput = direction.sqrMagnitude > 0.01f;
        float targetSpeed = walkSpeed;
        bool canSprint = sprintInput && hasMovementInput && !isCrouching && staminaSystem != null && staminaSystem.CanUseStamina();

        if (isCrouching)
        {
            targetSpeed = crouchSpeed;
        }
        else if (canSprint)
        {
            targetSpeed = sprintSpeed;
        }

        if (hasMovementInput)
        {
            float targetRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float smoothRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref smoothRotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothRotation, 0f);
        }

        Vector3 horizontalVelocity = direction * targetSpeed;
        controller.Move((horizontalVelocity + velocity) * Time.deltaTime);
    }

    private void HandleGravityAndJump()
    {
        if (controller.isGrounded)
        {
            if (velocity.y < 0f)
            {
                velocity.y = -2f;
            }

            if (jumpInput && !isCrouching)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpInput = false;
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }

    private void HandleCrouch()
    {
        if (crouchInput)
        {
            isCrouching = true;
        }
        else if (!crouchInput && controller.isGrounded)
        {
            isCrouching = false;
        }

        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
        UpdateControllerCenter(controller.height);
    }

    private void UpdateControllerCenter(float height)
    {
        controller.center = new Vector3(0f, height / 2f + controllerCenterOffset, 0f);
    }

    private void HandleStamina()
    {
        if (staminaSystem == null)
            return;

        if (sprintInput && moveInput.magnitude > 0.1f && !isCrouching)
        {
            staminaSystem.UseStamina();
        }
        else
        {
            staminaSystem.Regenerate(Time.deltaTime);
        }
    }

    public float GetCurrentStamina()
    {
        return staminaSystem != null ? staminaSystem.currentStamina : 0f;
    }

    public bool IsSprinting()
    {
        return sprintInput && moveInput.magnitude > 0.1f && staminaSystem != null && staminaSystem.CanUseStamina() && !isCrouching;
    }
}
