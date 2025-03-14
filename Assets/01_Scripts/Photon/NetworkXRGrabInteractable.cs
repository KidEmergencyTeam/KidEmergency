using Fusion;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkedXRGrabInteractable : NetworkBehaviour
{
	private XRGrabInteractable _grabInteractable;

	private void Awake()
	{
		_grabInteractable = GetComponent<XRGrabInteractable>();

		_grabInteractable.selectEntered.AddListener(OnGrab);
		_grabInteractable.selectExited.AddListener(OnRelease);
	}

	private void OnGrab(SelectEnterEventArgs args)
	{
	}

	private void OnRelease(SelectExitEventArgs args)
	{
	}

	public override void FixedUpdateNetwork()
	{
	}
}