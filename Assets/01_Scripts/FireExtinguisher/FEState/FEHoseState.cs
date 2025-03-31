public class FEHoseState : FEState
{
	public override void EnterState(FEScene scene)
	{
		scene.hose.isGrabbable = true;
	}

	public override void ExecuteState(FEScene scene)
	{
		if (scene.hose.IsGrabbed) scene.ChangeState(FEStateType.FEDialog);
	}

	public override void ExitState(FEScene scene)
	{
	}
}