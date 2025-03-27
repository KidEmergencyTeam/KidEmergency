public abstract class FEState
{
	public abstract void EnterState(FEScene scene);
	public abstract void ExecuteState(FEScene scene);
	public abstract void ExitState(FEScene scene);
}