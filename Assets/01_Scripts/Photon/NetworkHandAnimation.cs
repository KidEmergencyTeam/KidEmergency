using Fusion;
using UnityEngine;

[DefaultExecutionOrder(NetworkHandAnimation.ExecutionOrder)]
public class NetworkHandAnimation : NetworkBehaviour
{
	private const int ExecutionOrder = 100;
	
	public HardwareHandAnimation hardwareHandAnimation;
	public Animator animator;
	[HideInInspector] public NetworkTransform networkTransform;

	public bool IsLocalNetworkRig => Object.HasInputAuthority;

	private void Awake()
	{
		networkTransform = GetComponent<NetworkTransform>();
	}

	public override void FixedUpdateNetwork()
	{
		base.FixedUpdateNetwork();

		if (GetInput<HandInput>(out var input))
		{
			animator.SetFloat("Left Trigger", input.leftTriggerValue);
			animator.SetFloat("Left Grip", input.leftGripValue);
			animator.SetFloat("Right Trigger", input.rightTriggerValue);
			animator.SetFloat("Right Grip", input.rightGripValue);
		}
	}

	public override void Render()
	{
		base.Render();

		if (IsLocalNetworkRig)
		{
			animator.SetFloat("Left Trigger", hardwareHandAnimation. leftPinch.action.ReadValue<float>());
			animator.SetFloat("Left Grip", hardwareHandAnimation.leftGrip.action.ReadValue<float>());
			animator.SetFloat("Right Trigger", hardwareHandAnimation.rightPinch.action.ReadValue<float>());
			animator.SetFloat("Right Grip", hardwareHandAnimation.rightGrip.action.ReadValue<float>());
		}
		
	}
}