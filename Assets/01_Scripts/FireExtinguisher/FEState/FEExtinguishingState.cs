using UnityEngine;

public class FEExtinguishingState : FEState
{
	private FEHandle handle;

	public override void EnterState(FEScene scene)
	{
		handle = scene.handle.GetComponent<FEHandle>();
		JSWDialogManager.Instance.delay = 1f;
	}

	public override void ExecuteState(FEScene scene)
	{
		if (handle.fireAction.action.ReadValue<float>() == 0 || handle.currentFire == FirePosition.None)
		{
			JSWDialogManager.Instance.delay = 5f;
			scene.currentDialogIndex = 11;
			scene.ChangeState(FEStateType.FEDialog);
		}
		else if (scene.handle.GetComponent<FEHandle>().currentExtinguishingDamage < 4f)
		{
			Debug.Log($"빗자루 CurrentDamage : {handle.GetComponent<FEHandle>().currentExtinguishingDamage}");
			JSWDialogManager.Instance.delay = 5f;
			scene.currentDialogIndex = 12;
			scene.ChangeState(FEStateType.FEDialog);
		}
		else if (!scene.fire.is30 && scene.fire.fireHp <= 30f)
		{
			Debug.Log($"Fire HP : {scene.fire.fireHp}");
			scene.fire.is30 = true;
			JSWDialogManager.Instance.delay = 5f;
			scene.currentDialogIndex = 13;
			scene.ChangeState(FEStateType.FEDialog);
		}
		else
		{
			JSWDialogManager.Instance.delay = 5f;
			scene.currentDialogIndex = Random.Range(9, 11);
			scene.ChangeState(FEStateType.FEDialog);
		}
	}

	public override void ExitState(FEScene scene)
	{
	}
}