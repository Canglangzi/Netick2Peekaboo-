using System;
using System.Collections.Generic;
using System.Threading.Tasks;
//using CocKleBurs.Core;
using Cysharp.Threading.Tasks;
using Netick.Unity;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class NetickPrefabRegistry 
{
    public static string prefabLabel = "NetworkPrefab"; // 在Addressables里给网络预制体加这个标签
    private static List<NetworkObject> loadedPrefabs = new List<NetworkObject>();
    private static void LoadPrefabsAndStartClient()
    {
         Addressables.LoadAssetsAsync<GameObject>(prefabLabel, null).Completed += OnPrefabsLoaded;
    }

    public static async UniTask<NetickConfig> LoadPrefabsAndGetConfigAsync()
    {
        loadedPrefabs = new List<NetworkObject>();

        var handle = Addressables.LoadAssetsAsync<NetworkObject>(prefabLabel, null);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var prefab in handle.Result)
            {
                loadedPrefabs.Add(prefab);
                Debug.Log($"Loaded network prefab: {prefab.name}");
            }

            var config = Network.CloneDefaultConfig();
            config.Prefabs = loadedPrefabs;
        
            Game.NotifyConfigLoaded(config);
            return config;
        }
        else
        {
            Debug.LogError("Failed to load Addressable network prefabs.");
            return null;
        }
    }
    private static void OnPrefabsLoaded(AsyncOperationHandle<IList<GameObject>> handle)
    {
       var networkObjects = new List<NetworkObject>();
        int successCount = 0;
        int errorCount = 0;

        try
        {
            // 确保加载结果不为空
            if (handle.Result == null)
            {
                Debug.LogError("加载结果为空！没有加载任何网络预制体。");
                return;
            }

            foreach (var prefab in handle.Result)
            {
                try
                {
                    // 检查预制体是否为空
                    if (prefab == null)
                    {
                        Debug.LogWarning("跳过空预制体引用");
                        continue;
                    }

                    // 安全尝试获取NetworkObject组件
                    if (prefab.TryGetComponent<NetworkObject>(out var netObj))
                    {
                        networkObjects.Add(netObj);
                        successCount++;
                        Debug.Log($"✅ 成功加载网络对象: {prefab.name} (ID: {netObj.Id})");
                    }
                    else
                    {
                        errorCount++;
                        Debug.LogError($"⚠️ 预制体 {prefab.name} 缺少NetworkObject组件！请确保这是一个有效的Netick网络预制体。");
                    }
                }
                catch (NullReferenceException ex)
                {
                    errorCount++;
                    Debug.LogError($"处理预制体时遇到空引用错误: {ex.Message}");
                }
                catch (MissingReferenceException ex)
                {
                    errorCount++;
                    Debug.LogError($"处理预制体时遇到缺失引用: {ex.Message}");
                }
                catch (Exception ex)
                {
                    errorCount++;
                    Debug.LogError($"处理预制体 {prefab?.name ?? "未知"} 时发生意外错误: {ex.Message}");
                    
                    // 附加更多调试信息
                    if (prefab != null)
                    {
                        Debug.Log($"预制体路径: {PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefab)}", prefab);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"遍历加载结果时发生严重错误: {ex.Message}");
        }

        // 最终统计报告
        Debug.Log($"加载网络预制体完成: 成功 {successCount}, 失败 {errorCount}, 总计 {handle.Result.Count}");
        
        if (networkObjects.Count == 0)
        {
            Debug.LogError($"未加载任何有效网络预制体！尝试加载: {handle.Result.Count} 个对象，但无有效NetworkObject组件");
            return;
        }

        // 配置Netick网络
        var config = Network.CloneDefaultConfig();
        config.Prefabs = networkObjects;
        
        // 通知系统配置已完成
        Game.NotifyConfigLoaded(config);
        
         UniTask.Yield(); // 确保配置传播
    }
}
