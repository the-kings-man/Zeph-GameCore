using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    public Animator animator;
    int horizontal;
    int vertical;

    public bool snapMovementAnimations = true;

    private void Awake() {
        animator = GetComponent<Animator>();
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
    }

    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting) {
        //Animation Snapping, can be nice if animations don't totally go together, to snap them instead of blending them
        float snappedHorizontal = horizontalMovement;
        float snappedVertical = verticalMovement;

        if (snapMovementAnimations) {
            #region "Snapped Hozirontal"
            if (horizontalMovement > 0 && horizontalMovement < 0.55f) {
                snappedHorizontal = 0.5f;
            } else if (horizontalMovement > 0.55f) {
                snappedHorizontal = 1f;
            } else if (horizontalMovement < 0 && horizontalMovement > 0.55f) {
                snappedHorizontal = -0.5f;
            } else if (horizontalMovement < -0.55f) {
                snappedHorizontal = -1f;
            } else {
                snappedHorizontal = 0f;
            }
            #endregion

            #region "Snapped Vertical"
            if (verticalMovement > 0 && verticalMovement < 0.55f) {
                snappedVertical = 0.5f;
            } else if (verticalMovement > 0.55f) {
                snappedVertical = 1f;
            } else if (verticalMovement < 0 && verticalMovement > 0.55f) {
                snappedVertical = -0.5f;
            } else if (verticalMovement < -0.55f) {
                snappedVertical = -1f;
            } else {
                snappedVertical = 0f;
            }
            #endregion

            if (isSprinting) {
                snappedHorizontal = horizontalMovement; //is there a bug herE? As this would not be the snappped value above...
                snappedVertical = 2;
            }
        }

        animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime); //dampTime -> Blend time, time between animations
        animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
    }

    public void PlayTargetAnimation(string targetAnimation, bool isInteracting) {
        animator.SetBool("IsInteracting", isInteracting);
        animator.CrossFade(targetAnimation, 0.2f);
    }
}
