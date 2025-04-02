public class FEPinState : FEState
{
	public override void EnterState(FEScene scene)
	{
		scene.pin.isGrabbable = true;
	}

	public override void ExecuteState(FEScene scene)
	{
	}

	public override void ExitState(FEScene scene)
	{
		scene.body.currentGrabber.OnRelease();
		scene.pin.currentGrabber.OnRelease();
		scene.pin.gameObject.SetActive(false);
	}
}