using System;
using CockleBurs.GamePlay;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadGameplaySettingsTask : ILoadingTask
{
    public float GetWeight() => 1.0f;
    public string GetName() => "加载游戏配置";
    
    public async UniTask Load(IProgress<float> progress)
    {
        progress.Report(0);
        
        try
        {
            // 加载设置
            var handle = Addressables.LoadAssetAsync<GameplaySettings>("GamePlaySettings");
            
            // 更新进度
            while (!handle.IsDone)
            {
                progress.Report(handle.PercentComplete * 0.8f);
                await UniTask.DelayFrame(1);
            }
            
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Game.GameplaySettings = handle.Result;
                progress.Report(1f);
            }
            else
            {
                Debug.LogError("加载设置失败");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"加载游戏设置异常: {ex.Message}");
        }
    }
}
