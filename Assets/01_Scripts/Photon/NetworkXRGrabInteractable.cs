using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(NetworkRigidbody3D))]
[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(Rigidbody))]
public class NetworkedXRGrabInteractable : NetworkBehaviour
{
	[Networked] public Vector3 Pos { get; set; }
	[Networked] public Quaternion Rot { get; set; }

	private XRGrabInteractable _grabInteractable;
	private NetworkObject _networkObject;
	private Rigidbody _rb;
	private NetworkRigidbody3D _networkRb;
	private bool _isGrabbed = false;
	private NetworkObject _grabber;

	private void Awake()
	{
		_grabInteractable = GetComponent<XRGrabInteractable>();
		_networkObject = GetComponent<NetworkObject>();
		_rb = GetComponent<Rigidbody>();
		_networkRb = GetComponent<NetworkRigidbody3D>();

		_grabInteractable.selectEntered.AddListener(OnGrab);
		_grabInteractable.selectExited.AddListener(OnRelease);
	}

	private void OnGrab(SelectEnterEventArgs args)
	{
		_isGrabbed = true;
		_rb.isKinematic = true;
		_grabber = args.interactor.GetComponent<NetworkObject>();
	}

	private void OnRelease(SelectExitEventArgs args)
	{
		_rb.isKinematic = false;
		_isGrabbed = false;
		_grabber = null;
	}

	public override void FixedUpdateNetwork()
	{
		if (_isGrabbed)
		{
			if (_grabber.HasInputAuthority)
			{
				Pos = transform.position;
				Rot = transform.rotation;
			}
		}
	}

	public override void Render()
	{
		if (_isGrabbed)
		{
			if (HasStateAuthority)
			{
				transform.position = Pos;
				transform.rotation = Rot;
			}
		}
	}
}