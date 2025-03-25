using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(FEHandle.ExecutionOrder)]
public class FEHandle : MonoBehaviour
{
	private const int ExecutionOrder = Grabbable.ExecutionOrder + 10;

	public FEHose hose;
	public ParticleSystem powder;

	private Grabbable _grabbable;
	private InputActionProperty _fireAction;

	public void Start()
	{
		_grabbable = GetComponent<Grabbable>();
		_fireAction = _grabbable.currentGrabber.controllerButtonClick;
	}

	public void Update()
	{
		if (!hose.grabbable.currentGrabber) return;

		if (_fireAction.action.ReadValue<float>() > 0)
		{
			powder.Play();
		}
	}
}