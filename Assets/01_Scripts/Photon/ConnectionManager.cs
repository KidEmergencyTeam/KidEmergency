using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionManager : MonoBehaviour, INetworkRunnerCallbacks
{
	[SerializeField] private NetworkPrefabRef _playerPrefab;
	[SerializeField] private Transform _spawnTransform;
	
	private HardwareRig _hardwareRig;
	private NetworkRunner _networkRunner;
	private Dictionary<PlayerRef, NetworkObject> _spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();

	private void Start()
	{
		_networkRunner = gameObject.AddComponent<NetworkRunner>();
		_networkRunner.ProvideInput = true;
		
		_hardwareRig = FindObjectOfType<HardwareRig>();
		
		StartGame(GameMode.AutoHostOrClient);
	}

	private void StartGame(GameMode mode)
	{
		var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
		var sceneInfo = new NetworkSceneInfo();
		if (scene.IsValid)
		{
			sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
		}

		_networkRunner.StartGame(new StartGameArgs()
		{
			GameMode = mode,
			SessionName = "TestRoom",
			Scene = scene,
			SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
		});
	}
	
	#region 사용하는 INetworkRunnerCallbacks
	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		if (runner.IsServer)
		{
			NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, _spawnTransform.position, Quaternion.identity, player);
			_spawnedPlayers.Add(player, networkPlayerObject);
		}
	}

	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		if (_spawnedPlayers.TryGetValue(player, out NetworkObject networkPlayerObject))
		{
			runner.Despawn(networkPlayerObject);
			_spawnedPlayers.Remove(player);
		}
	}

	#endregion

	#region 사용하지 않는 INetworkRunnerCallbacks
	
	public void OnInput(NetworkRunner runner, NetworkInput input) { }
	
	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
	}

	public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
	{
	}

	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
	}

	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
	}

	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
	}

	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
	{
	}

	public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
	{
	}
	
	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
	}

	public void OnConnectedToServer(NetworkRunner runner)
	{
	}

	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
	}

	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
	}

	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
	}

	public void OnSceneLoadDone(NetworkRunner runner)
	{
	}

	public void OnSceneLoadStart(NetworkRunner runner)
	{
	}
	#endregion
}