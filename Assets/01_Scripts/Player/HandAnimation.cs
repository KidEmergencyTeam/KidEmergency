using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimation : MonoBehaviour
{
    public InputActionProperty leftPinch;
    public InputActionProperty leftGrip;

    public InputActionProperty rightPinch;
    public InputActionProperty rightGrip;

    public Animator animator;
    
    public bool isLeftGrabbed = false;
    public bool isRightGrabbed = false;

    private RayController ray;

    private void Start()
    {
        ray = FindObjectOfType<RayController>();
    }

    void Update()
    {
        //RayController ray = FindObjectOfType<RayController>();

        if (!isLeftGrabbed)
        {
            if (ray.UIActive())
            {
                float leftTriggerValue = leftPinch.action.ReadValue<float>();
                animator.SetFloat("Left Grip", leftTriggerValue);

                float leftGripValue = leftGrip.action.ReadValue<float>();
                animator.SetFloat("Left Trigger", leftGripValue);
            }
            
            else
            {
                float leftTriggerValue = leftPinch.action.ReadValue<float>();
                animator.SetFloat("Left Trigger", leftTriggerValue);

                float leftGripValue = leftGrip.action.ReadValue<float>();
                animator.SetFloat("Left Grip", leftGripValue);
            }
        }

        if (!isRightGrabbed)
        {
            if (ray.UIActive())
            {
                float rightTriggerValue = rightPinch.action.ReadValue<float>();
                animator.SetFloat("Right Grip", rightTriggerValue);

                float rightGripValue = rightGrip.action.ReadValue<float>();
                animator.SetFloat("Right Trigger", rightGripValue);
            }
            else
            {
                float rightTriggerValue = rightPinch.action.ReadValue<float>();
                animator.SetFloat("Right Trigger", rightTriggerValue);

                float rightGripValue = rightGrip.action.ReadValue<float>();
                animator.SetFloat("Right Grip", rightGripValue);
            }
        }
    }
}
