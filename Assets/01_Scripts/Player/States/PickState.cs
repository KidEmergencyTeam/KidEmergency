using UnityEngine;

public class PickState : State
{
	public override void Enter(PlayerController player)
	{
		Debug.Log("Entered pick state");
	}

	public override void Execute(PlayerController player)
	{
		Debug.Log("Pick state...");
	}

	public override void Exit(PlayerController player)
	{
		Debug.Log("Exited pick state");
	}
}