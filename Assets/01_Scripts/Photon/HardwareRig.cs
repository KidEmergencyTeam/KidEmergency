using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;

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

public class HardwareRig : MonoBehaviour, INetworkRunnerCallbacks
{
	[Header("VR References")]
	public Transform headset;
	public Transform leftController;
	public Transform rightController;
    
	private NetworkRunner _runner;

	private void Start()
	{
		_runner = FindObjectOfType<NetworkRunner>();
	}

	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
		var rigInput = new RigInput();
		rigInput.headPosition = headset.position;
		rigInput.headRotation = headset.rotation;
		rigInput.leftHandPosition = leftController.position;
		rigInput.leftHandRotation = leftController.rotation;
		rigInput.rightHandPosition = rightController.position;
		rigInput.rightHandRotation = rightController.rotation;

		input.Set(rigInput);
	}
	#region Unuse Callbacks
	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
	public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress,
		NetConnectFailedReason reason) { }
	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key,
		ArraySegment<byte> data) { }
	public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key,
		float progress) { }
	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
	public void OnConnectedToServer(NetworkRunner runner) { }
	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
	public void OnSceneLoadDone(NetworkRunner runner) { }
	public void OnSceneLoadStart(NetworkRunner runner) { }
	#endregion
}