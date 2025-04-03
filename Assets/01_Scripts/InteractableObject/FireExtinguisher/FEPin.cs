using UnityEngine;

public class FEPin : Grabbable
{
	public float destroyDistance = 1f;
	public float originalXPosition;
	public GameObject pinWire;

	protected override void Start()
	{
		base.Start();
		originalXPosition = transform.localPosition.y;
	}

	protected override void Update()
	{
		if (isGrabbable)
		{
			highlight.SetActive(true);
			if (isTrigger)
			{
				highlighter.SetColor(Color.green);
				highlighter.isBlinking = false;
			}
			else
			{
				highlighter.SetColor(Color.yellow);
				highlighter.isBlinking = true;
			}
		}
		else
		{
			highlight.SetActive(false);

			if (!IsGrabbed) return;

			if (isMoving)
			{
				realMovingObject.transform.position =
					currentGrabber.transform.position + grabPosOffset;
				if (realMovingObject.transform.localPosition.y >= originalXPosition + destroyDistance)
				{
					FEScene.Instance.ChangeState(FEStateType.FEDialog);
					Destroy(pinWire);
				}
			}
		}
	}
}