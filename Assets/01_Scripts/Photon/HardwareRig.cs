using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
	public HardwareHeadset headset;
	public HardwareHand leftController;
	public HardwareHand rightController;

	public enum RunnerExpectations
	{
		NoRunner,
		PresetRunner,
		DetectRunner
	}
	public RunnerExpectations runnerExpectations = RunnerExpectations.DetectRunner;
	
	private NetworkRunner _runner;
	
	bool searchingForRunner  = false;

	private void Start()
	{
		_runner = FindObjectOfType<NetworkRunner>();
		_runner.AddCallbacks(this);
	}

	public async Task<NetworkRunner> FindRunner()
	{
		while(searchingForRunner) await Task.Delay(10);
		searchingForRunner = true;
		if (_runner == null && runnerExpectations != RunnerExpectations.NoRunner)
		{
			if (runnerExpectations == RunnerExpectations.PresetRunner ||
			    NetworkProjectConfig.Global.PeerMode ==
			    NetworkProjectConfig.PeerModes.Multiple)
			{
				Debug.LogWarning("인스펙터에 러너 설정 안 됨");
			}
			else
			{
				_runner = FindObjectOfType<NetworkRunner>(true);
				var searchStart = Time.time;
				while (searchingForRunner && _runner == null)
				{
					if (NetworkRunner.Instances.Count > 0)
					{
						_runner = NetworkRunner.Instances[0]; 
					}

					if (_runner == null)
					{
						await Task.Delay(10);
					}
				}
			}
		}
		searchingForRunner = false;
		return _runner;
	}

	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
		var rigInput = new RigInput();
		rigInput.headPosition = headset.transform.position;
		rigInput.headRotation = headset.transform.rotation;
		rigInput.leftHandPosition = leftController.transform.position;
		rigInput.leftHandRotation = leftController.transform.rotation;
		rigInput.rightHandPosition = rightController.transform.position;
		rigInput.rightHandRotation = rightController.transform.rotation;

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