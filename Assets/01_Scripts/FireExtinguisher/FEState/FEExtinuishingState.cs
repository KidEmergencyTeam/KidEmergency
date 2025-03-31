public class FEExtinuishingState : FEState
{
	public override void EnterState(FEScene scene)
	{
	}

	public override void ExecuteState(FEScene scene)
	{
		if (scene.handle.GetComponent<FEHandle>().fireAction.action.ReadValue<float>() == 0)
		{
			scene.currentDialogIndex = 12;
			scene.ChangeState(FEStateType.FEDialog);
		}
		
	}

	public override void ExitState(FEScene scene)
	{
	}
}