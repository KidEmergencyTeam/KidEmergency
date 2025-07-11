using UnityEngine;

public class Highlighter : MonoBehaviour
{
	public bool isBlinking = false; //true일 때 깜빡깜빡

	private Vector4 _currentColor;
	private Material _material;
	private float _currentAlpha;
	private bool _alphaUp = false;

	private void Start()
	{
		_material = GetComponent<Renderer>().material;
		_currentAlpha = 0.7f;
	}

	private void Update()
	{
		Blink();
	}

	public void SetColor(Color color)
	{
		_currentColor = color;
	}

	private void Blink()
	{
		if (isBlinking)
		{
			_material.color = new Color(_currentColor.x, _currentColor.y, _currentColor.z, _currentAlpha);
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
		else _material.color = new Color(_currentColor.x, _currentColor.y, _currentColor.z, 0.7f);
	}
}