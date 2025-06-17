using System;
using System.Collections.Generic;
using UnityEngine;
using Netick.Unity;


/// <summary>
/// 专门处理场景加载事件的处理器，负责管理场景加载完成时的通知分发。
/// </summary>
public class SceneLoadEventHandler : NetworkEventsListener
{
    /// <summary>
    /// 缓存场景中实现了 INetickSceneLoadDone 接口的监听器列表。
    /// </summary>
    private readonly List<INetickSceneLoadDone> _sceneListeners = new List<INetickSceneLoadDone>();
    
    /// <summary>
    /// 缓存已通知过的监听器，避免重复通知
    /// </summary>
    private readonly HashSet<INetickSceneLoadDone> _notifiedListeners = new HashSet<INetickSceneLoadDone>();
    
    /// <summary>
    /// 当前关联的自定义网络沙盒
    /// </summary>
    private Orchard _currentSandbox;

    /// <summary>
    /// 初始化事件处理器
    /// </summary>
    private void Initialize()
    {
        _sceneListeners.Clear();
        _notifiedListeners.Clear();
        _sceneListeners.AddRange(Sandbox.FindObjectsOfType<INetickSceneLoadDone>());
    }

    /// <summary>
    /// 通知符合条件的监听器场景加载已完成
    /// </summary>
    private void DispatchLoadCompleteEvent()
    {
        if (_currentSandbox == null)
        {
            Debug.LogWarning("无法分发事件: 当前沙盒为空");
            return;
        }
        
        int notifiedCount = 0;
        
        foreach (var listener in _sceneListeners)
        {
            // 跳过空引用
            if (listener == null) continue;
            
            // 跳过已通知的监听器
            if (_notifiedListeners.Contains(listener)) continue;
            
            try
            {
                listener.OnSceneLoadDone(_currentSandbox);
                _notifiedListeners.Add(listener);
                notifiedCount++;
            }
            catch (System.Exception ex)
            {
              Debug.LogError($"listener {listener.GetType().Name}: {ex}");
            }
        }
        
    }

    /// <summary>
    /// 当沙盒启动时调用，处理初始场景加载
    /// </summary>
    public override void OnStartup(NetworkSandbox sandbox)
    {
        _currentSandbox = sandbox as Orchard;
        
        if (_currentSandbox == null)
        {
            Debug.LogError($"无效的沙盒类型: {sandbox.GetType().Name}, 期望 CKBNetworkSandbox");
            return;
        }
        
        HandleSceneLoaded();
    }

    /// <summary>
    /// 当场景操作完成时调用
    /// </summary>
    public override void OnSceneOperationDone(NetworkSandbox sandbox, NetworkSceneOperation sceneOperation)
    {
        if (sceneOperation.IsLoadOperation)
        {
            //_currentSandbox = sandbox as Orchard;
            
            if (_currentSandbox == null)
            {
                Debug.LogError($"无效的沙盒类型: {sandbox.GetType().Name}, 期望 CKBNetworkSandbox");
                return;
            }
            
            HandleSceneLoaded();
        }
    }
    
    /// <summary>
    /// 统一处理场景加载完成的逻辑
    /// </summary>
    private void HandleSceneLoaded()
    {
        // 初始化监听器
        Initialize();
        
        // 分发事件
        DispatchLoadCompleteEvent();
    }
}