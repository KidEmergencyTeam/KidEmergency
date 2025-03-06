using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimation : NetworkBehaviour
{
    public InputActionProperty leftPinch;
    public InputActionProperty leftGrip;
    public InputActionProperty rightPinch;
    public InputActionProperty rightGrip;

    [Networked] public float leftTriggerValue { get; set; }
    [Networked] public float leftGripValue { get; set; }
    [Networked] public float rightTriggerValue { get; set; }
    [Networked] public float rightGripValue { get; set; }

    public Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public override void Render()
    {
        UpdateAnimations();
    }
    
    private void UpdateAnimations()
    {
        leftTriggerValue = leftPinch.action.ReadValue<float>();
        animator.SetFloat("Left Trigger", leftTriggerValue);

        leftGripValue = leftGrip.action.ReadValue<float>();
        animator.SetFloat("Left Grip", leftGripValue);


        rightTriggerValue = rightPinch.action.ReadValue<float>();
        animator.SetFloat("Right Trigger", rightTriggerValue);

        rightGripValue = rightGrip.action.ReadValue<float>();
        animator.SetFloat("Right Grip", rightGripValue);
    }
}
