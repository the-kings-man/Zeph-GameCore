using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    /// <summary>
    /// The object the camera will follow
    /// </summary>
    public Transform targetTransform;
    /// <summary>
    /// The object the camera uses to pivot
    /// </summary>
    public Transform cameraPivotTransform; 
    public InputManager inputManager;
    /// <summary>
    /// The transform of the actual camera object
    /// </summary>
    public Transform cameraTransform;
    /// <summary>
    /// The layers we want our camera to collide with
    /// </summary>
    public LayerMask collisionLayers;

    /// <summary>
    /// How much the camera will jump off of objects its colliding with
    /// </summary>
    public float cameraCollisionOffset = 0.2f;
    public float minimumCollisionOffset = 0.2f;
    public float cameraCollisionRadius = 0.2f;
    public float cameraFollowSpeed = 0.2f;
    public float cameraLookSpeed = 2f;
    public float cameraPivotSpeed = 2f;

    public float lookAngle; //camera look up and down
    public float pivotAngle; //camera look left and right
    public float minimumPivotAngle = -35f;
    public float maximumPivotAngle = 35f;

    private Vector3 cameraFollowVelocity = Vector3.zero;
    private float defaultPosition;
    private Vector3 cameraVectorPosition;
    
    private void Awake() {
        defaultPosition = cameraTransform.localPosition.z;
    }

    public void HandleAllCameraMovement() {
        FollowTarget();
        RotateCamera();
        HandleCameraCollisions();
    }

    void FollowTarget() {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed); //move something softly between 1 location to another

        transform.position = targetPosition;
    }

    void RotateCamera() {
        Vector3 rotation;
        Quaternion targetRotation;

        lookAngle = lookAngle + (inputManager.cameraInputHorizontal * cameraLookSpeed);
        pivotAngle = pivotAngle - (inputManager.cameraInputVertical * cameraPivotSpeed);
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle);

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivotTransform.localRotation = targetRotation;
    }

    void HandleCameraCollisions() {
        float targetPosition = defaultPosition;

        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayers)) {
            float distance = Vector3.Distance(cameraPivotTransform.position, hit.point); //distance between pivot and thing we hit
            targetPosition = - (distance - cameraCollisionOffset);
        }

        if (Mathf.Abs(targetPosition) < minimumCollisionOffset) {
            targetPosition = targetPosition - minimumCollisionOffset;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f); //0.2f is time
        cameraTransform.localPosition = cameraVectorPosition;
    }
}
