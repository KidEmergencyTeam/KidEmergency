using UnityEngine;

public class NoneState : State
{
	public override void Enter(PlayerController player)
	{
		Debug.Log("Entered none state");
	}

	public override void Execute(PlayerController player)
	{
		Debug.Log("None state...");
	}

	public override void Exit(PlayerController player)
	{
		Debug.Log("Exited none state");
	}
}