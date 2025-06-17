using UnityEngine;
using Netick;
using Netick.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using CockleBurs.GamePlay;
// using CocKleBurs.Console;
// using CocKleBurs.Core;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine.Serialization;


[DisallowMultipleComponent]
    [CKBScriptInfo(
        "Network Manager",
        "This script manages the network settings and connectivity for the Debug. It handles server and client initialization, network transport configuration, authentication, and reconnection logic. The NetworkManager also manages the sandbox environment for networked Debugplay.")]
    public partial class NetworkManager : Cherry
    {
        [CKBFieldHelp("CurrentNetworkMode")]
        public NetworkMode CurrentNetworkMode = NetworkMode.None;
        [CKBFieldHelp("If true, the network manager will start as a host when the Debug starts.")]
        public bool AutoStartAsHost;

        [CKBFieldHelp("If true, the network manager will start automatically in the editor.")]
        public bool AutoStartInEditor;

        [CKBFieldHelp("Determines the mode for starting the network manager in a headless environment.")]
        public HeadlessStartMode HeadlessMode = HeadlessStartMode.DoNothing;

        [CKBFieldHelp("If true, the application will continue running in the background when the window is not focused.")]
        public bool RunInBackground = true;

        [Header("Network Settings")]
        [Space]
        [CKBFieldHelp("Prefab to use for the sandbox environment.")]
        [BoxGroup("SandboxPrefab")] public GameObject SandboxPrefab;

        [CKBFieldHelp("The transport provider used for network communication.")]
        public NetworkTransportProvider Transport;

        [CKBFieldHelp("The network authenticator used for handling authentication.")]
        public NetworkAuthenticator Authenticator; // Can be SimpleNetworkAuthenticator or another implementation

        [CKBFieldHelp("IP address of the server to connect to.")]
        public string ServerIPAddress = "127.0.0.1";

        [CKBFieldHelp("Port number for network communication.")]
        public int Port = 7777;

        [CKBFieldHelp("Number of clients to support.")]
        public int NumberOfClients = 1;

        [CKBFieldHelp("Number of servers to support.")]
        public int NumberOfServers = 1;
    
        [CKBFieldHelp("The current network sandbox.")]
        [CanBeNull]
        public NetworkSandbox Sandbox { get;  set; }

        [CKBFieldHelp("Array of all network sandboxes.")]
        private NetworkSandbox[] allSandboxes;

        public enum HeadlessStartMode
        {
            DoNothing,
            AutoStartServer,
            AutoStartClient
        }
        public enum NetworkMode
        {
            None,
            StandAlone,
            Client,
            ListenServer,
            DedicatedServer
        }
        /*1、StandAlone：单机模式，不联网

        2、Client，网络游戏中的客户端。

        3、ListenServer，服务器和一个客户端

        4、DedicatedServer，专用服务器，没有图形表现，本地没有客户端*/
        public bool IsClient => Sandbox?.IsClient ?? false;
        public bool IsServer => Sandbox?.IsServer ?? false;
        public bool IsHost => Sandbox?.IsHost ?? false;
        public bool IsRunning => Sandbox?.IsRunning ?? false;
        public bool IsConnected => Sandbox?.IsConnected ?? false;
        public bool IsOffline => !IsClient && !IsServer && !IsHost;
        public bool IsSinglePlayer => !IsClient && !IsServer && !IsHost && (Sandbox?.IsRunning ?? false);
        
        public bool IsVisible => Sandbox?.IsVisible ?? false;
        public bool SandboxIsNull => Sandbox != null;
        
        public delegate void ReconnectFailedDelegate();
        public delegate void HostStartedDelegate();
        public delegate void ServerStartedDelegate();
        public delegate void ClientStartedDelegate();
        public delegate void ServerStoppedDelegate();
        public delegate void ClientStoppedDelegate();
        public delegate void HostStoppedDelegate();
        public delegate void BeforeStartServerDelegate();
        public event ReconnectFailedDelegate OnReconnectFailed;
        public event HostStartedDelegate OnHostStarted;
        public event ServerStartedDelegate OnServerStarted;
        public event ClientStartedDelegate OnClientStarted;
        public event ServerStoppedDelegate OnServerStopped;
        public event ClientStoppedDelegate OnClientStopped;
        public event HostStoppedDelegate OnHostStopped;
        public event BeforeStartServerDelegate OnBeforeStartServer;
        
        public NetickConfig netickConfig => Game.NetickConfig;
            
        

        private void Start()
        {
            Application.runInBackground = RunInBackground;

            switch (CurrentNetworkMode)
            {
                case NetworkMode.StandAlone:
                    StartSinglePlayer();
                    break;
                case NetworkMode.Client:
                    StartClient();
                    break;
                case NetworkMode.ListenServer:
                    StartHost();
                    break;
                case NetworkMode.DedicatedServer:
                    StartServer();
                    break;
                case NetworkMode.None:
                default:
                    break;
            }

            if (AutoStartAsHost)
            {
                StartHost();
            }
            else if (AutoStartInEditor)
            {
                HandleEditorAutoStart();
            }
        }

        private void HandleEditorAutoStart()
        {
            switch (HeadlessMode)
            {
                case HeadlessStartMode.AutoStartServer:
                    StartServer();
                    break;
                case HeadlessStartMode.AutoStartClient:
                    StartClient();
                    break;
                case HeadlessStartMode.DoNothing:
                    break;
            }
        }

        public void StartHost()
        {
            if (!IsRunning)
            {
                try
                {      
                    OnBeforeStartServer?.Invoke();
                    Debug.Log($"Starting host at IP: {GetIPAddress()} and Port: {GetServerPort()}");
                    Sandbox = Network.StartAsHost(Transport, Port, SandboxPrefab,netickConfig);
                     Register();
                    OnStartHost();
                    Debug.Log("[Started]Host");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Failed to start host: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }

        public void StartClient()
        {
            if (!IsRunning)
            {
                try
                {
                    Sandbox = Network.StartAsClient(Transport, Port, SandboxPrefab,netickConfig);
                    SandboxRegistry.Instance.RegisterSandbox(Sandbox);
                    byte[] connectionData = GetConnectionData();
                    int connectionDataLength = connectionData?.Length ?? 0;

                    if (Authenticator == null || Authenticator.AuthenticateConnection(connectionData, connectionDataLength))
                    {
                        Debug.Log($"Starting Connect at IP: {GetIPAddress()} and Port: {GetServerPort()}");
                        Register();
                        OnStartClient();
                        Debug.Log("[Started]Client");
                    }
                    else
                    {
                        Network.Shutdown();
                        Debug.LogError("Connection authentication failed.");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Failed to start client: {ex.Message}\n{ex.StackTrace}");
                }
                
            }
        }
        public void ClientConnect()
        {
            Sandbox.Connect(Port, ServerIPAddress);
        }
        public void ClientDisconnectFromServer()
        {
            Sandbox.DisconnectFromServer();
        }
        private byte[] GetConnectionData()
        {
            return Authenticator?.GetConnectionData();
        }

        public void StartServer()
        {
            if (!IsRunning)
            {
                try
                {
                    OnBeforeStartServer?.Invoke();
                    Debug.Log($"Starting Server at IP: {GetIPAddress()} and Port: {GetServerPort()}");
                    Sandbox = Network.StartAsServer(Transport, Port, SandboxPrefab,netickConfig);
                     Register();
                    Debug.Log("[Started]Server");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Failed to start server: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }

        public void StartSinglePlayer()
        {
            if (!IsRunning)
            {
                try
                {
                    Sandbox = Network.StartAsSinglePlayer(SandboxPrefab,netickConfig);
                      Register();
                    Debug.Log("[Started]SinglePlayer");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Failed to start single player: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }
        public void Register()
        {
            SandboxRegistry.Instance.RegisterSandbox(Sandbox);
            SandboxRegistry.Instance.DefaultSandbox = Sandbox as Orchard;
            RegisterEvents();
            OnStartServer();
        }
   

        public void StartMultiplePeers()
        {
            if (!IsRunning)
            {
                try
                {
                    // 启动多个沙盒（服务器和客户端）
                    var launchResults = Network.Launch(StartMode.MultiplePeers, new LaunchData()
                    {
                        SandboxPrefab = SandboxPrefab,
                        Port = Port,
                        Config = netickConfig,
                        TransportProvider = Transport,
                        NumberOfServers = NumberOfServers,
                        NumberOfClients = NumberOfClients
                    });

                    // 创建一个总的沙盒列表来存储所有沙盒
                    allSandboxes = new NetworkSandbox[launchResults.Servers.Length + launchResults.Clients.Length];
                    launchResults.Servers.CopyTo(allSandboxes, 0); // 将服务器沙盒加入列表
                    launchResults.Clients.CopyTo(allSandboxes, launchResults.Servers.Length); // 将客户端沙盒加入列表

                    // 使用 for 循环注册所有服务器沙盒
                    for (int i = 0; i < launchResults.Servers.Length; i++)
                    {
                        var serverSandbox = launchResults.Servers[i];
                        SandboxRegistry.Instance.RegisterSandbox(serverSandbox);
                        Debug.Log("Server sandbox registered: " + serverSandbox.Name);
                    }

                    // 连接所有客户端到第一个服务器
                    if (launchResults.Servers.Length > 0 && launchResults.Clients.Length > 0)
                    {
                        var firstServer = launchResults.Servers[0]; // 获取第一个服务器沙盒
                        for (int i = 0; i < launchResults.Clients.Length; i++)
                        {
                            var clientSandbox = launchResults.Clients[i];
                            clientSandbox.Connect(Port, ServerIPAddress); // 连接客户端到第一个服务器
                            SandboxRegistry.Instance.RegisterSandbox(clientSandbox); // 注册客户端沙盒
                            Debug.Log("Client sandbox registered and connected: " + clientSandbox.Name);
                        }
                    }

                    RegisterEvents(); // 注册相关事件
                    Debug.Log("Multiple peers started successfully. All clients connected to the first server.");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Failed to start multiple peers: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }



        public void StopHost()
        {
            if (IsHost)
            {
                try
                {
                    StopServer();
                    StopClient();
                    OnStopHost();
                    Debug.Log("[Stopped]Host");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Failed to stop host: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }

        public void StopClient()
        {
            if (IsConnected)
            {
                try
                {
                    Sandbox.DisconnectFromServer();
                    SandboxRegistry.Instance.UnregisterSandbox(Sandbox);
                    OnStopClient();
                    Debug.Log("[Stopped]Client");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Failed to stop client: {ex.Message}\n{ex.StackTrace}");
                }
            }
            else
            {
                Network.Shutdown(); 
            }
        }

        public void StopServer()
        {
            if (IsServer)
            {
                try
                {
                    Network.Shutdown();
                    OnStopServer();
                    SandboxRegistry.Instance.UnregisterSandbox(Sandbox);
                    Debug.Log("[Stopped]Server");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Failed to stop server: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }

        public void StopSinglePlayer()
        {
            if (IsSinglePlayer)
            {
                try
                {
                    Network.Shutdown();
                    Debug.Log("[Started]Single Player");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Failed to stop single player: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }

        public void StopMultiplePeers()
        {
            try
            {
                Network.Shutdown();
                Debug.Log("Multiple peers stopped successfully.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to stop multiple peers: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void RegisterEvents()
        {
            if (Sandbox != null)
            {
                Sandbox.Events.OnStartup += OnStartup;
                Sandbox.Events.OnShutdown += OnShutdown;
                Sandbox.Events.OnConnectRequest += OnConnectRequest;
                Sandbox.Events.OnConnectFailed += OnConnectFailed;
            }
        }

        private void UnregisterEvents()
        {
            if (Sandbox != null)
            {
                Sandbox.Events.OnStartup -= OnStartup;
                Sandbox.Events.OnShutdown -= OnShutdown;
                Sandbox.Events.OnConnectRequest -= OnConnectRequest;
                Sandbox.Events.OnConnectFailed -= OnConnectFailed;
            }
        }

        public void OnConnectRequest(NetworkSandbox sandbox, NetworkConnectionRequest request)
        {
            // 获取当前已连接的客户端数量
            var currentClientCount = sandbox.ConnectedClients.Count;
            Debug.Log(currentClientCount);
            
        }


        public void OnConnectFailed(NetworkSandbox sandbox, ConnectionFailedReason reason)
        {
            Debug.Log($"Connection failed: {reason}");
        }

        public void OnStartup(NetworkSandbox networkSandbox)
        {
         //   Debug.Log("Netick started");
           OnNetworkStartup();
        }
        
        public  void OnShutdown(NetworkSandbox networkSandbox)
        {
         //   Debug.Log("Netick shutdown");
            UnregisterEvents();
            Sandbox = null;
            
            OnNetworkShutdown();
        }
        protected virtual void OnNetworkStartup(){
        }
        protected virtual void OnNetworkShutdown() {
        }
        private void OnDestroy()
        {
            UnregisterEvents();
            if (IsRunning)
            {
                Network.Shutdown();
            }
        }
        

        public void NetworkInstantiate(GameObject go, Vector3 position, Quaternion rotation, NetworkConnection inputSource = null)
        {
            if (Sandbox != null)
            {
                Sandbox.NetworkInstantiate(go, position, rotation, inputSource);
            }
        }

        public void NetworkDestroy(NetworkObject go)
        {
            if (Sandbox != null)
            {
                Sandbox.Destroy(go);
            }
        }

        
        // API to get current network state
        public string GetNetworkState()
        {
            if (IsHost) return "Host";
            if (IsServer) return "Server";
            if (IsClient) return "Client";
            if (IsSinglePlayer) return "Single Player";
            return "Offline";
        }

        // Broadcast message to all clients
        public void BroadcastMessage(string message)
        {
            if (IsServer || IsHost)
            {
                // Implement message broadcast logic
                Debug.Log($"Broadcasting message: {message}");
            }
        }

        // Unicast message to a specific client
        public void UnicastMessage(NetworkConnection client, string message)
        {
            if (IsServer || IsHost)
            {
                // Implement message unicast logic
                Debug.Log($"Sending message to client: {message}");
            }
        }

        // API for room management
        public void CreateRoom(string roomName)
        {
            if (IsServer || IsHost)
            {
                // Implement room creation logic
                Debug.Log($"Room created: {roomName}");
            }
        }

        public void DestroyRoom(string roomName)
        {
            if (IsServer || IsHost)
            {
                // Implement room destruction logic
                Debug.Log($"Room destroyed: {roomName}");
            }
        }
        /// <summary>
        /// 获取服务器的 IP 地址。
        /// </summary>
        /// <returns>返回服务器的 IP 地址。</returns>
        public string GetIPAddress()
        {
            
            return ServerIPAddress;
        }

        /// <summary>
        /// 获取服务器的端口号。
        /// </summary>
        /// <returns>返回服务器的端口号。</returns>
        public int GetServerPort()
        {
            return Port;
        }
        public virtual void OnStartHost()=>OnHostStarted?.Invoke();
 

        public virtual void OnStartServer()=>OnServerStarted?.Invoke();
        

        public virtual void OnStartClient() => OnClientStarted?.Invoke();
        

        public virtual void OnStopServer() =>OnServerStopped?.Invoke();

        public virtual void OnStopClient() => OnClientStopped?.Invoke();
        

        public virtual void OnStopHost() => OnHostStopped?.Invoke();
    }
