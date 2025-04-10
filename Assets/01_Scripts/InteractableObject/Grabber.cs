using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(SphereCollider))]
public class Grabber : MonoBehaviour
{
	public bool isLeft = true;
	public float detectRadius = 0.05f;
	public XRRayInteractor rayInteractor;

	// ture일 경우 레이 스위치 불가 -> 잡은 상태
	// false일 경우 레이 스위치 가능 -> 놓은 상태
	[Header("OnGrab 호출 여부")] public bool isOnGrabCalled = false;

	// OnGrab 메서드 호출 시 실행 -> 실행되었음을 전달 -> RayController2.cs에서 우측 레이로 전환
	public event Action OnGrabEvent;

	[HideInInspector] public Grabbable currentGrabbedObject;
	[HideInInspector] public InputActionProperty controllerButtonClick;

	private SphereCollider _detectCollider;
	private HandAnimation _handAnimation;
	private TargetFollower _targetFollower;
	private List<Transform> _originalHandTargetTransforms = new List<Transform>();
	private List<Vector3> _originalHandTargetPosOffset = new List<Vector3>();
	private List<Vector3> _originalHandTargetRotOffset = new List<Vector3>();

	public bool Grabbed => currentGrabbedObject;

	private void Start()
	{
		_detectCollider = GetComponent<SphereCollider>();
		_detectCollider.isTrigger = true;
		_detectCollider.radius = detectRadius;


		_handAnimation = FindObjectOfType<HandAnimation2>();
		if (_handAnimation == null) _handAnimation = FindObjectOfType<HandAnimation>();
		if (isLeft)
		{
			controllerButtonClick = _handAnimation.leftGrip;
		}
		else
		{
			controllerButtonClick = _handAnimation.rightGrip;
		}

		_targetFollower = FindObjectOfType<TargetFollower>();
		_originalHandTargetTransforms.Add(_targetFollower.followTargets[2].target);
		_originalHandTargetPosOffset.Add(_targetFollower.followTargets[2].posOffset);
		_originalHandTargetRotOffset.Add(_targetFollower.followTargets[2].rotOffset);
		_originalHandTargetTransforms.Add(_targetFollower.followTargets[3].target);
		_originalHandTargetPosOffset.Add(_targetFollower.followTargets[3].posOffset);
		_originalHandTargetRotOffset.Add(_targetFollower.followTargets[3].rotOffset);
	}

	private void OnTriggerStay(Collider other)
	{
		print(other.name);
		if (Grabbed) return;
		if (!other.TryGetComponent<Grabbable>(out Grabbable grabbable)) return;

		if (controllerButtonClick.action.ReadValue<float>() >= 1 &&
		    grabbable.isGrabbable && grabbable.isLeft == isLeft)
		{
			OnGrab(grabbable);
		}
	}

	public void OnGrab(Grabbable grabbable)
	{
		print("OnGrab");
		rayInteractor.enabled = false;
		grabbable.rb.useGravity = false;
		grabbable.rb.isKinematic = true;
		grabbable.isGrabbable = false;
		if (isLeft)
		{
			_handAnimation.isLeftGrabbed = true;
		}
		else
		{
			_handAnimation.isRightGrabbed = true;
		}

		currentGrabbedObject = grabbable;
		currentGrabbedObject.isGrabbable = false;
		currentGrabbedObject.currentGrabber = this;
		if (currentGrabbedObject.isMoving)
		{
			if (isLeft)
			{
				_handAnimation.animator.SetFloat("Left Grip", 1);
			}

			else
			{
				_handAnimation.animator.SetFloat("Right Grip", 1);
			}
		}
		else
		{
			if (isLeft)
			{
				_targetFollower.followTargets[2].target =
					currentGrabbedObject.transform;
				_targetFollower.followTargets[2].posOffset =
					currentGrabbedObject.grabPosOffset;
				_targetFollower.followTargets[2].rotOffset =
					currentGrabbedObject.grabRotOffset;
			}
			else
			{
				_targetFollower.followTargets[3].target =
					currentGrabbedObject.transform;
				_targetFollower.followTargets[3].posOffset =
					currentGrabbedObject.grabPosOffset;
				_targetFollower.followTargets[3].rotOffset =
					currentGrabbedObject.grabRotOffset;
			}
		}

		OnGrabEvent?.Invoke();
		Debug.Log("[Grabber] OnGrab 이벤트 발생");

		// 레이 전환 불가
		isOnGrabCalled = true;
	}

	public void OnRelease()
	{
		print("OnRelease");
		rayInteractor.enabled = true;
		if (isLeft)
		{
			_handAnimation.isLeftGrabbed = false;
		}
		else
		{
			_handAnimation.isRightGrabbed = false;
		}

		if (currentGrabbedObject.isMoving)
		{
			currentGrabbedObject.currentGrabber = null;
		}
		else
		{
			if (isLeft)
			{
				_targetFollower.followTargets[2].target = _originalHandTargetTransforms[0];
				_targetFollower.followTargets[2].posOffset = _originalHandTargetPosOffset[0];
				_targetFollower.followTargets[2].rotOffset = _originalHandTargetRotOffset[0];
			}
			else
			{
				_targetFollower.followTargets[3].target = _originalHandTargetTransforms[1];
				_targetFollower.followTargets[3].posOffset = _originalHandTargetPosOffset[1];
				_targetFollower.followTargets[3].rotOffset = _originalHandTargetRotOffset[1];
			}
		}

		currentGrabbedObject = null;

		// 레이 전환 가능
		isOnGrabCalled = false;
	}
}