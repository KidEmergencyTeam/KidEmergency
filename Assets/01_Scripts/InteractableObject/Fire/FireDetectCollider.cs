using UnityEngine;

public enum FirePosition
{
	Left,
	Middle,
	Right,
	Aim,
	None
}

public class FireDetectCollider : MonoBehaviour
{
	public FirePosition firePosition;
}