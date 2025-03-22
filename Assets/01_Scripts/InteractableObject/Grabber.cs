using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SphereCollider))]
public class Grabber : MonoBehaviour
{
	public HandAnimation handAnimation;
	public InputActionProperty controllerButtonClick;
	public Grabbable currentGrabbedObject;
	public bool isLeft = true;
	public TargetFollower targetFollower;
	public List<Transform> originalHandTargetTransforms;

	public bool Grabbed => currentGrabbedObject;

	private void Start()
	{
		targetFollower = FindObjectOfType<TargetFollower>();
		originalHandTargetTransforms.Add(targetFollower.followTargets[2].target);
		originalHandTargetTransforms.Add(targetFollower.followTargets[3].target);
	}

	private void Update()
	{
		/*오브젝트 Offset 맞추는 용도
		if (!Grabbed) return;
		if (currentGrabbedObject.isMoving) return;
		if (isLeft)
		{
			targetFollower.followTargets[2].posOffset = currentGrabbedObject.posOffset;
			targetFollower.followTargets[2].rotOffset = currentGrabbedObject.rotOffset;
		}
		else
		{
			targetFollower.followTargets[3].posOffset = currentGrabbedObject.posOffset;
			targetFollower.followTargets[3].rotOffset = currentGrabbedObject.rotOffset;
		}*/
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.TryGetComponent<Grabbable>(out Grabbable grabbable)) return;

		if (grabbable.isGrabbable)
		{
			grabbable.outlinable.OutlineParameters.Color = Color.green;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		print(other.name);
		if (Grabbed) return;
		if (!other.TryGetComponent<Grabbable>(out Grabbable grabbable)) return;

		if (controllerButtonClick.action.ReadValue<float>() > 0 &&
		    grabbable.isGrabbable &&
		    grabbable.isLeft == isLeft)
		{
			OnGrab(grabbable);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!other.TryGetComponent<Grabbable>(out Grabbable grabbable)) return;

		if (grabbable.isGrabbable)
		{
			grabbable.outlinable.OutlineParameters.Color = Color.yellow;
		}
	}

	public void OnGrab(Grabbable grabbable)
	{
		print("OnGrab");
		grabbable.isGrabbable = false;
		handAnimation.enabled = false;
		currentGrabbedObject = grabbable;
		if (isLeft) handAnimation.animator.SetFloat("Left Trigger", 1);
		else handAnimation.animator.SetFloat("Right Trigger", 1);

		if (currentGrabbedObject.isMoving)
		{
			currentGrabbedObject.isGrabbable = false;
			currentGrabbedObject.currentGrabber = this;
		}

		else
		{
			if (isLeft)
			{
				targetFollower.followTargets[2].target = currentGrabbedObject.transform;
				targetFollower.followTargets[2].posOffset =
					currentGrabbedObject.posOffset;
				targetFollower.followTargets[2].rotOffset =
					currentGrabbedObject.rotOffset;
			}
			else
			{
				targetFollower.followTargets[3].target = currentGrabbedObject.transform;
				targetFollower.followTargets[3].posOffset =
					currentGrabbedObject.posOffset;
				targetFollower.followTargets[3].rotOffset =
					currentGrabbedObject.rotOffset;
			}
		}
	}

	public void OnRelease()
	{
		print("OnRelease");
		handAnimation.enabled = true;
		if (currentGrabbedObject.isMoving)
		{
			currentGrabbedObject.isGrabbable = true;
			currentGrabbedObject.currentGrabber = null;
		}
		else
		{
			if (isLeft)
			{
				targetFollower.followTargets[2].target = originalHandTargetTransforms[0];
			}
			else targetFollower.followTargets[3].target = originalHandTargetTransforms[1];
		}

		currentGrabbedObject = null;
	}
}