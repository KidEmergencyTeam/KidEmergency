using UnityEngine;

public class PushState : State
{
	public override void Enter(PlayerController player)
	{
		Debug.Log("Entered Push state");
	}

	public override void Execute(PlayerController player)
	{
		Debug.Log("Push state...");
	}

	public override void Exit(PlayerController player)
	{
		Debug.Log("Exited Push state");
	}
}