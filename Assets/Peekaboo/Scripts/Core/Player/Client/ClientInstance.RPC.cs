using System.Collections.Generic;
using Netick;
using Netick.Unity;
using UnityEngine;


    public partial class ClientInstance
    {

        [Rpc(RpcPeers.InputSource, RpcPeers.Owner, true)]
        private void CmdPlayerName(NetworkString32 name, ulong steamId)
        {
            /*characterName = new NetworkString32(name);*/
            characterName = name;
            //   SteamID = steamId;
            Debug.Log($"CmdPlayerName: {name}, SteamID: {steamId}");
        }

      
        [Rpc(source: RpcPeers.InputSource, target: RpcPeers.Owner, isReliable: true, localInvoke: false)]
        public void RequestSlotRpc()
        {
            //  AssignPlayerSlot();
            Debug.Log("RequestSlotRpc executed successfully.");
        }

        [Rpc(source: RpcPeers.InputSource, target: RpcPeers.Owner, isReliable: true, localInvoke: false)]
        public void CmdAddPlayer(NetworkObjectRef playerRef)
        {
          

        }

    
    }