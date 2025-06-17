using System;
using Netick;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
[System.Serializable]
public class PlayerSpawnedEvent : UnityEvent<Actor, NetworkPlayer> { }
public partial class Orchard
{
    public PlayerSpawnedEvent onPlayerSpawned;
    public class _EventLobby
    {
        public  event Action<int> OnLobbyCreated;
        public  event Action<string> OnLobbyJoined;
        public  event Action OnLobbyLeft;
        public  event Action<string, string> OnLobbyDataSet;
        public  event Action OnLobbyListRequested;
            
        public  event Action<ulong,ulong> OnLobbyPlayerJoined;
        public  event Action<ulong, ulong,string> OnLobbyChatMessage;
        public  void NotifyLobbyPlayerJoined(ulong lobbyId,ulong playerID ) => OnLobbyPlayerJoined?.Invoke(lobbyId , playerID);

        public void NotifyLobbyCreated(int maxPlayers) => OnLobbyCreated?.Invoke(maxPlayers);

        public void NotifyLobbyJoined(string lobbyId) => OnLobbyJoined?.Invoke(lobbyId);

        public void NotifyLobbyLeft() => OnLobbyLeft?.Invoke();

        public void NotifyLobbyDataSet(string key, string value) => OnLobbyDataSet?.Invoke(key, value);
            
        public void NotifyLobbyListRequested() => OnLobbyListRequested?.Invoke();

        public void NotifyLobbyChatMessage(ulong arg1Id,ulong arg2Id, string arg3)=> OnLobbyChatMessage?.Invoke(arg1Id, arg2Id, arg3);
            
    }
    public class PlayerEvents
    {
        // 玩家生成事件 (服务器和客户端)
        public event Action<ulong, GameObject> OnPlayerSpawned;
        
        // 玩家销毁事件 (服务器和客户端)
        public event Action<ulong> OnPlayerDespawned;
        
        // 玩家进入就绪状态 (准备开始游戏)
        public event Action<ulong> OnPlayerReady;
        
        // 玩家断开连接/离开游戏
        public event Action<ulong, string> OnPlayerLeft;
        
        // 玩家受到伤害
        public event Action<ulong, ulong, int> OnPlayerDamaged;
        
        // 玩家死亡
        public event Action<ulong, ulong> OnPlayerDied;
        
        // 玩家重生
        public event Action<ulong> OnPlayerRespawned;
        
        // 玩家分数变化
        public event Action<ulong, int, int> OnPlayerScoreChanged;
        
        // 玩家获得新物品
        public event Action<ulong, string, int> OnPlayerItemAcquired;
        
        // 通知触发事件的方法
        public void NotifyPlayerSpawned(ulong playerId, GameObject playerObj) => OnPlayerSpawned?.Invoke(playerId, playerObj);
        public void NotifyPlayerDespawned(ulong playerId) => OnPlayerDespawned?.Invoke(playerId);
        public void NotifyPlayerReady(ulong playerId) => OnPlayerReady?.Invoke(playerId);
        public void NotifyPlayerLeft(ulong playerId, string reason = "正常离开") => OnPlayerLeft?.Invoke(playerId, reason);
        public void NotifyPlayerDamaged(ulong victimId, ulong attackerId, int damage) => OnPlayerDamaged?.Invoke(victimId, attackerId, damage);
        public void NotifyPlayerDied(ulong victimId, ulong killerId) => OnPlayerDied?.Invoke(victimId, killerId);
        public void NotifyPlayerRespawned(ulong playerId) => OnPlayerRespawned?.Invoke(playerId);
        public void NotifyPlayerScoreChanged(ulong playerId, int scoreDelta, int newTotal) => OnPlayerScoreChanged?.Invoke(playerId, scoreDelta, newTotal);
        public void NotifyPlayerItemAcquired(ulong playerId, string itemId, int quantity) => OnPlayerItemAcquired?.Invoke(playerId, itemId, quantity);
    }
    public class EntityEvents
    {
        // 实体生成
        public event Action<GameObject> OnEntitySpawned;
        
        // 实体销毁
        public event Action<ulong> OnEntityDespawned;
        
        // 实体所有权转移
        public event Action<ulong, ulong> OnEntityOwnershipChanged;
        
        // 对象状态变化
        public event Action<ulong, string, object> OnObjectStateChanged;
        
        // 通知触发事件的方法
        public void NotifyEntitySpawned(GameObject entity) => OnEntitySpawned?.Invoke(entity);
        public void NotifyEntityDespawned(ulong entityId) => OnEntityDespawned?.Invoke(entityId);
        public void NotifyEntityOwnershipChanged(ulong entityId, ulong newOwnerId) => OnEntityOwnershipChanged?.Invoke(entityId, newOwnerId);
        public void NotifyObjectStateChanged(ulong objectId, string stateName, object newValue) => OnObjectStateChanged?.Invoke(objectId, stateName, newValue);
    }

    public _EventLobby EventLobby;
    
    public ulong playerid;

    public event Action OnInitializeCallbacks;

    public void NotifyInitializeCallbacks() => OnInitializeCallbacks?.Invoke();
}