using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FEHose : Grabbable
{
	public ChainIKConstraint hoseIKChain;
	public Transform grabPoint;

	protected override void Update()
	{
		if (!isGrabbable)
		{
			highlight.SetActive(false);

			if (!IsGrabbed) return;

			if (isMoving)
			{
				realMovingObject.transform.position =
					grabPoint.position + grabPosOffset;
				realMovingObject.transform.rotation =
					grabPoint.rotation * Quaternion.Euler(grabRotOffset);
			}
		}
		else
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
	}
}