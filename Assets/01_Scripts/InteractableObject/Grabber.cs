using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(SphereCollider))]
public class Grabber : MonoBehaviour
{
	public InputActionProperty interactor;
	public Grabbable currentGrabbable;
	public bool isLeft = true;
	public bool Grabbed => currentGrabbable;

	private void OnTriggerStay(Collider other)
	{
		if (Grabbed) return;
		print(other.name);
		other.TryGetComponent<Grabbable>(out Grabbable grabbable);
		if (interactor.action.ReadValue<float>() > 0 && grabbable.isGrabbable &&
		    grabbable.isLeft == isLeft)
		{
			OnGrab(grabbable);
		}
	}

	public void OnGrab(Grabbable grabbable)
	{
		currentGrabbable = grabbable;
		currentGrabbable.isGrabbable = false;
		currentGrabbable.currentGrabber = this;
	}

	public void OnRelease()
	{
		currentGrabbable.isGrabbable = true;
		currentGrabbable.currentGrabber = null;
		currentGrabbable = null;
	}
}