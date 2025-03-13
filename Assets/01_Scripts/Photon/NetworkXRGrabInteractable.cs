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
	[Networked] public NetworkBool IsGrabbed { get; set; } = false;

	private XRGrabInteractable _grabInteractable;
	private NetworkObject _networkObject;
	private Rigidbody _rb;
	private NetworkRigidbody3D _networkRb;
	private GameObject _currentInteractor;

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
		Runner.SetIsSimulated(Object, true);
		IsGrabbed = true;
		_rb.isKinematic = true;
		_currentInteractor = args.interactorObject.transform.gameObject;
		Object.AssignInputAuthority(_currentInteractor
			.GetComponentInParent<NetworkObject>().InputAuthority);
	}

	private void OnRelease(SelectExitEventArgs args)
	{
		Runner.SetIsSimulated(Object, false);
		_rb.isKinematic = false;
		IsGrabbed = false;
		_currentInteractor = null;
		Object.RemoveInputAuthority();
	}

	public override void FixedUpdateNetwork()
	{
		if (!IsGrabbed) return;

		Follow(transform, _currentInteractor.transform);
	}

	public override void Render()
	{
		if (IsGrabbed)
		{
			Follow(transform, _currentInteractor.transform);
		}
	}

	public void Follow(Transform followingTransform, Transform follwedTransform)
	{
		followingTransform.position = follwedTransform.position;
		followingTransform.rotation = follwedTransform.rotation;
	}
}