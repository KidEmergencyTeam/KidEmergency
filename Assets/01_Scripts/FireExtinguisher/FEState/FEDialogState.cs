public class FEDialogState : FEState
{
	public override void EnterState(FEScene scene)
	{
		JSWDialogManager.Instance.DialogStart();
	}

	public override void ExecuteState(FEScene scene)
	{
		if (!UIManager.Instance.dialogUI.dialogPanel.activeSelf)
		{
			scene.ChangeState(scene.dialogueData[scene.currentDialogIndex].nextState);
		}
	}

	public override void ExitState(FEScene scene)
	{
		scene.currentDialogIndex++;
		scene.currentDialog = scene.dialogueData[scene.currentDialogIndex].dialogs;
	}
}