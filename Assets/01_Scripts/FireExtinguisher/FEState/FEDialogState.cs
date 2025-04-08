using UnityEngine;

public class FEDialogState : FEState
{
	public override void EnterState(FEScene scene)
	{
		scene.currentDialogData = scene.dialogueData[scene.currentDialogIndex];
		scene.currentDialog = scene.currentDialogData.dialogs;
		Debug.Log($"currentDialog: {scene.currentDialog}");
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
		JSWDialogManager.Instance.startDelay = 0f;
	}
}