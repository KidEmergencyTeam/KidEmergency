using EPOOutline;
using UnityEngine;

[RequireComponent(typeof(Outlinable))]
public class Grabbable : MonoBehaviour
{
	public bool isGrabbable = false;
	public Vector3 posOffset;
	public Vector3 rotOffset;
	public Grabber currentGrabber;
	public bool isSameMoveAndGrabbable = true; //그랩 가능한 오브젝트와 실제 움직이는 오브젝트가 다른 경우
	public GameObject realMovingObject; //실제 움직이는 오브젝트
	public bool isLeft = true; //상호작용 가능한 손의 위치
	public bool isMoving = true; //손을 따라 움직이는 Object면 true, Object에 손이 박히면 false
	public Outlinable outlinable;

	public bool IsGrabbed => currentGrabber;

	private void Start()
	{
		outlinable = GetComponent<Outlinable>();
		if (isSameMoveAndGrabbable) realMovingObject = this.gameObject;
	}

	public void Update()
	{
		if (isGrabbable)
		{
			outlinable.enabled = true;
		}
		else
		{
			outlinable.enabled = false;

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
}