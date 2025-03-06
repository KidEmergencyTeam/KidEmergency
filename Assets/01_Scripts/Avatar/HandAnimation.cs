using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public struct HandCommand : INetworkStruct
{
    public float leftTriggerValue;
    public float leftGripValue;
    public float rightTriggerValue;
    public float rightGripValue;
}

public class HandAnimation : NetworkBehaviour
{
    public InputActionProperty leftPinch;
    public InputActionProperty leftGrip;
    public InputActionProperty rightPinch;
    public InputActionProperty rightGrip;

    public Animator animator;
    
    [Networked] public HandCommand handCommand { get; set; }

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
            HandCommand command = handCommand;
            command.leftTriggerValue = leftPinch.action.ReadValue<float>();
            command.leftGripValue = leftGrip.action.ReadValue<float>();
            command.rightTriggerValue = rightPinch.action.ReadValue<float>();
            command.rightGripValue = rightGrip.action.ReadValue<float>();
        }
    }

    private void UpdateAnimations()
    {
        animator.SetFloat("Left Trigger", handCommand.leftTriggerValue);
        animator.SetFloat("Left Grip", handCommand.leftGripValue);
        animator.SetFloat("Right Trigger", handCommand.rightTriggerValue);
        animator.SetFloat("Right Grip", handCommand.rightGripValue);
    }
}
