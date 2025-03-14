using System;
using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkedXRGrabInteractable : NetworkBehaviour
{
	[Networked] public bool IsGrabbed { get; set; }
	[Networked] public PlayerRef GrabbedBy { get; set; }

	// 네트워크 위치 동기화를 위한 변수
	[Networked] public Vector3 NetworkPosition { get; set; }
	[Networked] public Quaternion NetworkRotation { get; set; }

	private XRGrabInteractable _grabInteractable;
	private NetworkRigidbody3D _networkRigidbody;
	private NetworkTransform _networkTransform;
	private Rigidbody _rigidbody;

	// 그랩한 인터랙터 참조 저장
	private IXRSelectInteractor _selectingInteractor;

	// 디버깅용 변수
	private bool _isLocalPlayerGrabbing = false;

	private void Awake()
	{
		_grabInteractable = GetComponent<XRGrabInteractable>();
		_networkRigidbody = GetComponent<NetworkRigidbody3D>();
		_networkTransform = GetComponent<NetworkTransform>();
		_rigidbody = GetComponent<Rigidbody>();

		// XR 그랩 인터랙션 설정 변경
		_grabInteractable.movementType = XRBaseInteractable.MovementType.VelocityTracking;
		_grabInteractable.trackPosition = true;
		_grabInteractable.trackRotation = true;

		_grabInteractable.selectEntered.AddListener(OnGrab);
		_grabInteractable.selectExited.AddListener(OnRelease);
	}

	public override void Spawned()
	{
		base.Spawned();
		IsGrabbed = false;
		GrabbedBy = PlayerRef.None;
		NetworkPosition = transform.position;
		NetworkRotation = transform.rotation;

		// 네트워크 트랜스폼 설정 - 버전에 따라 다를 수 있으므로 주석 처리
		// if (_networkTransform != null)
		// {
		//     _networkTransform.InterpolationDataSource = InterpolationDataSources.Snapshots;
		// }
	}

	private void OnGrab(SelectEnterEventArgs args)
	{
		_selectingInteractor = args.interactorObject;

		// 로컬 플레이어가 그랩했을 때만 RPC 호출
		if (Runner.LocalPlayer != PlayerRef.None)
		{
			Debug.Log($"Local player {Runner.LocalPlayer} grabbing object");
			_isLocalPlayerGrabbing = true;
			Rpc_RequestGrab(Runner.LocalPlayer);
		}
	}

	private void OnRelease(SelectExitEventArgs args)
	{
		// 로컬 플레이어가 릴리즈했을 때만 RPC 호출
		if (_isLocalPlayerGrabbing && Runner.LocalPlayer != PlayerRef.None)
		{
			Debug.Log($"Local player {Runner.LocalPlayer} releasing object");
			_isLocalPlayerGrabbing = false;
			Rpc_RequestRelease(Runner.LocalPlayer);
		}

		_selectingInteractor = null;
	}

	// 매 프레임 로컬 업데이트
	private void Update()
	{
		// 디버깅 정보 출력
		if (IsGrabbed)
		{
			Debug.Log($"Object is grabbed by {GrabbedBy}, Local player: {Runner.LocalPlayer}, IsLocalGrabbing: {_isLocalPlayerGrabbing}");
		}

		// 로컬 플레이어가 그랩 중이고 인터랙터가 있을 때
		if (_isLocalPlayerGrabbing && _selectingInteractor != null)
		{
			// 로컬에서 위치 업데이트
			UpdateLocalPosition();
		}
	}

	// 네트워크 업데이트
	public override void FixedUpdateNetwork()
	{
		// 그랩된 상태에서 위치 동기화
		if (IsGrabbed)
		{
			if (_isLocalPlayerGrabbing && _selectingInteractor != null)
			{
				// 로컬 플레이어가 그랩 중일 때 네트워크 위치 업데이트
				NetworkPosition = transform.position;
				NetworkRotation = transform.rotation;
				
				// 디버깅 메시지
				Debug.Log($"Updating network position: {NetworkPosition}");
			}
			else if (!_isLocalPlayerGrabbing)
			{
				// 다른 플레이어가 그랩 중일 때 네트워크 위치로 업데이트
				transform.position = NetworkPosition;
				transform.rotation = NetworkRotation;
				
				// 디버깅 메시지
				Debug.Log($"Receiving network position: {NetworkPosition}");
			}
		}
	}

	// 로컬 위치 업데이트
	private void UpdateLocalPosition()
	{
		if (_selectingInteractor != null)
		{
			// XR 인터랙터의 위치와 회전 가져오기
			Transform interactorTransform = _selectingInteractor.transform;
			
			// 직접 위치 설정 (호스트와 클라이언트 모두에서 작동하도록)
			transform.position = interactorTransform.position;
			transform.rotation = interactorTransform.rotation;
			
			// 디버깅 메시지
			Debug.Log($"Local position updated to: {transform.position}");
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public void Rpc_RequestGrab(PlayerRef player)
	{
		Debug.Log($"Player {player} requested grab");
		
		// 이미 잡혀있는지 확인
		if (IsGrabbed && GrabbedBy != player)
			return;
			
		IsGrabbed = true;
		GrabbedBy = player;
		
		// 물리 속성 변경
		if (_rigidbody != null)
		{
			_rigidbody.isKinematic = true;
			_rigidbody.useGravity = false;
		}
		
		// 네트워크 리지드바디 설정
		if (_networkRigidbody != null)
		{
			_networkRigidbody.enabled = false;
		}
		
		// 로컬 플레이어가 그랩한 경우
		if (player == Runner.LocalPlayer)
		{
			_isLocalPlayerGrabbing = true;
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public void Rpc_RequestRelease(PlayerRef player)
	{
		Debug.Log($"Player {player} requested release");
		
		// 해당 플레이어가 잡고 있는지 확인
		if (!IsGrabbed || GrabbedBy != player)
			return;
			
		IsGrabbed = false;
		GrabbedBy = PlayerRef.None;
		
		// 물리 속성 복원
		if (_rigidbody != null)
		{
			_rigidbody.isKinematic = false;
			_rigidbody.useGravity = true;
		}
		
		// 네트워크 리지드바디 설정
		if (_networkRigidbody != null)
		{
			_networkRigidbody.enabled = true;
		}
		
		// 로컬 플레이어가 릴리즈한 경우
		if (player == Runner.LocalPlayer)
		{
			_isLocalPlayerGrabbing = false;
		}
	}
}