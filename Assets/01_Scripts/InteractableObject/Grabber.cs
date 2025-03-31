using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

[RequireComponent(typeof(SphereCollider))]
public class Grabber : MonoBehaviour
{
	public bool isLeft = true;
	public float detectRadius = 0.05f;
	public bool setObjectOffset = false; //오브젝트 오프셋 맞추는 용도

    //public XRRayInteractor rayInteractor;
    
	// ture일 경우 레이 스위치 불가 -> 잡은 상태
	// false일 경우 레이 스위치 가능 -> 놓은 상태
    [Header("OnGrab 호출 여부")]
    public bool isOnGrabCalled = false;

    // OnGrab 메서드 호출 시 실행되는 이벤트 -> 이벤트 실행 시 -> RayController2.cs에서 레이 우측으로 고정
    public delegate void GrabEvent(Grabbable grabbable);
    public event GrabEvent OnGrabEvent;

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

	private void OnTriggerEnter(Collider other)
	{
		if (!other.TryGetComponent<Grabbable>(out Grabbable grabbable)) return;
		if (grabbable.isGrabbable && grabbable.isLeft == isLeft)
		{
			grabbable.outlinable.OutlineParameters.Color = Color.green;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		print(other.name);
		if (Grabbed) return;
		if (!other.TryGetComponent<Grabbable>(out Grabbable grabbable)) return;
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
		// rayInteractor.enabled = false;
		grabbable.rb.useGravity = false;
		grabbable.rb.isKinematic = true;
		grabbable.isGrabbable = false;
		_handAnimation.enabled = false;
		currentGrabbedObject = grabbable;
		if (isLeft) _handAnimation.animator.SetFloat("Left Trigger", 1);
		else _handAnimation.animator.SetFloat("Right Trigger", 1);

		if (currentGrabbedObject.isMoving)
		{
			currentGrabbedObject.isGrabbable = false;
			currentGrabbedObject.currentGrabber = this;
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

        Debug.Log("[Grabber] OnGrab 이벤트 발생");
        isOnGrabCalled = true;
        OnGrabEvent?.Invoke(grabbable);
    }

    // 물체 놓기
    public void OnRelease()
    {
        print("OnRelease");

        //if (currentGrabbedObject == null) return; -> 단순하게 GrabStatePersistence.cs에서 OnDestroy 호출될 때 손수건 오브젝트 제거됨에 따라 OnRelease 호출,
		//따라서 null 처리하면 중간에 멈출수 있기 때문에 주석 처리

        // 손수건 오브젝트 제거되면 -> 손가락 펴지고 -> 우측 레이 활성화(우측 레이 활성화가 초기값)
        //rayInteractor.enabled = true;

        currentGrabbedObject.rb.useGravity = true;
        currentGrabbedObject.rb.isKinematic = false;
        currentGrabbedObject.isGrabbable = true;
        currentGrabbedObject.currentGrabber = null;

        _handAnimation.enabled = true;

        if (isLeft)
        {
            _handAnimation.animator.SetFloat("Left Trigger", 0);
            _targetFollower.followTargets[2].target = _originalHandTargetTransforms[0];
            _targetFollower.followTargets[2].posOffset = Vector3.zero;
            _targetFollower.followTargets[2].rotOffset = Vector3.zero;
        }
        else
        {
            _handAnimation.animator.SetFloat("Right Trigger", 0);
            _targetFollower.followTargets[3].target = _originalHandTargetTransforms[1];
            _targetFollower.followTargets[3].posOffset = Vector3.zero;
            _targetFollower.followTargets[3].rotOffset = Vector3.zero;
        }

		// 레이 전환 가능
        isOnGrabCalled = false;
        //currentGrabbedObject = null; -> GrabStatePersistence.cs에서 OnDestroy 호출될 때 손수건 오브젝트 제거됨에 따라 null 처리 주석 처리
    }
}

//private void OnRelease()
//{
//    print("OnRelease");
//    rayInteractor.enabled = true;
//    _handAnimation.enabled = true;
//    if (currentGrabbedObject.isMoving)
//    {
//        currentGrabbedObject.currentGrabber = null;
//    }
//    else
//    {
//        if (isLeft)
//        {
//            _targetFollower.followTargets[2].target =
//                _originalHandTargetTransforms[0];
//        }
//        else
//            _targetFollower.followTargets[3].target =
//                _originalHandTargetTransforms[1];
//    }

//    currentGrabbedObject = null;
//}