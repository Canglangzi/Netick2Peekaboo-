using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Debug = UnityEngine.Debug;

public static partial class Game
{
    private class ManagedResource
    {
        public Object Asset;
        public int ReferenceCount;
        public float LastAccessTime;
        public AsyncOperationHandle Handle;
    }

    private static readonly Dictionary<string, ManagedResource> _managedResources = new();
    private static readonly Stopwatch _memoryCheckTimer = new();
    private const float MEMORY_CHECK_INTERVAL = 3000; // 3秒
    private const float HIGH_MEMORY_THRESHOLD = 0.8f; // 80%
    
     [RuntimeInitializeOnLoadMethod]
    private static void InitializeResourceManager()
    {
        _memoryCheckTimer.Start();
    }
    public static async UniTask<GameObject> LoadAndInstantiateAssetAsync(
        AssetReference assetReference,
        Transform parent = null,
        CancellationToken cancellationToken = default)
    {
        Debug.Assert(assetReference != null);
        var prefab = await LoadAssetAsync<GameObject>(assetReference, cancellationToken);
        var instance = parent == null ? Object.Instantiate(prefab) : Object.Instantiate(prefab, parent);
        return instance;
    }
    public static async UniTask<T> LoadAssetAsync<T>(AssetReference assetReference, 
        CancellationToken cancellationToken = default) where T : Object
    {
        CheckMemoryPressure();

        if (assetReference == null)
            throw new System.ArgumentNullException("资源引用不能为空");
        
        var guid = assetReference.RuntimeKey.ToString();

        // 已加载的资源
        if (_managedResources.TryGetValue(guid, out var resource))
        {
            resource.ReferenceCount++;
            resource.LastAccessTime = Time.realtimeSinceStartup;
            return (T)resource.Asset;
        }

        // 新资源加载
        var handle = Addressables.LoadAssetAsync<T>(assetReference);
        await handle.WithCancellation(cancellationToken);
        
        if (handle.Status == AsyncOperationStatus.Failed)
            throw new System.Exception($"资源加载失败: {assetReference.AssetGUID}");

        var newResource = new ManagedResource
        {
            Asset = handle.Result,
            ReferenceCount = 1,
            LastAccessTime = Time.realtimeSinceStartup,
            Handle = handle
        };

        _managedResources.Add(guid, newResource);
        return (T)newResource.Asset;
    }

    /// <summary>
    /// 释放资源引用
    /// </summary>
    public static void ReleaseAsset(AssetReference assetReference)
    {
        if (assetReference == null) return;
        
        var guid = assetReference.RuntimeKey.ToString();
        if (_managedResources.TryGetValue(guid, out var resource))
        {
            resource.ReferenceCount--;
            resource.LastAccessTime = Time.realtimeSinceStartup;
        }
    }

    /// <summary>
    /// 强制卸载资源
    /// </summary>
    public static void ForceUnloadAsset(AssetReference assetReference)
    {
        if (assetReference == null) return;
        
        var guid = assetReference.RuntimeKey.ToString();
        if (_managedResources.TryGetValue(guid, out var resource))
        {
            Addressables.Release(resource.Handle);
            _managedResources.Remove(guid);
        }
    }

    /// <summary>
    /// 检查内存压力并自动释放
    /// </summary>
    private static void CheckMemoryPressure()
    {
        if (_memoryCheckTimer.ElapsedMilliseconds < MEMORY_CHECK_INTERVAL) 
            return;
        
        _memoryCheckTimer.Restart();

        var memoryUsage = GetSystemMemoryUsage();
        if (memoryUsage < HIGH_MEMORY_THRESHOLD) return;

        Debug.Log($"检测到高内存使用({memoryUsage:P0})，触发自动资源释放...");

        // 获取可释放候选资源（最近最少使用且引用计数低）
        var releaseCandidates = _managedResources
            .Where(r => r.Value.ReferenceCount <= 1)
            .OrderBy(r => r.Value.LastAccessTime)
            .ThenBy(r => r.Value.ReferenceCount)
            .ToList();

        foreach (var candidate in releaseCandidates)
        {
            if (GetSystemMemoryUsage() < HIGH_MEMORY_THRESHOLD * 0.9f) break;

            Debug.Log($"释放资源: {candidate.Key} (引用:{candidate.Value.ReferenceCount})");
            Addressables.Release(candidate.Value.Handle);
            _managedResources.Remove(candidate.Key);
        }
    }

    /// <summary>
    /// 获取系统内存使用率
    /// </summary>
    private static float GetSystemMemoryUsage()
    {
        try
        {
            var memoryInfo = Process
                .Start("wmic", "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value");
            memoryInfo.WaitForExit(100);
            var output = memoryInfo.StandardOutput.ReadToEnd();
            var lines = output.Trim().Split("\n");
            
            var total = 0L;
            var free = 0L;
            
            foreach (var line in lines)
            {
                if (line.StartsWith("TotalVisibleMemorySize")) 
                    total = long.Parse(line.Split('=')[1]);
                if (line.StartsWith("FreePhysicalMemory")) 
                    free = long.Parse(line.Split('=')[1]);
            }
            
            return total > 0 ? 1.0f - (free * 1024f) / total : 0f;
        }
        catch
        {
            // 备用方案：使用Unity性能统计
            return System.GC.GetTotalMemory(false) / (float)SystemInfo.systemMemorySize;
        }
    }
}