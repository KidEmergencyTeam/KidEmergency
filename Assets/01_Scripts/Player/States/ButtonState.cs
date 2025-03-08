using UnityEngine;

public class ButtonState : State
{
	private int _loop = 0;

	public override void Enter(PlayerController player)
	{
		Debug.Log("Entered button state");
	}

	public override void Execute(PlayerController player)
	{
		_loop++;
		Debug.Log("Button state...");
		if (_loop > 100)
		{
			player.ChangeStateToNone();
		}
	}

	public override void Exit(PlayerController player)
	{
		Debug.Log("Exited button state");
	}
}