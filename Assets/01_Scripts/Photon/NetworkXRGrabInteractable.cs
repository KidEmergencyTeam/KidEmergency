using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Fusion;
using Fusion.Addons.Physics;

public class NetworkGrabbable : NetworkBehaviour
{
	private XRGrabInteractable m_GrabInteractable;
	private NetworkObject m_NetworkObject;
	private NetworkRigidbody3D m_NetworkRigidbody;
	private PlayerRef m_OriginalOwner;
	private bool m_WasKinematic;

	private void Awake()
	{
		m_GrabInteractable = GetComponent<XRGrabInteractable>();
		m_NetworkObject = GetComponent<NetworkObject>();
		m_NetworkRigidbody = GetComponent<NetworkRigidbody3D>();

		// XRGrabInteractable 이벤트 등록
		m_GrabInteractable.selectEntered.AddListener(OnGrabbed);
		m_GrabInteractable.selectExited.AddListener(OnReleased);
	}

	private void OnGrabbed(SelectEnterEventArgs args)
	{
		// 로컬 플레이어가 그랩했을 때만 소유권 요청
		if (IsLocalPlayer(args.interactorObject))
		{
			// 현재 소유자 저장
			m_OriginalOwner = m_NetworkObject.HasStateAuthority
				? Runner.LocalPlayer
				: m_NetworkObject.StateAuthority;
			m_WasKinematic = m_NetworkRigidbody.RBIsKinematic;

			// 소유권 요청 RPC 호출
			if (!m_NetworkObject.HasInputAuthority)
			{
				RPC_RequestObjectOwnership();
			}
		}
	}

	private void OnReleased(SelectExitEventArgs args)
	{
		// 로컬 플레이어가 놓았을 때만 소유권 반환
		if (IsLocalPlayer(args.interactorObject) && m_NetworkObject.HasInputAuthority)
		{
			// 일정 시간 후 소유권 반환 (즉시 반환하면 던지기 동작이 제대로 동기화되지 않을 수 있음)
			Invoke(nameof(ReturnOwnership), 0.5f);
		}
	}

	private void ReturnOwnership()
	{
		// 소유권을 원래 소유자에게 반환
		if (m_NetworkObject.HasInputAuthority)
		{
			RPC_ReturnObjectOwnership(m_OriginalOwner);
		}
	}

	private bool IsLocalPlayer(IXRInteractor interactor)
	{
		// 인터랙터가 로컬 플레이어에 속하는지 확인하는 로직
		// 구현은 네트워크 플레이어 설정에 따라 달라질 수 있음
		var networkPlayer = interactor.transform.GetComponentInParent<NetworkPlayer>();
		return networkPlayer != null && networkPlayer.Object.HasInputAuthority;
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	private void RPC_RequestObjectOwnership()
	{
		// 호스트/서버에서만 실행됨
		if (Runner.IsServer || Runner.IsSharedModeMasterClient)
		{
			m_NetworkObject.AssignInputAuthority(Runner.LocalPlayer);
		}
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	private void RPC_ReturnObjectOwnership(PlayerRef originalOwner)
	{
		// 호스트/서버에서만 실행됨
		if (Runner.IsServer || Runner.IsSharedModeMasterClient)
		{
			m_NetworkObject.AssignInputAuthority(originalOwner);
		}
	}
}