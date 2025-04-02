using UnityEngine;

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
			highlight.SetActive(true);
			if (_isTrigger)
			{
				_highlightMaterial.color = new Color(0, 255, 0, 150);
			}
			else
			{
				_highlightMaterial.color = new Color(255, 255, 0, _currentAlpha);
				if (_alphaUp)
				{
					_currentAlpha = Mathf.MoveTowards(_currentAlpha, 150, Time.deltaTime * 0.5f);
				}
				else
				{
					_currentAlpha = Mathf.MoveTowards(_currentAlpha, 0, Time.deltaTime * 0.5f);
				}
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
				}
			}
		}
	}
}