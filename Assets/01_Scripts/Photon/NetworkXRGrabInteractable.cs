using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(NetworkRigidbody3D))]
[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(Rigidbody))]
public class NetworkedXRGrabInteractable : NetworkBehaviour
{
	[Networked] public Vector3 Pos { get; set; }
	[Networked] public Quaternion Rot { get; set; }
	[Networked] public NetworkBool IsGrabbed { get; set; } = false;

	private XRGrabInteractable _grabInteractable;
	private NetworkObject _networkObject;
	private Rigidbody _rb;
	private NetworkRigidbody3D _networkRb;
	private GameObject _grabber;

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
		IsGrabbed = true;
		_rb.isKinematic = true;
		_grabber = args.interactor.gameObject;
		print($"{_grabber.name} is grabbed");
	}

	private void OnRelease(SelectExitEventArgs args)
	{
		_rb.isKinematic = false;
		IsGrabbed = false;
		_grabber = null;
	}

	public override void FixedUpdateNetwork()
	{
		if (IsGrabbed)
		{
			if (_grabber)
			{
				Pos = transform.position;
				Rot = transform.rotation;
			}
		}
	}

	public override void Render()
	{
		if (IsGrabbed)
		{
			transform.position = Pos;
			transform.rotation = Rot;
		}
	}
}