using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable), typeof(NetworkObject), typeof(Rigidbody))]
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
            networkObject.RequestStateAuthority();  // 플레이어가 잡으면 네트워크 권한 요청
        }

        isGrabbed = true;
        grabberTransform = args.interactorObject.transform;
        rb.isKinematic = true; // XRGrabInteractable 특성상 kinematic 설정 필요
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (networkObject.HasStateAuthority)
        {
            networkObject.ReleaseStateAuthority();
        }

        isGrabbed = false;
        grabberTransform = null;
        rb.isKinematic = false; // 물리 활성화
    }

    private void FixedUpdate()
    {
        if (isGrabbed && grabberTransform != null && networkObject.HasStateAuthority)
        {
            // 네트워크 상에서 Rigidbody를 이동시키도록 처리
            rb.MovePosition(grabberTransform.position);
            rb.MoveRotation(grabberTransform.rotation);
        }
    }
}
