using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkedXRGrabInteractable : NetworkBehaviour
{
	[Networked] private Vector3 Pos { get; set; }
	[Networked] private Quaternion Rot { get; set; }
	public bool isGrabbed = false;
	private XRGrabInteractable _grabInteractable;

	private void Awake()
	{
		_grabInteractable = GetComponent<XRGrabInteractable>();

		_grabInteractable.selectEntered.AddListener(OnGrab);
		_grabInteractable.selectExited.AddListener(OnRelease);
	}

	private void OnGrab(SelectEnterEventArgs args)
	{
		isGrabbed = true;
		Rpc_RequestGrab(Runner.LocalPlayer);
	}

	private void OnRelease(SelectExitEventArgs args)
	{
		isGrabbed = false;
		Rpc_RequestRelease();
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public void Rpc_RequestGrab(PlayerRef player)
	{
		if (Object.HasStateAuthority)
		{
			print("Requesting Grab");
			Object.AssignInputAuthority(player);
		}
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public void Rpc_RequestRelease()
	{
		if (Object.HasStateAuthority)
		{
			print("Requesting Release");
			Object.RemoveInputAuthority();
		}
	}
}