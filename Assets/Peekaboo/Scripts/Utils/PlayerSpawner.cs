using UnityEngine;
using System.Collections.Generic;
using Netick;
using Netick.Unity;

[RequireComponent(typeof(Actor))]
public class PlayerSpawner : NetworkCherry
{
    private Actor TestPlayerPrefab => base.GameplaySettings.GameConfig.TestPlayerPrefab;
    [SerializeField] private Actor PlayerPrefab => base.GameplaySettings.GameConfig.PlayerPrefab;

    private List<Transform> _spawnPoints; // 使用列表存储出生点

    public override void NetworkStart()
    {
        FindSpawnPointsByComponent(); // 通过组件查找出生点
        InitializeObjectPools();
        base.Orchard.Events.OnPlayerConnected += OnPlayerConnected;
    }

    // 通过组件查找出生点
    [ContextMenu("Find Spawn Points by Component")]
    private void FindSpawnPointsByComponent()
    {
        _spawnPoints = new List<Transform>();
        
        // 查找所有挂载了PlayerStart组件的对象
        PlayerStart[] playerStartComponents = FindObjectsOfType<PlayerStart>();
        
        foreach (PlayerStart start in playerStartComponents)
        {
            _spawnPoints.Add(start.transform);
        }

        // 若没有找到任何出生点，创建默认位置
        if (_spawnPoints.Count == 0)
        {
            Debug.LogWarning("No PlayerStart components found! Creating fallback spawn.");
            CreateFallbackSpawnPoint();
        }
    }

    private void CreateFallbackSpawnPoint()
    {
        GameObject fallback = new GameObject("FallbackSpawn");
        fallback.transform.position = Vector3.zero;
        fallback.transform.rotation = Quaternion.identity;
        fallback.AddComponent<PlayerStart>(); // 添加PlayerStart组件
        _spawnPoints = new List<Transform> { fallback.transform };
    }

    public void InitializeObjectPools()
    {
        if (PlayerPrefab == null) return;

        if (GameplaySettings.GameConfig.PoolActive)
        {
            Sandbox.InitializePool(
                GameplaySettings.GameConfig.EnableTestMode 
                    ? TestPlayerPrefab.gameObject 
                    : PlayerPrefab.gameObject,
                Game.GameplaySettings.GameConfig.PlayerPoolSize
            );
        }
    }

    private void OnPlayerConnected(NetworkSandbox sandbox, NetworkPlayer player)
    {   
        SpawnPlayer(sandbox, player);
    }
    
    public void OnDisconnectedFromServer(NetworkSandbox sandbox, NetworkConnection server, TransportDisconnectReason reason)
    {
        if (Network.StartMode == StartMode.Client)
        {
            Network.Shutdown();
        }
    }

    public virtual void SpawnPlayer(NetworkSandbox sandbox, NetworkPlayer client)
    {
        if (PlayerPrefab == null) return;

        // 确保出生点列表已初始化
        if (_spawnPoints == null || _spawnPoints.Count == 0)
            FindSpawnPointsByComponent();

        // 随机选择出生点
        Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Count)];
        Vector3 spawnPos = spawnPoint.position + Vector3.left * (1 + sandbox.ConnectedPlayers.Count);
        Quaternion spawnRot = spawnPoint.rotation;

        // 根据测试模式选择预制体
        Actor playerPrefab = GameplaySettings.GameConfig.EnableTestMode 
            ? TestPlayerPrefab 
            : PlayerPrefab;

        // 生成玩家对象
        Actor playerObject = base.Orchard.NetworkSpawn(
            playerPrefab.gameObject, 
            spawnPos, 
            spawnRot, 
            client
        );

        // 设置玩家对象引用
        client.PlayerObject = playerObject.GetBehaviour<NetworkCherry>();

        // 触发生成事件
        Orchard.onPlayerSpawned.Invoke(playerObject, client);
    }
}