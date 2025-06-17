using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using UnityEngine.AddressableAssets;

public class LoadManager : MonoBehaviour
{
    public static LoadManager Instance { get; private set; }

    private List<ILoadingTask> _loadingTasks = new List<ILoadingTask>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 添加加载任务
    public void AddLoadingTask(ILoadingTask task)
    {
        if (task != null)
        {
            _loadingTasks.Add(task);
        }
    }

    // 执行所有加载任务（带进度）
    public async UniTask ExecuteAll()
    {
        bool uiAvailable = LoadingScreen.Instance != null;
        
        // 只在UI可用时显示加载界面
        if (uiAvailable)
        {
            await LoadingScreen.Instance.Show();
        }
        
        float totalWeight = CalculateTotalWeight();
        float accumulatedProgress = 0;
        
        try
        {
            // 执行所有任务
            for (int i = 0; i < _loadingTasks.Count; i++)
            {
                var task = _loadingTasks[i];
                
                // 更新状态（如果UI可用）
                if (uiAvailable)
                {
                    LoadingScreen.Instance.SetStatus(task.GetName());
                }
                
                // 创建进度回调
                var taskProgress = new Progress<float>(progress => 
                {
                    if (!uiAvailable) return;
                    
                    float weightedProgress = accumulatedProgress + (progress * task.GetWeight() / totalWeight);
                    LoadingScreen.Instance.SetProgress(weightedProgress);
                });
                
                // 执行加载任务
                await task.Load(taskProgress);
                
                // 更新累计进度
                accumulatedProgress += task.GetWeight() / totalWeight;
                
                // 更新最终进度（如果UI可用）
                if (uiAvailable)
                {
                    LoadingScreen.Instance.SetProgress(accumulatedProgress);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error during loading: {e}");
            throw; // 重新抛出异常保持错误传播
        }
        finally
        {
            // 只在UI可用时隐藏加载界面
            if (uiAvailable)
            {
                await LoadingScreen.Instance.Hide();
            }
            
            // 清空任务列表
            _loadingTasks.Clear();
        }
    }

    private float CalculateTotalWeight()
    {
        float total = 0;
        foreach (var task in _loadingTasks)
        {
            total += task.GetWeight();
        }
        return total;
    }
}

// 加载任务接口
public interface ILoadingTask
{
    float GetWeight();
    string GetName();
    UniTask Load(IProgress<float> progress);
}