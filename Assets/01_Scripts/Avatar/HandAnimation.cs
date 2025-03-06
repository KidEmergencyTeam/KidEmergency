using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimation : NetworkBehaviour
{
    [Networked] public InputActionProperty leftPinch { get; set; }
    [Networked] public InputActionProperty leftGrip { get; set; }
    [Networked] public InputActionProperty rightPinch { get; set; }
    [Networked] public InputActionProperty rightGrip { get; set; }

    public float leftTriggerValue;
    public float leftGripValue;
    public float rightTriggerValue;
    public float rightGripValue;

    public Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public override void Render()
    {
        UpdateAnimations();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (Object.HasInputAuthority)
        {
            leftTriggerValue = leftPinch.action.ReadValue<float>();
            leftGripValue = leftGrip.action.ReadValue<float>();
            rightTriggerValue = rightPinch.action.ReadValue<float>();
            rightGripValue = rightGrip.action.ReadValue<float>();
        }
    }

    private void UpdateAnimations()
    {
        animator.SetFloat("Left Trigger", leftTriggerValue);
        animator.SetFloat("Left Grip", leftGripValue);
        animator.SetFloat("Right Trigger", rightTriggerValue);
        animator.SetFloat("Right Grip", rightGripValue);
    }
}
