using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimation2 : HandAnimation
{
	void Update()
	{
		if (!isLeftGrabbed)
		{
			float leftGripValue = leftGrip.action.ReadValue<float>();
			animator.SetFloat("Left Trigger", leftGripValue);
		}

		if (!isRightGrabbed)
		{
			float rightGripValue = rightGrip.action.ReadValue<float>();
			animator.SetFloat("Right Trigger", rightGripValue);
		}
	}
}