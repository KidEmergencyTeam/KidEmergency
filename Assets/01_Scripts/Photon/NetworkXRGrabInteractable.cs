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
	private Rigidbody m_Rigidbody;

	// 디버깅용 로그 활성화
	[SerializeField] private bool m_DebugLog = true;
	
	// 네트워크 변수 추가 - 물리 상태 동기화를 위해
	[Networked] private Vector3 NetworkPosition { get; set; }
	[Networked] private Quaternion NetworkRotation { get; set; }
	[Networked] private Vector3 NetworkVelocity { get; set; }
	[Networked] private Vector3 NetworkAngularVelocity { get; set; }
	[Networked] private NetworkBool IsKinematic { get; set; }

	private void Awake()
	{
		m_GrabInteractable = GetComponent<XRGrabInteractable>();
		m_NetworkObject = GetComponent<NetworkObject>();
		m_NetworkRigidbody = GetComponent<NetworkRigidbody3D>();
		m_Rigidbody = GetComponent<Rigidbody>();
		
		// XRGrabInteractable 이벤트 등록
		m_GrabInteractable.selectEntered.AddListener(OnGrabbed);
		m_GrabInteractable.selectExited.AddListener(OnReleased);
	}

	public override void Spawned()
	{
		// 초기 소유자 저장
		m_OriginalOwner = Object.HasStateAuthority ? Runner.LocalPlayer : Object.StateAuthority;
		
		// 초기 네트워크 상태 설정
		if (Object.HasStateAuthority)
		{
			NetworkPosition = transform.position;
			NetworkRotation = transform.rotation;
			NetworkVelocity = m_Rigidbody.velocity;
			NetworkAngularVelocity = m_Rigidbody.angularVelocity;
			IsKinematic = m_Rigidbody.isKinematic;
		}
		
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
			
			// 던지기 후 최종 상태 동기화
			if (Object.HasStateAuthority)
			{
				NetworkPosition = transform.position;
				NetworkRotation = transform.rotation;
				NetworkVelocity = m_Rigidbody.velocity;
				NetworkAngularVelocity = m_Rigidbody.angularVelocity;
				IsKinematic = m_Rigidbody.isKinematic;
			}
			
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
			// 호스트의 인터랙터인지 확인
			// 여기서는 간단히 호스트일 때 모든 인터랙터를 로컬로 간주
			return true;
		}
		
		return false;
	}

	// 네트워크 상태 동기화를 위한 메서드
	public override void FixedUpdateNetwork()
	{
		// 소유자일 때 - 물리 상태를 네트워크에 동기화
		if (Object.HasStateAuthority)
		{
			if (m_IsGrabbed && Object.HasInputAuthority)
			{
				// 잡고 있을 때 위치/회전/속도 업데이트
				NetworkPosition = transform.position;
				NetworkRotation = transform.rotation;
				NetworkVelocity = m_Rigidbody.velocity;
				NetworkAngularVelocity = m_Rigidbody.angularVelocity;
				IsKinematic = m_Rigidbody.isKinematic;
			}
			else if (!m_IsGrabbed)
			{
				// 잡고 있지 않을 때도 물리 상태 업데이트
				NetworkPosition = transform.position;
				NetworkRotation = transform.rotation;
				NetworkVelocity = m_Rigidbody.velocity;
				NetworkAngularVelocity = m_Rigidbody.angularVelocity;
				IsKinematic = m_Rigidbody.isKinematic;
			}
		}
		// 소유자가 아닐 때 - 네트워크 값으로 업데이트
		else if (!m_IsGrabbed)
		{
			// 잡고 있지 않을 때만 네트워크 값으로 업데이트
			transform.position = NetworkPosition;
			transform.rotation = NetworkRotation;
			m_Rigidbody.velocity = NetworkVelocity;
			m_Rigidbody.angularVelocity = NetworkAngularVelocity;
			m_Rigidbody.isKinematic = IsKinematic;
		}
	}

	// 물리 상태 강제 동기화 RPC
	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	private void RPC_SyncPhysicsState(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity, NetworkBool isKinematic)
	{
		if (!Object.HasInputAuthority)
		{
			transform.position = position;
			transform.rotation = rotation;
			m_Rigidbody.velocity = velocity;
			m_Rigidbody.angularVelocity = angularVelocity;
			m_Rigidbody.isKinematic = isKinematic;
		}
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	private void RPC_RequestObjectOwnership()
	{
		// 호스트/서버에서만 실행됨
		if (Runner.IsServer || Runner.IsSharedModeMasterClient)
		{
			if (m_DebugLog)
				Debug.Log($"[{name}] RPC_RequestObjectOwnership from {Runner.LocalPlayer}");
			
			Object.AssignInputAuthority(Runner.LocalPlayer);
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
			
			// 물리 상태 동기화 후 소유권 변경
			RPC_SyncPhysicsState(transform.position, transform.rotation, m_Rigidbody.velocity, m_Rigidbody.angularVelocity, m_Rigidbody.isKinematic);
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
	
	// 물리 상태 변경 시 강제 동기화
	private void LateUpdate()
	{
		// 소유자이고 물리 상태가 변경되었을 때 강제 동기화
		if (Object.HasInputAuthority && !m_IsGrabbed && HasPhysicsChanged())
		{
			RPC_SyncPhysicsState(transform.position, transform.rotation, m_Rigidbody.velocity, m_Rigidbody.angularVelocity, m_Rigidbody.isKinematic);
		}
	}
	
	// 물리 상태 변경 감지
	private bool HasPhysicsChanged()
	{
		// 위치, 회전, 속도 등이 네트워크 값과 다른지 확인
		return Vector3.Distance(transform.position, NetworkPosition) > 0.01f ||
			   Quaternion.Angle(transform.rotation, NetworkRotation) > 0.5f ||
			   Vector3.Distance(m_Rigidbody.velocity, NetworkVelocity) > 0.1f ||
			   Vector3.Distance(m_Rigidbody.angularVelocity, NetworkAngularVelocity) > 0.1f ||
			   m_Rigidbody.isKinematic != IsKinematic;
	}
}