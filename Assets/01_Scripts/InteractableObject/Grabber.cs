using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SphereCollider))]
public class Grabber : MonoBehaviour
{
	public bool isLeft = true;
	public float detectRadius = 0.05f;
	public bool setObjectOffset = false; //오브젝트 오프셋 맞추는 용도

	[HideInInspector] public Grabbable currentGrabbedObject;

	private HandAnimation _handAnimation;
	private InputActionProperty _controllerButtonClick;
	private TargetFollower _targetFollower;
	private SphereCollider _detectCollider;
	private List<Transform> _originalHandTargetTransforms = new List<Transform>();

	public bool Grabbed => currentGrabbedObject;

	private void Start()
	{
		_detectCollider = GetComponent<SphereCollider>();
		_detectCollider.radius = detectRadius;

		_handAnimation = FindObjectOfType<HandAnimation>();
		if (isLeft) _controllerButtonClick = _handAnimation.leftGrip;
		else _controllerButtonClick = _handAnimation.rightGrip;

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
		if (_controllerButtonClick.action.ReadValue<float>() > 0 &&
		    grabbable.isGrabbable &&
		    grabbable.isLeft == isLeft)
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
		print("OnGrab");
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
				_targetFollower.followTargets[2].target = currentGrabbedObject.transform;
				_targetFollower.followTargets[2].posOffset =
					currentGrabbedObject.grabPosOffset;
				_targetFollower.followTargets[2].rotOffset =
					currentGrabbedObject.grabRotOffset;
			}
			else
			{
				_targetFollower.followTargets[3].target = currentGrabbedObject.transform;
				_targetFollower.followTargets[3].posOffset =
					currentGrabbedObject.grabPosOffset;
				_targetFollower.followTargets[3].rotOffset =
					currentGrabbedObject.grabRotOffset;
			}
		}
	}

	public void OnRelease()
	{
		print("OnRelease");
		_handAnimation.enabled = true;
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
	}
}