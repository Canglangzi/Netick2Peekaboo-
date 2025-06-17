using System;
using CockleBurs.GamePlay;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public class LoadGameInstanceTask : ILoadingTask
{
    
    public GameObject GameInstance { get; set; }
    public float GetWeight() => 1.5f;
    public string GetName() => "初始化游戏实例";
    
    public async UniTask Load(IProgress<float> progress)
    {
        if (Game.GameplaySettings?.InstanceSettings?.GameInstancePrefab == null)
        {
            Debug.LogError("游戏实例资源无效");
            return;
        }
        
        progress.Report(0);
        
        try
        {
            // 实例化游戏实例
            var handle = Game.GameplaySettings.InstanceSettings.GameInstancePrefab.InstantiateAsync();
            
            // 更新进度
            while (!handle.IsDone)
            {
                progress.Report(handle.PercentComplete * 0.9f);
                await UniTask.DelayFrame(1);
            }
            
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameInstance = handle.Result;
                
                if (Game.GameplaySettings.InstanceSettings.DontDestroyOnLoad)
                {
                    Object.DontDestroyOnLoad(GameInstance);
                }
                progress.Report(1f);
            }
            else
            {
                Debug.LogError("实例化游戏实例失败");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"初始化游戏实例异常: {ex.Message}");
        }
    }
    
}