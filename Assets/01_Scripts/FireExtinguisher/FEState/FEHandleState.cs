public class FEHandleState : FEState
{
	public override void EnterState(FEScene scene)
	{
		scene.handle.isGrabbable = true;
	}

	public override void ExecuteState(FEScene scene)
	{
		if (scene.handle.IsGrabbed) scene.ChangeState(FEStateType.FEDialog);
	}

	public override void ExitState(FEScene scene)
	{
	}
}