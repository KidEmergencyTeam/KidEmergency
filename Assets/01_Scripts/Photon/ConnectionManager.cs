using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    private byte[] _connectionToken;
    private Dictionary<int, NetworkObject> _playersMap = new Dictionary<int, NetworkObject>();
    private Dictionary<PlayerRef, NetworkObject> _spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();
    private List<int> _pendingTokens = new List<int>();
    private System.Diagnostics.Stopwatch _watch = new System.Diagnostics.Stopwatch();
    private const float _cleanupTime = 5000f; // 5초

    private void Start()
    {
        _connectionToken = ConnectionTokenUtils.NewToken();
        StartGame(GameMode.AutoHostOrClient);
    }

    private async void StartGame(GameMode mode, HostMigrationToken hostMigrationToken = null)
    {
        if (_networkRunner == null)
        {
            _networkRunner = gameObject.AddComponent<NetworkRunner>();
            _networkRunner.ProvideInput = true;
            _networkRunner.AddCallbacks(this);
        }

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
    
        await _networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<SceneNOSolver>(), 
            ConnectionToken = _connectionToken,
            HostMigrationToken = hostMigrationToken,
            HostMigrationResume = hostMigrationToken != null ? HostMigrationResumeMethod : null
        });
    }

    private void Update()
    {
        if (_networkRunner == null) return;

        // 재접속 대기 시간 초과 체크
        if (_networkRunner.IsServer && _watch.IsRunning && _watch.ElapsedMilliseconds > _cleanupTime)
        {
            _watch.Stop();
            _watch.Reset();

            lock (_pendingTokens)
            {
                foreach (var token in _pendingTokens.ToList())
                {
                    if (_playersMap.TryGetValue(token, out var playerObject))
                    {
                        Debug.Log($"시간 초과로 플레이어 제거: {token}");
                        _networkRunner.Despawn(playerObject);
                        _playersMap.Remove(token);
                    }
                }
                _pendingTokens.Clear();
            }

            if (_networkRunner.IsServer)
            {
                _networkRunner.PushHostMigrationSnapshot();
            }
        }
    }

    private void HostMigrationResumeMethod(NetworkRunner runner)
    {
        Debug.Log("호스트 마이그레이션: 게임 상태 복원 시작");

        foreach (var resumeNO in runner.GetResumeSnapshotNetworkObjects())
        {
            if (resumeNO.TryGetBehaviour<NetworkTransform>(out var netTransform))
            {
                var newNetworkObject = runner.Spawn(resumeNO,
                    position: netTransform.transform.position,
                    rotation: netTransform.transform.rotation,
                    onBeforeSpawned: (innerRunner, networkObject) =>
                    {
                        networkObject.CopyStateFrom(resumeNO);
                    });

                if (newNetworkObject.TryGetComponent<Player>(out var player))
                {
                    var token = player.Token;
                    if (_playersMap.TryGetValue(token, out var oldPlayer))
                    {
                        runner.Despawn(oldPlayer);
                    }

                    _playersMap[token] = newNetworkObject;
                    lock (_pendingTokens)
                    {
                        _pendingTokens.Add(token);
                    }
                }
            }
        }

        _watch.Restart();
        Debug.Log("호스트 마이그레이션: 게임 상태 복원 완료");
    }
   
    private int GetPlayerToken(NetworkRunner runner, PlayerRef player)
    {
        if (runner.LocalPlayer == player)
        {
            return ConnectionTokenUtils.HashToken(_connectionToken);
        }

        var token = runner.GetPlayerConnectionToken(player);
        if (token != null)
        {
            return ConnectionTokenUtils.HashToken(token);
        }

        throw new InvalidOperationException("플레이어 토큰을 찾을 수 없습니다.");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        _playersMap.Clear();
        _spawnedPlayers.Clear();
        lock (_pendingTokens)
        {
            _pendingTokens.Clear();
        }

        if (Application.isPlaying && shutdownReason != ShutdownReason.HostMigration)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    #region Callbacks
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer) return;

        var playerToken = GetPlayerToken(runner, player);
        
        // 재접속한 플레이어 처리
        if (_playersMap.TryGetValue(playerToken, out var existingPlayer))
        {
            Debug.Log($"재접속한 플레이어: {playerToken}");
            existingPlayer.AssignInputAuthority(player);
            _spawnedPlayers[player] = existingPlayer;
            
            lock (_pendingTokens)
            {
                if (_pendingTokens.Contains(playerToken))
                {
                    _pendingTokens.Remove(playerToken);
                }
            }
        }
        else
        {
            Debug.Log($"새로운 플레이어: {playerToken}");
            NetworkObject playerObject = runner.Spawn(_playerPrefab, 
                _spawnTransform.position,
                Quaternion.identity,
                player,
                (r, obj) => obj.GetComponent<Player>().Token = playerToken
            );
            
            _playersMap[playerToken] = playerObject;
            _spawnedPlayers[player] = playerObject;
        }
        
        if (runner.IsServer)
        {
            runner.PushHostMigrationSnapshot();
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedPlayers.TryGetValue(player, out NetworkObject networkPlayerObject))
        {
            if (runner.IsServer)
            {
                var token = networkPlayerObject.GetComponent<Player>().Token;
                _playersMap.Remove(token);
                runner.Despawn(networkPlayerObject);
                runner.PushHostMigrationSnapshot();
            }
            _spawnedPlayers.Remove(player);
        }
    } 
    
    public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("호스트 마이그레이션 시작");
        
        await runner.Shutdown(shutdownReason: ShutdownReason.HostMigration);
        
        var completedLoad = new TaskCompletionSource<bool>();
        var sceneAsync = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        sceneAsync.completed += (op) => completedLoad.SetResult(true);
        await completedLoad.Task;
        
        StartGame(hostMigrationToken.GameMode, hostMigrationToken);
    }
    #endregion

    #region Unuse Callbacks
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress,
        NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key,
        ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key,
        float progress) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    #endregion
}