public class HandAnimation2 : HandAnimation
{
	protected override void Update()
	{
		if (!isLeftGrabbed)
		{
			float leftGripValue = leftGrip.action.ReadValue<float>();
			animator.SetFloat("Left Grip", leftGripValue);
		}

		if (!isRightGrabbed)
		{
			float rightGripValue = rightGrip.action.ReadValue<float>();
			animator.SetFloat("Right Grip", rightGripValue);
		}
	}
}