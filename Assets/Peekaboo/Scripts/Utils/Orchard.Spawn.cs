using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Netick;
using Netick.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets; // 需要引入Addressables
using System;
using CockleBurs.GamePlay;
using UnityEngine.ResourceManagement.AsyncOperations;

public partial class Orchard
{
    [SerializeReference] private List<NetworkObject> instantiatedObjects = new List<NetworkObject>();
    private static Dictionary<uint, GameplaySettings.PoolConfig> _poolConfigs;
    private Dictionary<uint, GameObject> _prefabCache = new Dictionary<uint, GameObject>();
    private Dictionary<uint, GameObjectPool> _gameObjectPools = new Dictionary<uint, GameObjectPool>();
    private Dictionary<GameObject, uint> _instanceToPoolMap = new Dictionary<GameObject, uint>();
    private static void InitializePoolConfigs()
    {
        if (_poolConfigs != null) return;
        
        _poolConfigs = new Dictionary<uint, GameplaySettings.PoolConfig>();
        var poolSettings = Game.GameplaySettings.PoolSettings;

        foreach (var config in poolSettings.ObjectPools)
        {
            var poolConfig = config;
            if (!poolConfig.active) continue;
            
            // 使用FNV哈希算法生成唯一ID
            uint hashId = FNVHash(poolConfig.Prefab.AssetGUID);
            poolConfig.hashId = hashId;
            
            // 检查哈希冲突
            if (_poolConfigs.ContainsKey(hashId))
            {
                Debug.LogError($"PoolConfig hash冲突! GUID: {poolConfig.Prefab.AssetGUID}");
                continue;
            }
            
            _poolConfigs.Add(hashId, poolConfig);
        }
    }
    private void InitializeGameObjectPools()
    {
        foreach (var kvp in _poolConfigs)
        {
            uint hashId = kvp.Key;
            GameplaySettings.PoolConfig config = kvp.Value;
            
            // 跳过网络对象和不激活的配置
            if (!config.active || config.isNetworkObject) continue;
            
            // 确保每个配置只初始化一次
            if (_gameObjectPools.ContainsKey(hashId)) continue;
            
            GameObject prefab = GetOrLoadPrefab(config);
            if (prefab == null) continue;
            
            int poolSize = config.usePoolSize ? config.poolSize : 0;
            _gameObjectPools[hashId] = new GameObjectPool(prefab, poolSize);
        }
    }
    // FNV-1a 32位哈希算法
    private static uint FNVHash(string input)
    {
        const uint fnvPrime = 0x811C9DC5;
        uint hash = 0;
        
        foreach (char c in input)
        {
            hash ^= c;
            hash *= fnvPrime;
        }
        return hash;
    }

    // 核心生成方法（根据哈希ID）
    public Actor Spawn(
        uint poolHash,
        Vector3? position = null,
        Quaternion? rotation = null,
        NetworkPlayer inputSource = null)
    {
        if (!_poolConfigs.TryGetValue(poolHash, out GameplaySettings.PoolConfig config))
        {
            Debug.LogError($"找不到对象池配置: {poolHash}");
            return null;
        }

        GameObject prefab = GetOrLoadPrefab(config);
        if (prefab == null)
        {
            Debug.LogError($"预制体加载失败: {poolHash}");
            return null;
        }

        // 使用不同的生成方式
        return config.isNetworkObject 
            ? NetworkSpawn(prefab, position, rotation, inputSource)
            : DefaultSpawn(poolHash, position, rotation);
    }
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Actor NetworkSpawn(GameObject go, 
                            Vector3? position = null, 
                            Quaternion? rotation = null, 
                            NetworkPlayer inputSource = null)
    {
        var networkObject = SpawnInternal(go, 
                                         position ?? Vector3.zero, 
                                         rotation ?? Quaternion.identity, 
                                         inputSource);
        return SetReplicatedObject(networkObject);
    }

    public Actor SetReplicatedObject(NetworkObject networkObject)
    {
        if (networkObject is Actor actor)
            return actor;
        else
            throw new System.InvalidCastException($"{networkObject.name} is not an Actor.");
    }

    public int GetInputSourcePlayerId(NetworkPlayer networkPlayer)
    {
        return networkPlayer.PlayerId == 0 ? -2 : networkPlayer.PlayerId - 1;
    }

    // ========== Spawn Core Methods ========== //
    
    public virtual NetworkObject Spawn(GameObject go, 
                                     Vector3 position = default, 
                                     Quaternion rotation = default, 
                                     NetworkConnection ownerConnection = null, 
                                     Scene targetScene = default)
    {
        return SpawnInternal(go, position, rotation, ownerConnection, targetScene);
    }

    public virtual NetworkObject Spawn(NetworkObject go, 
                                     Vector3 position = default, 
                                     Quaternion rotation = default, 
                                     NetworkConnection ownerConnection = null, 
                                     Scene targetScene = default)
    {
        return SpawnInternal(go.gameObject, position, rotation, ownerConnection, targetScene);
    }

    public virtual NetworkObject Spawn(GameObject go, 
                                     Vector3 position = default, 
                                     Quaternion rotation = default, 
                                     NetworkConnection ownerConnection = null)
    {
        return Spawn(go, position, rotation, ownerConnection, default);
    }

    public virtual NetworkObject Spawn(NetworkObject go, 
                                     Vector3 position = default, 
                                     Quaternion rotation = default, 
                                     NetworkConnection ownerConnection = null)
    {
        return Spawn(go, position, rotation, ownerConnection, default);
    }
    private NetworkObject SpawnInternal(UnityEngine.Object prefab, 
        Vector3 position, 
        Quaternion rotation, 
        object connection,
        Scene? targetScene = null)
    {
        if (prefab == null)
            throw new System.ArgumentNullException("Prefab cannot be null");

        position = position == default ? Vector3.zero : position;
        rotation = rotation == default ? Quaternion.identity : rotation;

        NetworkObject networkObject;
        
        if (prefab is GameObject gameObj)
        {
            networkObject = connection switch
            {
                NetworkConnection conn => NetworkInstantiate(gameObj, position, rotation, conn),
                NetworkPlayer player => NetworkInstantiate(gameObj, position, rotation, player),
                null => NetworkInstantiate(gameObj, position, rotation),
                _ => throw new System.ArgumentException("Invalid connection type")
            };
        }
        else
        {
            throw new System.ArgumentException("Invalid prefab type");
        }

        if (targetScene.HasValue && targetScene.Value.IsValid() && targetScene.Value.isLoaded)
        {
            SceneManager.MoveGameObjectToScene(networkObject.gameObject, targetScene.Value);
        }

        instantiatedObjects.Add(networkObject);
        return networkObject;
    }
    private void DestroyAllTrackedObjects()
    {
        foreach (var obj in GetAllInstantiatedObjects())
        {
            if (obj != null) Destroy(obj);
        }
        instantiatedObjects.Clear();
    }

    private void DestroySingleObject(NetworkObject obj)
    {
        if (obj == null) return;
        
        Destroy(obj);
        instantiatedObjects.Remove(obj);
    }
    public List<NetworkObject> GetAllInstantiatedObjects() => new List<NetworkObject>(instantiatedObjects);
    // 获取或加载预制体
    private GameObject GetOrLoadPrefab(GameplaySettings.PoolConfig config)
    {
        uint hashId = config.hashId;
        
        // 从缓存获取
        if (_prefabCache.TryGetValue(hashId, out GameObject prefab))
            return prefab;
        
        // 同步加载Addressables预制体
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(config.Prefab);
        handle.WaitForCompletion(); // 阻塞直到加载完成
        
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            prefab = handle.Result;
            _prefabCache[hashId] = prefab;
            return prefab;
        }
        
        Debug.LogError($"预制体加载失败: {hashId}");
        return null;
    }

    // 默认生成方式（非网络对象）
    private Actor DefaultSpawn(
        uint poolHash, 
        Vector3? position, 
        Quaternion? rotation)
    {
        if (!_gameObjectPools.TryGetValue(poolHash, out GameObjectPool pool))
        {
            Debug.LogError($"对象池未初始化: {poolHash}");
            return null;
        }
        
        Vector3 pos = position ?? Vector3.zero;
        Quaternion rot = rotation ?? Quaternion.identity;
        
        GameObject instance = pool.GetPooledObject(pos, rot);
        if (instance == null)
        {
            Debug.LogError($"对象池获取失败: {poolHash}");
            return null;
        }
        
        // 记录对象到池的映射关系
        _instanceToPoolMap[instance] = poolHash;
        
        return instance.GetComponent<Actor>();
    }
    public bool IsNetworkObject(GameObject obj)
    {
        if (obj.TryGetComponent(out NetworkObject netObj))
        {
            return Objects.ContainsKey(netObj.Id);
        }
        return false;
    }
    
    public bool IsPooledObject(GameObject obj) => _instanceToPoolMap.ContainsKey(obj);
    
    public void Destroy(GameObject obj)
    {
        if (obj == null) return;
    
        if (IsNetworkObject(obj))
        {
           Destroy(obj);
        }
        else if (IsPooledObject(obj))
        {
            ReturnToPool(obj);
        }
        else
        {
            Destroy(obj);
        }
    }
    public void ReturnToPool(GameObject instance)
    {
        if (instance == null) return;
        
        // 尝试获取对象所在的池
        if (!_instanceToPoolMap.TryGetValue(instance, out uint poolHash))
        {
            Debug.LogWarning($"对象不是池化对象: {instance.name}", instance);
            Destroy(instance);
            return;
        }
        
        if (!_gameObjectPools.TryGetValue(poolHash, out var pool))
        {
             Debug.LogWarning($"找不到对象池: {poolHash}", instance);
           Destroy(instance);
            return;
        }
        
        pool.ReturnToPool(instance);
        _instanceToPoolMap.Remove(instance);
    }

    // 初始化方法（在游戏启动时调用）
  //  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializePoolSystem()
    {
        InitializePoolConfigs();
    }
}
