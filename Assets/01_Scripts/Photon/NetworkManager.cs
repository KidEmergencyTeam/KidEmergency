using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
	[SerializeField] private NetworkPrefabRef _playerPrefab;
	[SerializeField] private Transform spawnTransform;
	
	private NetworkRunner _networkRunner;
	private Dictionary<PlayerRef, NetworkObject> _spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();

	private void Start()
	{
		if (FindObjectsOfType<NetworkRunner>().Length == 0)
		{
			StartGame(GameMode.Host);
		}
		else
		{
			StartGame(GameMode.Client);
		}
	}

	async void StartGame(GameMode mode)
	{
		_networkRunner = gameObject.AddComponent<NetworkRunner>();
		_networkRunner.ProvideInput = true;
		
		var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
		var sceneInfo = new NetworkSceneInfo();
		if (scene.IsValid)
		{
			sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
		}

		await _networkRunner.StartGame(new StartGameArgs()
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
			NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnTransform.position, Quaternion.identity, player);
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
	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
		throw new NotImplementedException();
	}

	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
		throw new NotImplementedException();
	}

	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
		throw new NotImplementedException();
	}

	public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
	{
		throw new NotImplementedException();
	}

	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
		throw new NotImplementedException();
	}

	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
		throw new NotImplementedException();
	}

	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
		throw new NotImplementedException();
	}

	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
	{
		throw new NotImplementedException();
	}

	public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
	{
		throw new NotImplementedException();
	}

	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
		var data = new NetworkInputData();
		
		if(Input.GetKey(KeyCode.W))
			data.direction += Vector3.forward;
		if(Input.GetKey(KeyCode.S))
			data.direction += Vector3.back;
		if(Input.GetKey(KeyCode.A))
			data.direction += Vector3.left;
		if (Input.GetKey(KeyCode.D))
			data.direction += Vector3.right;

		input.Set(data);
	}

	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
		throw new NotImplementedException();
	}

	public void OnConnectedToServer(NetworkRunner runner)
	{
		throw new NotImplementedException();
	}

	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
		throw new NotImplementedException();
	}

	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
		throw new NotImplementedException();
	}

	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
		throw new NotImplementedException();
	}

	public void OnSceneLoadDone(NetworkRunner runner)
	{
		throw new NotImplementedException();
	}

	public void OnSceneLoadStart(NetworkRunner runner)
	{
		throw new NotImplementedException();
	}
	#endregion
}