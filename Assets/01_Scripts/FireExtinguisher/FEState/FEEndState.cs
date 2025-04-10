using UnityEngine;
using UnityEngine.SceneManagement;

public class FEEndState : FEState
{
	private float startTime;
	private float delay = 1f;

	public override void EnterState(FEScene scene)
	{
		startTime = Time.time;
	}

	public override void ExecuteState(FEScene scene)
	{
		if (startTime + delay < Time.time) scene.FadeOut();
		if (startTime + delay + OVRScreenFade.Instance.fadeTime < Time.time) SceneManager.LoadScene(0);
	}

	public override void ExitState(FEScene scene)
	{
	}
}