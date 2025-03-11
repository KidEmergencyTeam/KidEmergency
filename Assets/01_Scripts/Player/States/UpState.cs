using UnityEngine;

public class UpState : State
{
	private int _loop = 0;

	public override void Enter(PlayerController player)
	{
		Debug.Log("Entered Up state");
	}

	public override void Execute(PlayerController player)
	{
		_loop++;
		Debug.Log("Up state...");
		if (_loop > 100)
		{
			player.ChangeStateToNone();
		}
	}

	public override void Exit(PlayerController player)
	{
		Debug.Log("Exited Up state");
	}
}