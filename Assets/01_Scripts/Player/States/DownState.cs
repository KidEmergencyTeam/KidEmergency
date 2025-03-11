using UnityEngine;

public class DownState : State
{
	private int _loop = 0;

	public override void Enter(PlayerController player)
	{
		Debug.Log("Entered Down state");
	}

	public override void Execute(PlayerController player)
	{
		_loop++;
		Debug.Log("Down state...");
		if (_loop > 100)
		{
			player.ChangeStateToNone();
		}
	}

	public override void Exit(PlayerController player)
	{
		Debug.Log("Exited Down state");
	}
}