using UnityEngine;
using Fusion;

public enum RigPart
{
	LeftController,
	RightController
}

public struct RigInput : INetworkInput
{
	public Vector3 playAreaPosition;
	public Quaternion playAreaRotation;
	public Vector3 headPosition;
	public Quaternion headRotation;
	public Vector3 leftHandPosition;
	public Quaternion leftHandRotation;
	public Vector3 rightHandPosition;
	public Quaternion rightHandRotation;
}

public class HardwareRig : MonoBehaviour
{
	[Header("VR References")]
	public Transform headset;
	public Transform leftController;
	public Transform rightController;
    
	private NetworkRunner _runner;
	private Player _Player;

	private void Start()
	{
		_runner = FindObjectOfType<NetworkRunner>();
	}

	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
		var rigInput = new RigInput
		{
			headPosition = headset.position,
			headRotation = headset.rotation,
			leftHandPosition = leftController.position,
			leftHandRotation = leftController.rotation,
			rightHandPosition = rightController.position,
			rightHandRotation = rightController.rotation
		};

		input.Set(rigInput);
	}
}