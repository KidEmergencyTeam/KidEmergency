using UnityEngine;

public class WalkState : State
{
	public override void Enter(PlayerController player)
	{
		Debug.Log("Entered Walk state");
	}

	public override void Execute(PlayerController player)
	{
		Debug.Log("Walk state...");
	}

	public override void Exit(PlayerController player)
	{
		Debug.Log("Exited Walk state");
	}
}