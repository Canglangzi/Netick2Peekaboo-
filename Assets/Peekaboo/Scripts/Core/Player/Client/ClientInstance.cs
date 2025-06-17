using System.Collections.Generic;

//using CocKleBurs.Console;
using Netick;
using Netick.Transports.Facepunch;
using Netick.Unity;
using Steamworks;
using UnityEngine;

    public partial class ClientInstance : ClientEntity//,ILocalPlayer//,INetworkSpawn
    {
        [Networked, SerializeField] public NetworkString32 PlayerName { get; set; } = "null";

        [Networked, SerializeField] public NetworkString32 characterName { get; set; }     

        [SerializeField] public NetworkBehaviourRef<ClientInstance> clientInstanceRef;
        
    
        public static ClientInstance LocalPlayer;
        public static NetworkSandbox LocalSandbox => LocalPlayer.Object.Sandbox;
        
        public int LocalPlayerId { get; set; }
        public CKBSteamAPI.SteamUserInfo steamUser;


        private static HashSet<int> _assignedPlayerIds = new HashSet<int>();
        

        public override  void  NetworkStart()
        {
            clientInstanceRef = new NetworkBehaviourRef<ClientInstance>(this);
            if (base.IsInputSource)
            {
                LocalPlayer = this;
                base.Orchard.onPlayerSpawned.AddListener(OnPlayerSpawned);
                if (SteamClient.IsValid)
                { 
                   
                }
            }
        
        }

        public override void OnInputSourceLeft()
        {
            Sandbox.Destroy(Object);
        }

        public void OnShutdown(NetworkSandbox sandbox)
        {
            Destroy(gameObject);    
        }
        
        private void OnPlayerSpawned(Actor replicatedObject, NetworkPlayer networkPlayer)
        {
            if (Orchard.IsServer)
            {
              SteamId ID =  FacepunchTransport.GetPlayerSteamID(networkPlayer);
              CKBSteamAPI.GetSteamUserInfo(this, ID, info =>
              {
                  steamUser = info;
                  
              });
              CKBSteamAPI.DownloadAvatar(this, steamUser.AvatarMediumUrl, texture =>
              {
                  // 你可以设置给 RawImage 等 UI
                  Debug.Log("头像下载成功！");
              });
              Debug.Log($"名字: {steamUser.PersonaName}");
              PlayerName =steamUser.PersonaName;
            }
        }
        public  void OnClientDisconnected(NetworkSandbox sandbox, NetworkConnection client, TransportDisconnectReason reason)
        {
            
            
        }
        public void LocalPlayerInitialize()
        {
            
        }

        public void LocalPlayerCleanup()
        {
         
        }

        [Networked,SerializeField] public State PlayerState { get; set; } = State.Despawned;

        public enum State
        {
            Despawned,
            Spawned
        }
        public override void OnDespawned()
        {
            // print("OnDespawned");
            PlayerState = State.Despawned;
        }

        [OnChanged(nameof(PlayerState))]
        void OnPlayerStateChanged(State prevState)
        {
            // print("OnPlayerStateChanged");

            if (PlayerState == State.Despawned)
            {
                //PlayerManager.SpawnedPlayersObj.Remove(PlayerId);
            }
               
        }

        [Rpc(source: RpcPeers.InputSource, target: RpcPeers.Owner, isReliable: true)]
        public void RPC_RequestRespawn() // PlayerRef playerRef
        {
           Debug.Log($"RPC_RequestRespawn by: {Sandbox.CurrentRpcSource}");

            // if (!PlayerManager.SpawnedPlayers.TryGetValue(PlayerId, out var expectedPlayer))
            //     return;
            //
            // if (PlayerManager.SpawnedPlayersObj.ContainsKey(PlayerId)) // Player is still Alive
            //     return;

           // LevelManager.Instance.SpawnPlayer(expectedPlayer.InputSource, PlayerId);
        }

        protected override void OnPlayerIdAssigned()
        {
            Debug.Log($"Player Spawned: IsLocalPlayer: {Object.IsInputSource} PlayerId: {PlayerId.Id} Tick: {Sandbox.Tick}");
        }

        public void OnNetworkSpawn()
        {
            
            
        }
    }

