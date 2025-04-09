using UnityEngine.SceneManagement;

public class FEEndState : FEState
{
	public override void EnterState(FEScene scene)
	{
		SceneManager.LoadScene(0);
	}

	public override void ExecuteState(FEScene scene)
	{
	}

	public override void ExitState(FEScene scene)
	{
	}
}