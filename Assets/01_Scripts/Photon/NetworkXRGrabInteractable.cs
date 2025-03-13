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
	[Networked] public NetworkBool IsGrabbed { get; set; } = false;

	private XRGrabInteractable _grabInteractable;
	private Rigidbody _rb;
	private Transform _currentInteractor;

	private void Awake()
	{
		_grabInteractable = GetComponent<XRGrabInteractable>();
		_rb = GetComponent<Rigidbody>();

		_grabInteractable.selectEntered.AddListener(OnGrab);
		_grabInteractable.selectExited.AddListener(OnRelease);
	}

	private void OnGrab(SelectEnterEventArgs args)
	{
		if (Object.HasStateAuthority || Object.HasInputAuthority)
		{
			if (!Object.HasInputAuthority)
			{
				_currentInteractor = args.interactorObject.transform;
				Object.AssignInputAuthority(Object.InputAuthority);
			}

			IsGrabbed = true;
			_rb.isKinematic = true;
		}
	}

	private void OnRelease(SelectExitEventArgs args)
	{
		if (Object.HasInputAuthority)
		{
			_currentInteractor = null;
			Object.RemoveInputAuthority();
			IsGrabbed = false;
			_rb.isKinematic = false;
		}
	}

	public override void FixedUpdateNetwork()
	{
		if (!IsGrabbed) return;

		Follow(transform, _currentInteractor);
	}

	public override void Render()
	{
		if (IsGrabbed)
		{
			Follow(transform, _currentInteractor);
		}
	}

	public void Follow(Transform followingTransform, Transform follwedTransform)
	{
		followingTransform.position = follwedTransform.position;
		followingTransform.rotation = follwedTransform.rotation;
	}
}