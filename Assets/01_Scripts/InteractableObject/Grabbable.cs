using UnityEngine;

public class Grabbable : MonoBehaviour
{
	public bool isGrabbable = false;
	public Vector3 posOffset;
	public Vector3 rotOffset;
	public Grabber currentGrabber;
	public GameObject realMovingObject;
	public bool isLeft = true;
	public bool isMoving = true;

	public bool IsGrabbed => currentGrabber;

	public void Update()
	{
		if (!IsGrabbed) return;

		if (isMoving)
		{
			realMovingObject.transform.position =
				currentGrabber.transform.position + posOffset;
			realMovingObject.transform.rotation =
				currentGrabber.transform.rotation * Quaternion.Euler(rotOffset);
		}
	}
}