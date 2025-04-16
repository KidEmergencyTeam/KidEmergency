using UnityEngine;
using UnityEngine.SceneManagement;

public class FEEndState : FEState
{
	private float _startTime;
	private float _delay = 1f;
	private bool _startFadeOut = false;

	public override void EnterState(FEScene scene)
	{
		_startTime = Time.time;
	}

	public override void ExecuteState(FEScene scene)
	{
		if (_startTime + _delay < Time.time && !_startFadeOut)
		{
			scene.FadeOut();
			_startFadeOut = true;
		}

		if (_startTime + _delay + OVRScreenFade.Instance.fadeTime < Time.time)
		{
			UIManager.Instance.dialogUI.dialogText.text = "";
			SceneManager.LoadScene(0);
		}
	}

	public override void ExitState(FEScene scene)
	{
	}
}