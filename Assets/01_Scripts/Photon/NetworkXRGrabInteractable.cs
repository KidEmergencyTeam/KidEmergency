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
	private NetworkRigidbody _networkRigidbody;
	private Transform _originalParent;
	private Vector3 _grabOffset;
	private Quaternion _grabRotation;

	private void Awake()
	{
		_grabInteractable = GetComponent<XRGrabInteractable>();
		_networkRigidbody = GetComponent<NetworkRigidbody>();
		_originalParent = transform.parent;

		_grabInteractable.selectEntered.AddListener(OnGrab);
		_grabInteractable.selectExited.AddListener(OnRelease);
	}

	public override void Spawned()
	{
		base.Spawned();
		// 초기 상태 설정
		IsGrabbed = false;
		GrabbedBy = PlayerRef.None;
	}

	private void OnGrab(SelectEnterEventArgs args)
	{
		if (Object.HasInputAuthority || Runner.IsSharedModeMasterClient)
		{
			Rpc_RequestGrab(Runner.LocalPlayer);
		}
	}

	private void OnRelease(SelectExitEventArgs args)
	{
		if (Object.HasInputAuthority || Runner.IsSharedModeMasterClient)
		{
			Rpc_RequestRelease(Runner.LocalPlayer);
		}
	}

	public override void FixedUpdateNetwork()
	{
		// 그랩된 상태에서 위치 동기화
		if (IsGrabbed && Object.HasInputAuthority)
		{
			// 여기서 그랩한 플레이어의 손 위치에 따라 오브젝트 위치 업데이트
			// XR 컨트롤러의 위치를 가져와서 동기화
			Transform grabberTransform = _grabInteractable.selectingInteractor.transform;
			transform.position = grabberTransform.position + grabberTransform.rotation * _grabOffset;
			transform.rotation = grabberTransform.rotation * _grabRotation;
		}
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public void Rpc_RequestGrab(PlayerRef player)
	{
		if (Object.HasStateAuthority)
		{
			Debug.Log($"Player {player} requested grab");
			
			// 이미 잡혀있는지 확인
			if (IsGrabbed)
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
			if (_networkRigidbody != null)
			{
				_networkRigidbody.Rigidbody.isKinematic = true;
			}
			
			// 그랩 오프셋 저장 (상대적 위치)
			if (_grabInteractable.selectingInteractor != null)
			{
				Transform grabberTransform = _grabInteractable.selectingInteractor.transform;
				_grabOffset = Quaternion.Inverse(grabberTransform.rotation) * (transform.position - grabberTransform.position);
				_grabRotation = Quaternion.Inverse(grabberTransform.rotation) * transform.rotation;
			}
		}
		else
		{
			// 릴리즈 시 물리 속성 복원
			if (_networkRigidbody != null)
			{
				_networkRigidbody.Rigidbody.isKinematic = false;
			}
		}
	}
}