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
	private XRGrabInteractable grabInteractable;
	private NetworkObject networkObject;
	private Rigidbody rb;
	private NetworkRigidbody3D networkRb;
	private Transform grabberTransform;
	private bool isGrabbed = false;

	private void Awake()
	{
		grabInteractable = GetComponent<XRGrabInteractable>();
		networkObject = GetComponent<NetworkObject>();
		rb = GetComponent<Rigidbody>();
		networkRb = GetComponent<NetworkRigidbody3D>();

		grabInteractable.selectEntered.AddListener(OnGrab);
		grabInteractable.selectExited.AddListener(OnRelease);
	}

	private void OnGrab(SelectEnterEventArgs args)
	{
		if (!Runner.IsServer && !networkObject.HasStateAuthority)
		{
			networkObject.RequestStateAuthority();
		}

		isGrabbed = true;
		grabberTransform = args.interactorObject.transform;
		rb.isKinematic = true;
	}

	private void OnRelease(SelectExitEventArgs args)
	{
		if (networkObject.HasStateAuthority)
		{
			networkObject.ReleaseStateAuthority();
		}

		isGrabbed = false;
		grabberTransform = null;
		rb.isKinematic = false;
	}

	private void FixedUpdate()
	{
		Debug.Log(
			$"isGrabbed: {isGrabbed}, grabberTransform: {grabberTransform}, HasStateAuthority: {networkObject.HasStateAuthority}");

		if (isGrabbed && grabberTransform != null)
		{
			if (!networkObject.HasStateAuthority)
			{
				Debug.LogError("권한이 없습니다!"); // 권한이 없을 때 에러 로그 출력
				return;
			}

			// 로그 추가: grabberTransform의 위치와 회전 출력
			Debug.Log(
				$"Grabber Position: {grabberTransform.position}, Rotation: {grabberTransform.rotation}");

			// 네트워크 상에서 Rigidbody를 이동시키도록 처리
			rb.MovePosition(grabberTransform.position);
			rb.MoveRotation(grabberTransform.rotation);
		}
	}
}