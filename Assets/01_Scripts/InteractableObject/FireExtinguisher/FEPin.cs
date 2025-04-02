public class FEPin : Grabbable
{
	public float destroyDistance = 1f;
	public float originalXPosition;

	protected override void Start()
	{
		base.Start();
		originalXPosition = transform.localPosition.y;
	}

	protected override void Update()
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
					currentGrabber.transform.position + grabPosOffset;
				if (realMovingObject.transform.localPosition.y >= originalXPosition + destroyDistance)
				{
					FEScene.Instance.ChangeState(FEStateType.FEDialog);
				}
			}
		}
	}
}