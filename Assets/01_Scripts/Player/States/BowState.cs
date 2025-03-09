using UnityEngine;

public class BowState : State
{
	public override void Enter(PlayerController player)
	{
		Debug.Log("Entered Bow state");
	}

	public override void Execute(PlayerController player)
	{
		Debug.Log("Bow state...");
	}

	public override void Exit(PlayerController player)
	{
		Debug.Log("Exited Bow state");
	}
}