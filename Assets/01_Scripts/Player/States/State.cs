public abstract class State
{
	public abstract void Enter(PlayerController player);
	public abstract void Execute(PlayerController player);
	public abstract void Exit(PlayerController player);
}