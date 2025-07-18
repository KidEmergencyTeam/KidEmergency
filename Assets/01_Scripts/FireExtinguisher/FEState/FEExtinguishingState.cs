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
			scene.currentDialogIndex = 12;
			scene.ChangeState(FEStateType.FEDialog);
		}
		else if (handle.currentExtinguishingDamage < 2f)
		{
			Debug.Log($"빗자루 CurrentDamage : {handle.currentExtinguishingDamage}");
			scene.currentDialogIndex = 13;
			scene.ChangeState(FEStateType.FEDialog);
		}
		else if (!scene.fire.is30 && scene.fire.fireHp <= 30f)
		{
			Debug.Log($"Fire HP : {scene.fire.fireHp}");
			scene.fire.is30 = true;
			scene.currentDialogIndex = 14;
			scene.ChangeState(FEStateType.FEDialog);
		}
		else
		{
			JSWDialogManager.Instance.delay = 5f;
			scene.currentDialogIndex = Random.Range(10, 12);
			scene.ChangeState(FEStateType.FEDialog);
		}
	}

	public override void ExitState(FEScene scene)
	{
	}
}