using Fusion;
using UnityEngine;

[DefaultExecutionOrder(NetworkHand.ExecutionOrder)]
public class NetworkHand : NetworkBehaviour
{
	public const int ExecutionOrder = NetworkRig.ExecutionOrder + 10;
	public NetworkTransform networkTransform;

	public RigPart side;
	private NetworkRig _rig;

	public bool IsLocalNetworkRig => _rig.IsLocalNetworkRig;

	private void Awake()
	{
		_rig = GetComponentInParent<NetworkRig>();
		networkTransform = GetComponent<NetworkTransform>();
	}
}