using Fusion;
using UnityEngine;


[DefaultExecutionOrder(NetworkRig.ExecutionOrder)]
public class NetworkRig : NetworkBehaviour
{
	public const int ExecutionOrder = 100;

	public HardwareRig hardwareRig;
	public NetworkHand leftHand;
	public NetworkHand rightHand;
	public NetworkHeadset headset;

	public bool IsLocalNetworkRig => Object.HasInputAuthority;

	[HideInInspector] public NetworkTransform networkTransform;

	private void Awake()
	{
		networkTransform = GetComponent<NetworkTransform>();
	}

	private void Start()
	{
		ConnectionManager.instance.networkRig = this;
	}

	public override void Spawned()
	{
		base.Spawned();
		if (IsLocalNetworkRig)
		{
			hardwareRig = FindObjectOfType<HardwareRig>();
			hardwareRig.networkRig = this;
		}
	}

	public override void FixedUpdateNetwork()
	{
		base.FixedUpdateNetwork();

		if (GetInput<RigInput>(out var input))
		{
			transform.position = input.headPosition - new Vector3(0, 0.5f, 0.1f);
			transform.rotation = input.playAreaRotation;

			leftHand.transform.position = input.leftHandPosition;
			leftHand.transform.rotation = input.leftHandRotation;
			rightHand.transform.position = input.rightHandPosition;
			rightHand.transform.rotation = input.rightHandRotation;
			headset.transform.position = input.headPosition;
			headset.transform.rotation = input.headRotation;
		}
	}

	public override void Render()
	{
		base.Render();
		if (IsLocalNetworkRig)
		{
			transform.position = hardwareRig.headset.transform.position -
			                     new Vector3(0, 0.5f, 0.1f);
			transform.rotation = hardwareRig.transform.rotation;
			leftHand.transform.position = hardwareRig.leftController.transform.position;
			leftHand.transform.rotation = hardwareRig.leftController.transform.rotation;
			rightHand.transform.position = hardwareRig.rightController.transform.position;
			rightHand.transform.rotation = hardwareRig.rightController.transform.rotation;
			headset.transform.position = hardwareRig.headset.transform.position;
			headset.transform.rotation = hardwareRig.headset.transform.rotation;
		}
	}
}