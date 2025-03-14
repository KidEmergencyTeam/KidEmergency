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
	private bool m_IsGrabbed = false;

	// 디버깅용 로그 활성화
	[SerializeField] private bool m_DebugLog = true;

	public override void Spawned()
	{
		// 네트워크 오브젝트가 스폰될 때 초기화
		m_GrabInteractable = GetComponent<XRGrabInteractable>();
		m_NetworkObject = GetComponent<NetworkObject>();
		m_NetworkRigidbody = GetComponent<NetworkRigidbody3D>();
		
		// XRGrabInteractable 이벤트 등록
		m_GrabInteractable.selectEntered.AddListener(OnGrabbed);
		m_GrabInteractable.selectExited.AddListener(OnReleased);
		
		// 초기 소유자 저장
		m_OriginalOwner = Object.HasStateAuthority ? Runner.LocalPlayer : Object.StateAuthority;
		
		if (m_DebugLog)
			Debug.Log($"[{name}] Spawned. StateAuthority: {Object.StateAuthority}, HasInputAuth: {Object.HasInputAuthority}");
	}

	private void OnGrabbed(SelectEnterEventArgs args)
	{
		if (m_DebugLog)
			Debug.Log($"[{name}] Grabbed by {args.interactorObject.transform.name}, HasInputAuth: {Object.HasInputAuthority}");
		
		m_IsGrabbed = true;
		
		// 로컬 플레이어가 그랩했을 때만 소유권 요청
		if (IsLocalPlayer(args.interactorObject))
		{
			// 현재 소유자 저장 (아직 변경되지 않았을 때)
			if (!Object.HasInputAuthority)
				m_OriginalOwner = Object.StateAuthority;
			
			// 소유권 요청 - 호스트도 InputAuthority를 가져야 함
			if (!Object.HasInputAuthority)
			{
				if (m_DebugLog)
					Debug.Log($"[{name}] Requesting ownership from {Object.StateAuthority} to {Runner.LocalPlayer}");
				
				RPC_RequestObjectOwnership();
			}
			else
			{
				if (m_DebugLog)
					Debug.Log($"[{name}] Already has input authority");
			}
		}
	}

	private void OnReleased(SelectExitEventArgs args)
	{
		if (m_DebugLog)
			Debug.Log($"[{name}] Released by {args.interactorObject.transform.name}");
		
		m_IsGrabbed = false;
		
		// 로컬 플레이어가 놓았을 때만 소유권 반환
		if (IsLocalPlayer(args.interactorObject) && Object.HasInputAuthority)
		{
			// 일정 시간 후 소유권 반환 (던지기 동작이 완료된 후)
			Invoke(nameof(ReturnOwnership), 0.5f);
		}
	}

	private void ReturnOwnership()
	{
		// 다른 사람이 잡고 있지 않을 때만 소유권 반환
		if (!m_IsGrabbed && Object.HasInputAuthority)
		{
			if (m_DebugLog)
				Debug.Log($"[{name}] Returning ownership to {m_OriginalOwner}");
			
			RPC_ReturnObjectOwnership(m_OriginalOwner);
		}
	}

	private bool IsLocalPlayer(IXRInteractor interactor)
	{
		// 로컬 플레이어 확인 로직
		// 1. 인터랙터가 로컬 플레이어에 속하는지 확인
		var networkPlayer = interactor.transform.GetComponentInParent<NetworkPlayer>();
		if (networkPlayer != null && networkPlayer.Object.HasInputAuthority)
			return true;
		
		// 2. 호스트인 경우 추가 확인
		if (Runner.IsServer || Runner.IsSharedModeMasterClient)
		{
			// 호스트의 인터랙터인지 확인하는 추가 로직
			// 호스트의 인터랙터에 특별한 태그나 컴포넌트가 있다면 확인
			// 예시로 간단하게 호스트일 때는 모든 인터랙터를 로컬로 간주
			return true;
		}
		
		return false;
	}

	// 네트워크 상태 동기화를 위한 추가 메서드
	public override void FixedUpdateNetwork()
	{
		// 소유자만 물리 상태를 업데이트
		if (Object.HasInputAuthority && m_IsGrabbed)
		{
			// 필요한 경우 추가 동기화 로직
			// XRGrabInteractable이 이미 위치/회전을 처리하므로 여기서는 추가 작업 불필요
		}
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	private void RPC_RequestObjectOwnership()
	{
		// 호스트/서버에서만 실행됨
		if (Runner.IsServer || Runner.IsSharedModeMasterClient)
		{
			if (m_DebugLog)
				Debug.Log($"[{name}] RPC_RequestObjectOwnership from {RPC.Source}");
			
			Object.AssignInputAuthority(RPC.Source);
		}
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	private void RPC_ReturnObjectOwnership(PlayerRef originalOwner)
	{
		// 호스트/서버에서만 실행됨
		if (Runner.IsServer || Runner.IsSharedModeMasterClient)
		{
			if (m_DebugLog)
				Debug.Log($"[{name}] RPC_ReturnObjectOwnership to {originalOwner}");
			
			Object.AssignInputAuthority(originalOwner);
		}
	}

	// XRGrabInteractable 설정 최적화
	private void Start()
	{
		// NetworkRigidbody3D와 함께 사용할 때 최적의 설정
		if (m_GrabInteractable != null)
		{
			// 즉시 이동 모드 사용 (Kinematic도 가능)
			m_GrabInteractable.movementType = XRBaseInteractable.MovementType.Instantaneous;
			
			// 물리 충돌 방지
			m_GrabInteractable.trackPosition = true;
			m_GrabInteractable.trackRotation = true;
			m_GrabInteractable.throwOnDetach = true;
		}
	}
}