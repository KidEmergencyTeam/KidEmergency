using UnityEngine;

public class HoldState : State
{
	public override void Enter(PlayerController player)
	{
		Debug.Log("Entered hold state");
	}

	public override void Execute(PlayerController player)
	{
		Debug.Log("Hold state...");
	}

	public override void Exit(PlayerController player)
	{
		Debug.Log("Exited hold state");
	}
}