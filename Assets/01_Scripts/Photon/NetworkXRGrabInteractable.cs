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
	}

	private void OnRelease(SelectExitEventArgs args)
	{
		isGrabbed = false;
	}

	public override void FixedUpdateNetwork()
	{
		if (isGrabbed)
		{
			Pos = transform.position;
			Rot = transform.rotation;
		}
	}

	public override void Render()
	{
		transform.position = Pos;
		transform.rotation = Rot;
	}
}