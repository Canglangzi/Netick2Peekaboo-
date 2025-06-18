using System;
using System.Threading.Tasks;
using CockleBurs.GamePlay;
using Cysharp.Threading.Tasks;
using Netick.Unity;
using Unity.Burst;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public static partial class Game
{
    public static GameplaySettings GameplaySettings;
    public static GameplaySettings EditorGameplaySettings;
    public static Action<NetickConfig> OnConfigLoaded;
    public static NetickConfig NetickConfig;
    public static LoadManager LoadManager;
    public static void NotifyConfigLoaded(NetickConfig networkcfg)
    {
        NetickConfig = networkcfg;
        OnConfigLoaded?.Invoke(networkcfg);
    }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeBeforeSceneLoad()
    {
        var loadManagerGO = new GameObject("LoadManager");
        Object.DontDestroyOnLoad(loadManagerGO);
        LoadManager = loadManagerGO.AddComponent<LoadManager>();
        
        LoadRuntimeSettingsAndThenLoadAsset().Forget();
    }

    public static async UniTask  Load()
    {
        LoadManager.AddLoadingTask(new LoadGameInstanceTask());
        LoadManager.AddLoadingTask(new NetickPrefabLoadingTask());
        await LoadManager.ExecuteAll();
    }
    
    /// <summary>
    /// 先加载运行时设置，然后加载所需资源
    /// </summary>
    private static async UniTaskVoid LoadRuntimeSettingsAndThenLoadAsset()
    {
        try
        {
            // 1. 加载GameplaySettings
            var gameplaySettings = await Addressables.LoadAssetAsync<GameplaySettings>("GamePlaySettings");
            Debug.Log($"GameplaySettings加载成功: {gameplaySettings}");
            
            // 2. 将加载的设置保存到静态变量
            GameplaySettings = gameplaySettings;

            await Load();
         //   await LoadAsset();
        }
        catch (Exception ex)
        {
            Debug.LogError($"加载失败: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 加载并实例化游戏实例
    /// </summary>
    private static async UniTask LoadAsset()
    {
        if (GameplaySettings?.InstanceSettings?.GameInstancePrefab == null)
        {
            Debug.LogError("GameInstancePrefab未配置或未加载成功");
            return;
        }
        
        var gameInstance = await GameplaySettings.InstanceSettings.GameInstancePrefab.InstantiateAsync();
        Debug.Log($"游戏实例创建成功: {gameInstance.name}");

        if (GameplaySettings.InstanceSettings.DontDestroyOnLoad)
        {
            UnityEngine.Object.DontDestroyOnLoad(gameInstance);
        }

    }
}
public static unsafe class StringId
{
    public static int StringToHash(string str)
    {
        if (string.IsNullOrEmpty(str))
            return 0;

        fixed (char* ptr = str)
        {
            return ComputeHashBurst(ptr, str.Length);
        }
    }

   // [BurstCompile]
    private static int ComputeHashBurst(char* chars, int length)
    {
        int hash = 23;
        for (int i = 0; i < length; ++i)
        {
            hash = hash * 31 + chars[i];
        }
        return hash;
    }

#if !UNITY_EDITOR
    // AOT预编译（针对不支持JIT的平台）
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
        // 触发Burst编译
        ComputeHashBurst(null, 0);
    }
#endif
}