using System;
using UnityEngine;
using Fusion;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkedXRGrabInteractable : NetworkBehaviour
{
	[Networked] public bool IsGrabbed { get; set; }
	[Networked] public PlayerRef GrabbedBy { get; set; }

	// 네트워크 위치 및 물리 동기화를 위한 변수
	[Networked] public Vector3 NetworkPosition { get; set; }
	[Networked] public Quaternion NetworkRotation { get; set; }
	[Networked] public Vector3 NetworkVelocity { get; set; }
	[Networked] public Vector3 NetworkAngularVelocity { get; set; }

	private XRGrabInteractable _grabInteractable;
	private Rigidbody _rigidbody;

	// 그랩한 인터랙터 참조 저장
	private IXRSelectInteractor _selectingInteractor;

	// 던지기 관련 변수
	private Vector3 _previousPosition;
	private Quaternion _previousRotation;
	private float _throwVelocityScale = 1.5f; // 던지기 속도 배율
	private float _throwAngularVelocityScale = 1.0f; // 던지기 회전 속도 배율

	// 디버깅용 변수
	private bool _isLocalPlayerGrabbing = false;

	private void Awake()
	{
		_grabInteractable = GetComponent<XRGrabInteractable>();
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
		NetworkVelocity = Vector3.zero;
		NetworkAngularVelocity = Vector3.zero;
	}

	private void OnGrab(SelectEnterEventArgs args)
	{
		_selectingInteractor = args.interactorObject;

		// 로컬 플레이어가 그랩했을 때만 RPC 호출
		if (Runner.LocalPlayer != PlayerRef.None)
		{
			Debug.Log($"Local player {Runner.LocalPlayer} grabbing object");
			_isLocalPlayerGrabbing = true;

			// 이전 위치 초기화 (던지기 계산용)
			_previousPosition = transform.position;
			_previousRotation = transform.rotation;

			Rpc_RequestGrab(Runner.LocalPlayer);
		}
	}

	private void OnRelease(SelectExitEventArgs args)
	{
		// 로컬 플레이어가 릴리즈했을 때만 RPC 호출
		if (_isLocalPlayerGrabbing && Runner.LocalPlayer != PlayerRef.None)
		{
			Debug.Log($"Local player {Runner.LocalPlayer} releasing object");

			// 던지기 속도 계산
			Vector3 throwVelocity = CalculateThrowVelocity();
			Vector3 throwAngularVelocity = CalculateThrowAngularVelocity();

			_isLocalPlayerGrabbing = false;
			Rpc_RequestRelease(Runner.LocalPlayer, throwVelocity, throwAngularVelocity);
		}

		_selectingInteractor = null;
	}

	// 던지기 속도 계산
	private Vector3 CalculateThrowVelocity()
	{
		if (_selectingInteractor == null) return Vector3.zero;

		// 현재 위치와 이전 위치의 차이로 속도 계산
		Vector3 velocity = (transform.position - _previousPosition) / Time.deltaTime;
		return velocity * _throwVelocityScale;
	}

	// 던지기 각속도 계산
	private Vector3 CalculateThrowAngularVelocity()
	{
		if (_selectingInteractor == null) return Vector3.zero;

		// 현재 회전과 이전 회전의 차이로 각속도 계산
		Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(_previousRotation);
		deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);

		if (angle > 180)
			angle -= 360;

		Vector3 angularVelocity = axis * (angle * Mathf.Deg2Rad / Time.deltaTime);
		return angularVelocity * _throwAngularVelocityScale;
	}

	// 매 프레임 로컬 업데이트
	private void Update()
	{
		// 로컬 플레이어가 그랩 중이고 인터랙터가 있을 때
		if (_isLocalPlayerGrabbing && _selectingInteractor != null)
		{
			// 이전 위치 저장 (던지기 계산용)
			_previousPosition = transform.position;
			_previousRotation = transform.rotation;

			// 로컬에서 위치 업데이트
			UpdateLocalPosition();
		}
	}

	// 네트워크 업데이트
	public override void FixedUpdateNetwork()
	{
		if (IsGrabbed)
		{
			// 그랩 중인 경우
			if (_isLocalPlayerGrabbing && _selectingInteractor != null)
			{
				// 로컬 플레이어가 그랩 중일 때 네트워크 위치 업데이트
				NetworkPosition = transform.position;
				NetworkRotation = transform.rotation;
			}
			else if (!_isLocalPlayerGrabbing)
			{
				// 다른 플레이어가 그랩 중일 때 네트워크 위치로 업데이트
				transform.position = NetworkPosition;
				transform.rotation = NetworkRotation;
			}
		}
		else
		{
			// 그랩 중이 아닌 경우 (물리 시뮬레이션)
			if (Object.HasStateAuthority)
			{
				// 상태 권한이 있는 경우 (호스트) - 물리 시뮬레이션 결과를 네트워크로 전송
				NetworkPosition = transform.position;
				NetworkRotation = transform.rotation;
				NetworkVelocity = _rigidbody.velocity;
				NetworkAngularVelocity = _rigidbody.angularVelocity;
			}
			else
			{
				// 상태 권한이 없는 경우 (클라이언트) - 네트워크 값으로 업데이트
				transform.position = NetworkPosition;
				transform.rotation = NetworkRotation;
				_rigidbody.velocity = NetworkVelocity;
				_rigidbody.angularVelocity = NetworkAngularVelocity;
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
			_rigidbody.velocity = Vector3.zero;
			_rigidbody.angularVelocity = Vector3.zero;
		}

		// 로컬 플레이어가 그랩한 경우
		if (player == Runner.LocalPlayer)
		{
			_isLocalPlayerGrabbing = true;
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public void Rpc_RequestRelease(PlayerRef player, Vector3 throwVelocity, Vector3 throwAngularVelocity)
	{
		Debug.Log($"Player {player} requested release with velocity: {throwVelocity}");

		// 해당 플레이어가 잡고 있는지 확인
		if (!IsGrabbed || GrabbedBy != player)
			return;

		IsGrabbed = false;
		GrabbedBy = PlayerRef.None;

		// 물리 속성 복원 및 던지기 속도 적용
		if (_rigidbody != null)
		{
			_rigidbody.isKinematic = false;
			_rigidbody.useGravity = true;
			_rigidbody.velocity = throwVelocity;
			_rigidbody.angularVelocity = throwAngularVelocity;
		}

		// 네트워크 속도 업데이트
		NetworkVelocity = throwVelocity;
		NetworkAngularVelocity = throwAngularVelocity;

		// 로컬 플레이어가 릴리즈한 경우
		if (player == Runner.LocalPlayer)
		{
			_isLocalPlayerGrabbing = false;
		}
	}
}