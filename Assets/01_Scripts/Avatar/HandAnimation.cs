using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimation : NetworkBehaviour
{
    public InputActionProperty leftPinch;
    public InputActionProperty leftGrip;
    public InputActionProperty rightPinch;
    public InputActionProperty rightGrip;

    private float _leftTriggerValue;
    private float _leftGripValue;
    private float _rightTriggerValue;
    private float _rightGripValue;

    public Animator animator;

    public override void Render()
    {
        UpdateAnimations();
    }
    
    private void UpdateAnimations()
    {
        _leftTriggerValue = leftPinch.action.ReadValue<float>();
        animator.SetFloat("Left Trigger", _leftTriggerValue);

        _leftGripValue = leftGrip.action.ReadValue<float>();
        animator.SetFloat("Left Grip", _leftGripValue);


        _rightTriggerValue = rightPinch.action.ReadValue<float>();
        animator.SetFloat("Right Trigger", _rightTriggerValue);

        _rightGripValue = rightGrip.action.ReadValue<float>();
        animator.SetFloat("Right Grip", _rightGripValue);
    }
}
