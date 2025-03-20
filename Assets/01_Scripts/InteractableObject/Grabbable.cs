using UnityEngine;

public class Grabbable : MonoBehaviour
{
	public bool isGrabbable = true;
	public Vector3 grabOffset;
	public Quaternion grabRotation;
	public Grabber currentGrabber;
	public bool IsGrabbed => currentGrabber;
	public bool isLeft = true;


	public void Update()
	{
		if (IsGrabbed)
		{
			transform.position += grabOffset;
			transform.rotation *= grabRotation;
		}
	}
}