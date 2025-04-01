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
	public bool setObjectOffset = false; //오브젝트 오프셋 맞추는 용도
	public XRRayInteractor rayInteractor;

    // ture일 경우 레이 스위치 불가 -> 잡은 상태
    // false일 경우 레이 스위치 가능 -> 놓은 상태
    [Header("OnGrab 호출 여부")]
    public bool isOnGrabCalled = false;

    // OnGrab 메서드 호출 시 실행되는 이벤트 -> 이벤트 실행 시 -> RayController2.cs에서 우측 레이로 전환
    public event Action OnGrabEvent;

    [HideInInspector] public Grabbable currentGrabbedObject;
	[HideInInspector] public InputActionProperty controllerButtonClick;

	private HandAnimation _handAnimation;
	private TargetFollower _targetFollower;
	private SphereCollider _detectCollider;
	private List<Transform> _originalHandTargetTransforms = new List<Transform>();

	public bool Grabbed => currentGrabbedObject;

	private void Start()
	{
		_detectCollider = GetComponent<SphereCollider>();
		_detectCollider.isTrigger = true;
		_detectCollider.radius = detectRadius;

		_handAnimation = FindObjectOfType<HandAnimation>();
		if (isLeft) controllerButtonClick = _handAnimation.leftGrip;
		else controllerButtonClick = _handAnimation.rightGrip;

		_targetFollower = FindObjectOfType<TargetFollower>();
		_originalHandTargetTransforms.Add(_targetFollower.followTargets[2].target);
		_originalHandTargetTransforms.Add(_targetFollower.followTargets[3].target);
	}

	private void Update()
	{
		if (!setObjectOffset) return;
		if (!Grabbed) return;
		if (currentGrabbedObject.isMoving) return;
		if (isLeft)
		{
			_targetFollower.followTargets[2].posOffset =
				currentGrabbedObject.grabPosOffset;
			_targetFollower.followTargets[2].rotOffset =
				currentGrabbedObject.grabRotOffset;
		}
		else
		{
			_targetFollower.followTargets[3].posOffset =
				currentGrabbedObject.grabPosOffset;
			_targetFollower.followTargets[3].rotOffset =
				currentGrabbedObject.grabRotOffset;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		print(other.name);
		if (Grabbed) return;
		if (!other.TryGetComponent<Grabbable>(out Grabbable grabbable)) return;
		if (grabbable.isGrabbable && grabbable.isLeft == isLeft)
		{
			grabbable.outlinable.OutlineParameters.Color = Color.green;
		}

		if (controllerButtonClick.action.ReadValue<float>() > 0 && grabbable.isGrabbable && grabbable.isLeft == isLeft)
		{
			OnGrab(grabbable);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!other.TryGetComponent<Grabbable>(out Grabbable grabbable)) return;
		if (grabbable.isGrabbable && grabbable.isLeft == isLeft)
		{
			grabbable.outlinable.OutlineParameters.Color = Color.yellow;
		}
	}

	// 물체 잡기
	public void OnGrab(Grabbable grabbable)
	{
		// null 체크 후 초기화
		if (_handAnimation == null)
		{
			_handAnimation = FindObjectOfType<HandAnimation>();
		}

		if (_targetFollower == null)
		{
			_targetFollower = FindObjectOfType<TargetFollower>();
		}

		print("OnGrab");
		rayInteractor.enabled = false;
		grabbable.rb.useGravity = false;
		grabbable.rb.isKinematic = true;
		grabbable.isGrabbable = false;
		if(isLeft) _handAnimation.isLeftGrabbed = true;
		else _handAnimation.isRightGrabbed = true;
		currentGrabbedObject = grabbable;
		if (isLeft) _handAnimation.animator.SetFloat("Left Trigger", 1);
		else _handAnimation.animator.SetFloat("Right Trigger", 1);
		currentGrabbedObject.isGrabbable = false;
		currentGrabbedObject.currentGrabber = this;

		if (!currentGrabbedObject.isMoving)
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

        // OnGrab 메서드 실행 -> 이벤트가 발생 -> 다른 스크립트에서 OnGrab 메서드가 실행할 때 개별적인 처리가 가능
        OnGrabEvent?.Invoke();
        Debug.Log("[Grabber] OnGrab 이벤트 발생");

		// 레이 전환 불가
        isOnGrabCalled = true;
    }

    // 물체 놓기 -> 손 상태 복원
    public void OnRelease()
	{
		print("OnRelease");
		rayInteractor.enabled = true;
		if(isLeft) _handAnimation.isLeftGrabbed = false;
		else _handAnimation.isRightGrabbed = false;
		if (currentGrabbedObject.isMoving)
		{
			currentGrabbedObject.currentGrabber = null;
		}
		else
		{
			if (isLeft)
			{
				_targetFollower.followTargets[2].target =
					_originalHandTargetTransforms[0];
			}
			else
				_targetFollower.followTargets[3].target =
					_originalHandTargetTransforms[1];
		}

		currentGrabbedObject = null;

        // 레이 전환 가능
        isOnGrabCalled = false;
    }
}