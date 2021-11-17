using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Vector2 movementInput;
    public Vector2 cameraInput;

    public float cameraInputHorizontal;
    public float cameraInputVertical;

    public float moveAmount;
    public float verticalInput;
    public float horizontalInput;

    public bool sprintInput;
    public bool jumpInput;

    PlayerControls playerControls;
    PlayerLocomotion playerLocomotion;
    AnimatorManager animatorManager;

    void Awake() {
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    private void OnEnable() {
        if (playerControls == null) {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

            playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
            playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;

            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
            playerControls.PlayerActions.Jump.canceled += i => jumpInput = false;
        }

        playerControls.Enable();
    }

    private void OnDisable() {
        playerControls.Disable();
    }

    public void HandleAllInputs() {
        HandleMovementInput();
        HandleSprintingInput();
        HandeJumpingInput();
        //handle action input
    }

    private void HandleMovementInput() {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraInputVertical = cameraInput.y;
        cameraInputHorizontal = cameraInput.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput)); //abs cos animator can't work with negative value
        animatorManager.UpdateAnimatorValues(0, moveAmount, sprintInput);
    }

    private void HandleSprintingInput() {
        if (sprintInput && moveAmount > 0.5f) {
            playerLocomotion.isSprinting = true;
        } else {
            playerLocomotion.isSprinting = false;
        }
    }

    private void HandeJumpingInput() {
        if (jumpInput) {
            jumpInput = false;
            playerLocomotion.DoJump();
        }
    }
}
