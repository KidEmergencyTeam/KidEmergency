public class FEFireStartState : FEState
{
	public override void EnterState(FEScene scene)
	{
		scene.handle.GetComponent<FEHandle>().enabled = true;
		scene.fire.gameObject.SetActive(true);
		scene.ChangeState(FEStateType.FEDialog);
	}

	public override void ExecuteState(FEScene scene)
	{
	}

	public override void ExitState(FEScene scene)
	{
	}
}