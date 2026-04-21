using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public PlayerMovement playerMovement;

    private void Update()
    {
        if (playerMovement == null)
            return;

        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool sprintInput = Input.GetKey(KeyCode.LeftShift);
        bool jumpInput = Input.GetButtonDown("Jump");
        bool crouchInput = Input.GetKey(KeyCode.LeftControl);

        playerMovement.SetMoveInput(moveInput);
        playerMovement.SetSprintInput(sprintInput);
        playerMovement.SetJumpInput(jumpInput);
        playerMovement.SetCrouchInput(crouchInput);
    }
}
