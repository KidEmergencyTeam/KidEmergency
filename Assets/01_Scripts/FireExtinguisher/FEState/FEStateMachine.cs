using System.Collections.Generic;

public class FEStateMachine
{
	public FEScene owner;
	public FEState currentState;

	public FEStateMachine(FEScene entity)
	{
		owner = entity;
	}

	public void ChangeState(FEState newState)
	{
		currentState?.ExitState(owner);
		currentState = newState;
		currentState.EnterState(owner);
	}
}