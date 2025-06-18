using System;
using System.Collections.Generic;
//using CocKleBurs.Core;
using Netick.Unity;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace CockleBurs.GamePlay
{
    [CreateAssetMenu(menuName = "GameConfig")]
    public class GameplaySettings : ScriptableObject
    {

        #region 对象池配置
        [System.Serializable]
        public struct PoolConfig
        {
            public bool active;
            [FormerlySerializedAs("Object")]
            public AssetReferenceT<GameObject> Prefab; 
            public int poolSize;
            public bool usePoolSize;
            public bool isNetworkObject;
            public uint hashId;
        }
       
        [System.Serializable]
        public class ObjectPoolSettings
        {
            public List<PoolConfig> ObjectPools = new List<PoolConfig>();
        }
        
        [SerializeField]
        public ObjectPoolSettings PoolSettings = new ObjectPoolSettings();
        #endregion

        #region 游戏实例配置
        [System.Serializable]
        public class GameInstanceSettings
        { 
            [Header("Gameplay")]
            public AssetReference GameInstancePrefab;

            public bool DontDestroyOnLoad = true;
            public AssetReference SceneConfig;
        }
        
        [SerializeField]
        public GameInstanceSettings InstanceSettings = new GameInstanceSettings();
        #endregion

        #region Steam集成设置
        [System.Serializable]
        public class SteamIntegration
        {
            public uint GameAPI = 480;
            public string SupportPage = "https://store.steampowered.com/";
        }
        
        [SerializeField]
        public SteamIntegration SteamSettings = new SteamIntegration();
        #endregion

        #region 编辑器配置
        [System.Serializable]
        public class EditorConfiguration
        {
      
        }
        
        [SerializeField]
        public EditorConfiguration EditorSettings = new EditorConfiguration();
        #endregion

        [System.Serializable]
        public struct Config
        {
            [Header("Player")]
            public Actor PlayerPrefab;
            public int PlayerPoolSize;
            public bool PoolActive;
            
            public bool EnableTestMode;
            public Actor TestPlayerPrefab;
        }

      [SerializeField]public Config GameConfig;
    }
}