using UnityEngine;

public enum FirePosition
{
	Left,
	Middle,
	Right,
	None
}

public class FireDetectCollider : MonoBehaviour
{
	public FirePosition firePosition;
}