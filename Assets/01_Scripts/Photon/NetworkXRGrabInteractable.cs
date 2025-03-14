using System;
using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkedXRGrabInteractable : NetworkBehaviour
{
	[Networked] public bool IsGrabbed { get; set; }
	[Networked] public PlayerRef GrabbedBy { get; set; }

	private XRGrabInteractable _grabInteractable;
	private NetworkRigidbody3D _networkRigidbody;
	private NetworkTransform _networkTransform;
	private Rigidbody _rigidbody;

	// 그랩한 인터랙터 참조 저장
	private IXRSelectInteractor _selectingInteractor;

	// 네트워크 위치 동기화를 위한 변수
	[Networked] private Vector3 NetworkPosition { get; set; }
	[Networked] private Quaternion NetworkRotation { get; set; }

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
			Rpc_RequestGrab(Runner.LocalPlayer);
		}
	}

	private void OnRelease(SelectExitEventArgs args)
	{
		// 로컬 플레이어가 릴리즈했을 때만 RPC 호출
		if (Runner.LocalPlayer != PlayerRef.None && Runner.LocalPlayer == GrabbedBy)
		{
			Rpc_RequestRelease(Runner.LocalPlayer);
		}

		_selectingInteractor = null;
	}

	// 매 프레임 로컬 업데이트
	private void Update()
	{
		// 로컬 플레이어가 그랩 중이고 인터랙터가 있을 때
		if (IsGrabbed && Runner.LocalPlayer == GrabbedBy && _selectingInteractor != null)
		{
			// 로컬에서 위치 업데이트 (부드러운 움직임을 위해)
			UpdateGrabbedPosition();
		}
	}

	// 네트워크 업데이트
	public override void FixedUpdateNetwork()
	{
		// 그랩된 상태에서 위치 동기화
		if (IsGrabbed)
		{
			// 그랩한 플레이어만 위치 업데이트 전송
			if (Object.HasInputAuthority && _selectingInteractor != null)
			{
				// 네트워크 위치 업데이트
				NetworkPosition = transform.position;
				NetworkRotation = transform.rotation;
			}
			else if (!Object.HasInputAuthority)
			{
				// 다른 플레이어들은 네트워크 위치로 업데이트
				transform.position = NetworkPosition;
				transform.rotation = NetworkRotation;
			}
		}
	}

	// 그랩된 오브젝트 위치 업데이트
	private void UpdateGrabbedPosition()
	{
		if (_selectingInteractor != null)
		{
			// XR 인터랙터의 위치와 회전 가져오기
			Transform interactorTransform = _selectingInteractor.transform;

			// 위치와 회전 업데이트
			if (_grabInteractable.movementType == XRBaseInteractable.MovementType.VelocityTracking)
			{
				// 속도 기반 추적 (더 부드러운 움직임)
				Vector3 targetPosition = interactorTransform.position;
				Quaternion targetRotation = interactorTransform.rotation;

				// 리지드바디 속도 설정
				if (_rigidbody != null && !_rigidbody.isKinematic)
				{
					Vector3 velocity = (targetPosition - transform.position) / Time.deltaTime;
					_rigidbody.velocity = velocity;

					Quaternion rotationDelta = targetRotation * Quaternion.Inverse(transform.rotation);
					float angle;
					Vector3 axis;
					rotationDelta.ToAngleAxis(out angle, out axis);

					if (angle > 180)
						angle -= 360;

					Vector3 angularVelocity = axis * angle * Mathf.Deg2Rad / Time.deltaTime;
					_rigidbody.angularVelocity = angularVelocity;
				}
				else
				{
					// 키네마틱인 경우 직접 위치 설정
					transform.position = targetPosition;
					transform.rotation = targetRotation;
				}
			}
			else
			{
				// 직접 위치 설정
				transform.position = interactorTransform.position;
				transform.rotation = interactorTransform.rotation;
			}
		}
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public void Rpc_RequestGrab(PlayerRef player)
	{
		if (Object.HasStateAuthority)
		{
			Debug.Log($"Player {player} requested grab");

			// 이미 잡혀있는지 확인
			if (IsGrabbed && GrabbedBy != player)
				return;

			IsGrabbed = true;
			GrabbedBy = player;

			// 입력 권한 할당
			Object.AssignInputAuthority(player);

			// 모든 클라이언트에게 그랩 상태 동기화
			Rpc_SyncGrabState(true, player);
		}
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public void Rpc_RequestRelease(PlayerRef player)
	{
		if (Object.HasStateAuthority)
		{
			Debug.Log($"Player {player} requested release");

			// 해당 플레이어가 잡고 있는지 확인
			if (!IsGrabbed || GrabbedBy != player)
				return;

			IsGrabbed = false;
			GrabbedBy = PlayerRef.None;

			// 입력 권한 제거
			Object.RemoveInputAuthority();

			// 모든 클라이언트에게 릴리즈 상태 동기화
			Rpc_SyncGrabState(false, PlayerRef.None);
		}
	}

	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public void Rpc_SyncGrabState(bool grabbed, PlayerRef player)
	{
		IsGrabbed = grabbed;
		GrabbedBy = player;

		if (grabbed)
		{
			// 그랩 시 물리 속성 변경
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
		}
		else
		{
			// 릴리즈 시 물리 속성 복원
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
		}
	}
}