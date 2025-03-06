using System.Collections;
using System.Collections.Generic;
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
		if (_loop > 100)
		{
			player.CompleteCurrentAction();
		}
	}

	public override void Exit(PlayerController player)
	{
		Debug.Log("Exited button state");
	}
}