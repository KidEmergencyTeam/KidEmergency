using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimation2 : MonoBehaviour
{
    public InputActionProperty leftPinch;
    public InputActionProperty leftGrip;

    public InputActionProperty rightPinch;
    public InputActionProperty rightGrip;

    public Animator animator;

    public bool isLeftGrabbed = false;
    public bool isRightGrabbed = false;

    void Update()
    {
        if (!isLeftGrabbed)
        {
            float leftTriggerValue = leftPinch.action.ReadValue<float>();
            animator.SetFloat("Left Grip", leftTriggerValue);

            float leftGripValue = leftGrip.action.ReadValue<float>();
            animator.SetFloat("Left Trigger", leftGripValue);
        }

        if (!isRightGrabbed)
        {
            float rightTriggerValue = rightPinch.action.ReadValue<float>();
            animator.SetFloat("Right Grip", rightTriggerValue);

            float rightGripValue = rightGrip.action.ReadValue<float>();
            animator.SetFloat("Right Trigger", rightGripValue);
        }
    }
}
