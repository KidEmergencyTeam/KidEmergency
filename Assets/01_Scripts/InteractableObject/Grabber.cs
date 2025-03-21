using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SphereCollider))]
public class Grabber : MonoBehaviour
{
	public Animator handAnimator;
	public HandAnimation handAnimation;
	public InputActionProperty interactor;
	public Grabbable currentGrabbable;
	public bool isLeft = true;
	public TargetFollower targetFollower;
	public List<Transform> originalTarget;

	public bool Grabbed => currentGrabbable;

	private void Start()
	{
		targetFollower = FindObjectOfType<TargetFollower>();
		originalTarget.Add(targetFollower.followTargets[2].target);
		originalTarget.Add(targetFollower.followTargets[3].target);
	}

	private void Update()
	{
		if (!Grabbed) return;
		if (currentGrabbable.isMoving) return;
		if (isLeft)
		{
			targetFollower.followTargets[2].posOffset = currentGrabbable.posOffset;
			targetFollower.followTargets[2].rotOffset = currentGrabbable.rotOffset;
		}
		else
		{
			targetFollower.followTargets[3].posOffset = currentGrabbable.posOffset;
			targetFollower.followTargets[3].rotOffset = currentGrabbable.rotOffset;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		print(other.name);
		if (Grabbed) return;
		other.TryGetComponent<Grabbable>(out Grabbable grabbable);
		if (interactor.action.ReadValue<float>() > 0 && grabbable.isGrabbable &&
		    grabbable.isLeft == isLeft)
		{
			OnGrab(grabbable);
		}
	}

	public void OnGrab(Grabbable grabbable)
	{
		print("OnGrab");
		currentGrabbable = grabbable;
		handAnimation.enabled = false;
		if (isLeft) handAnimation.animator.SetFloat("Left Trigger", 1);
		else handAnimation.animator.SetFloat("Right Trigger", 1);
		if (currentGrabbable.isMoving)
		{
			currentGrabbable.isGrabbable = false;
			currentGrabbable.currentGrabber = this;
		}
		else
		{
			if (isLeft)
			{
				targetFollower.followTargets[2].target = currentGrabbable.transform;
				targetFollower.followTargets[2].posOffset = currentGrabbable.posOffset;
				targetFollower.followTargets[2].rotOffset = currentGrabbable.rotOffset;
			}
			else
			{
				targetFollower.followTargets[3].target = currentGrabbable.transform;
				targetFollower.followTargets[3].posOffset = currentGrabbable.posOffset;
				targetFollower.followTargets[3].rotOffset = currentGrabbable.rotOffset;
			}
		}
	}

	public void OnRelease()
	{
		print("OnRelease");
		if (currentGrabbable.isMoving)
		{
			currentGrabbable.isGrabbable = true;
			currentGrabbable.currentGrabber = null;
		}
		else
		{
			if (isLeft)
			{
				targetFollower.followTargets[2].target = originalTarget[0];
			}
			else targetFollower.followTargets[3].target = originalTarget[1];
		}

		currentGrabbable = null;
	}
}