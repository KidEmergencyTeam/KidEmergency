using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimation : NetworkBehaviour
{
    [Networked] public float leftTriggerValue{ get; set; }
    [Networked] public float leftGripValue{ get; set; }
    [Networked] public float rightTriggerValue{ get; set; }
    [Networked] public float rightGripValue{ get; set; }
    
    public InputActionProperty leftPinch;
    public InputActionProperty leftGrip;
    public InputActionProperty rightPinch;
    public InputActionProperty rightGrip;

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
