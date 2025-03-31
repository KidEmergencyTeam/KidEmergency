public class FEBodyState : FEState
{
	public override void EnterState(FEScene scene)
	{
		scene.body.isGrabbable = true;
	}

	public override void ExecuteState(FEScene scene)
	{
		if (scene.body.IsGrabbed) scene.ChangeState(FEStateType.FEDialog);
	}

	public override void ExitState(FEScene scene)
	{
	}
}