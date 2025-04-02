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
			if (_isTrigger)
			{
				_highlightMaterial.color = new Color(0, 1, 0, 0.7f);
			}
			else
			{
				_highlightMaterial.color = new Color(1, 1, 0, _currentAlpha);
				if (_alphaUp)
				{
					_currentAlpha = Mathf.MoveTowards(_currentAlpha, 0.7f, Time.deltaTime * 0.5f);
					if (_currentAlpha >= 0.69f) _alphaUp = false;
				}
				else
				{
					_currentAlpha = Mathf.MoveTowards(_currentAlpha, 0f, Time.deltaTime * 0.5f);
					if (_currentAlpha <= 0.01f) _alphaUp = true;
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
					Destroy(pinWire);
				}
			}
		}
	}
}