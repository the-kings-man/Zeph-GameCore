using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{

    AnimatorManager animatorManager;
    InputManager inputManager;
    PlayerManager playerManager;
    Rigidbody playerRigidbody;
    Vector3 moveDirection;
    
    public Transform cameraObject;

    [Header("Vertical Movements")]
    public float inAirTimer;
    public float leapingSpeed;
    public float fallingSpeed;
    public LayerMask groundLayer;
    public float rayCastHeightOffset = 0.5f;
    public float jumpHeight = 3;
    public float gravityIntensity = -15;
    public float maxClimbingHeight = 0.1f;

    [Header("Horizontal Movements")]
    public float walkingSpeed = 1.5f;
    public float runningSpeed = 5f;
    public float sprintingSpeed = 7f;
    public float rotationSpeed = 15f;

    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isGrounded;
    public bool isJumping;

    void Awake() {
        animatorManager = GetComponent<AnimatorManager>();
        inputManager = GetComponent<InputManager>();
        playerManager = GetComponent<PlayerManager>();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    public void HandleAllMovement() {
        HandleFallingAndLanding();

        if (!playerManager.isInteracting) {
            HandleMovement();
            HandleRotation();
        }
    }

    void HandleMovement() {
        if (!isJumping) {
            moveDirection = cameraObject.forward * inputManager.verticalInput;
            moveDirection = moveDirection + cameraObject.right * inputManager.horizontalInput;
            moveDirection.Normalize();
            moveDirection.y = 0;

            var moveSpeed = 0f;
            if (isSprinting) {
                moveSpeed = sprintingSpeed;
            } else {
                if (inputManager.moveAmount >= 0.5f) {
                    moveSpeed = runningSpeed;
                } else {
                    moveSpeed = walkingSpeed;
                }
            }

            Vector3 movementVelocity = moveDirection * moveSpeed;
            playerRigidbody.velocity = movementVelocity;
        }
    }

    void HandleRotation() {
        if (!isJumping) {
            Vector3 targetDirection = Vector3.zero;

            targetDirection = cameraObject.forward * inputManager.verticalInput;
            targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;
            targetDirection.Normalize();
            targetDirection.y = 0;

            if (targetDirection == Vector3.zero)
                targetDirection = transform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); //rotation between point a and point b

            transform.rotation = playerRotation;
        }
    }

    void HandleFallingAndLanding() {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        Vector3 groundedPosition = transform.position;
        rayCastOrigin.y = rayCastOrigin.y + rayCastHeightOffset;

        if (!isGrounded && !isJumping) {
            if (!playerManager.isInteracting) {
                animatorManager.PlayTargetAnimation("Falling", true);
            }

            inAirTimer = inAirTimer + Time.deltaTime;
            //playerRigidbody.AddForce(transform.forward * leapingSpeed);
            playerRigidbody.AddForce(-Vector3.up * fallingSpeed * inAirTimer);
        }

        if (Physics.SphereCast(rayCastOrigin, 0.2f, -Vector3.up, out hit, groundLayer)) {
            if (!isGrounded && !playerManager.isInteracting) {
                animatorManager.PlayTargetAnimation("Land", true);
            }

            Vector3 rayCastHitPoint = hit.point; //save point that the raycast hit
            groundedPosition.y = rayCastHitPoint.y;
            inAirTimer = 0;
            isGrounded = true;
        } else {
            isGrounded = false;
        }

        //adjust characters model based upon if we are grounded or not -> bring in contact with the ground. Had to uncheck "use gravity" in rigidbody
        if (isGrounded && !isJumping) {
            if ((groundedPosition.y - transform.position.y) < maxClimbingHeight) {
                if (playerManager.isInteracting || inputManager.moveAmount > 0) {
                    transform.position = Vector3.Lerp(transform.position, groundedPosition, Time.deltaTime / 0.1f);
                } else {
                    transform.position = groundedPosition;
                }
            }
            
        }
    }

    public void DoJump() {
        if (isGrounded) {
            animatorManager.animator.SetBool("IsJumping", true);
            animatorManager.PlayTargetAnimation("Jump", false);

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = playerRigidbody.velocity; //moveDirection in tutorial
            playerVelocity.y = jumpingVelocity;
            playerRigidbody.velocity = playerVelocity;
        }
    }

}
