using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Netick.Unity;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class NetickPrefabLoadingTask : ILoadingTask
{
    private const string PRELOAD_TASK_NAME = "加载网络预制体";
    private const float PRELOAD_WEIGHT = 10f;
    
    private AsyncOperationHandle<IList<GameObject>> _loadingHandle;
    
    public float GetWeight() => PRELOAD_WEIGHT;
    public string GetName() => PRELOAD_TASK_NAME;

    public async UniTask Load(IProgress<float> progress)
    {
        try
        {
            progress?.Report(0f);
            
            // 确保Addressables已初始化
            await InitializeAddressables();
            
            // 修复点1：使用地址替代标签加载
            string targetAddress = "NetworkPrefab"; // 替换为实际使用的地址
            Debug.Log($"尝试加载地址: {targetAddress}");
            
            // 修复点2：直接使用地址加载资源
            _loadingHandle = Addressables.LoadAssetsAsync<GameObject>(
                new List<string> { targetAddress }, // 使用地址而非标签
                null,
                Addressables.MergeMode.UseFirst // 更安全的合并模式
            );
            
            
            // 修复点3：增强的进度跟踪
            float lastProgress = 0f;
            while (!_loadingHandle.IsDone)
            {
                
                if (Math.Abs(_loadingHandle.PercentComplete - lastProgress) > 0.01f)
                {
                    lastProgress = _loadingHandle.PercentComplete;
                    progress?.Report(lastProgress * 0.9f);
                }
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            // 处理结果
            if (_loadingHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"成功加载 {_loadingHandle.Result?.Count ?? 0} 个预制体");
                await ConfigureNetick();
                progress?.Report(1f);
            }
            else
            {
                Debug.LogError($"加载失败! 状态: {_loadingHandle.Status}, 错误: {_loadingHandle.OperationException?.Message}");
                progress?.Report(1f);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"加载异常: {ex.Message}\n{ex.StackTrace}");
            progress?.Report(1f);
        }
    }

    private async UniTask InitializeAddressables()
    {
        // 确保Addressables系统已初始化
        if (!Addressables.InitializeAsync().IsDone)
        {
            await Addressables.InitializeAsync();
        }
    }

    private async UniTask ConfigureNetick()
    {
        if (_loadingHandle.Result == null || _loadingHandle.Result.Count == 0)
        {
            Debug.LogError("无有效预制体可配置");
            return;
        }

        var networkObjects = new List<NetworkObject>();
        int validCount = 0;
        
        foreach (var prefab in _loadingHandle.Result)
        {
            if (prefab == null) continue;
            
            var netObj = prefab.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                networkObjects.Add(netObj);
                validCount++;
                Debug.Log($"注册网络对象: {prefab.name}");
            }
            else
            {
                Debug.LogWarning($"预制体缺少NetworkObject组件: {prefab.name}");
            }
            
            // 每处理10个预制体暂停一帧
            if (validCount % 10 == 0) await UniTask.Yield();
        }

        if (networkObjects.Count > 0)
        {
            try
            {
                var config = Network.CloneDefaultConfig();
                config.Prefabs = networkObjects;
                Game.NotifyConfigLoaded(config);
                Debug.Log($"成功配置 {networkObjects.Count} 个网络预制体");
            }
            catch (Exception e)
            {
                Debug.LogError($"配置异常: {e.Message}");
            }
        }
        else
        {
            Debug.LogError("未找到任何有效网络预制体");
        }
    }
    
    public void Release()
    {
        if (_loadingHandle.IsValid())
        {
            Addressables.Release(_loadingHandle);
            Debug.Log("Addressables资源已释放");
        }
    }
}