using System;
using UnityEngine;

public class GameServer : Cherry
{
   //[OdinSerialize]
   public LobbyServiceProvider lobbyServiceProvider;
   //[OdinSerialize]
   //public ServerProvider serverProvider;
  //[OdinSerialize]
   public ServerQueueProvider serverQueueProvider;
   public void OnEnable()
   {
      base.Orchard.EventLobby.OnLobbyPlayerJoined += LobbyEventOnOnLobbyPlayerJoined;
      Orchard.EventLobby.OnLobbyChatMessage += LobbyEventOnOnLobbyChatMessage;
   }
   public void OnDisable()
   {
      Orchard.EventLobby.OnLobbyPlayerJoined -= LobbyEventOnOnLobbyPlayerJoined;
      Orchard.EventLobby.OnLobbyChatMessage -= LobbyEventOnOnLobbyChatMessage;
   }
   private void LobbyEventOnOnLobbyChatMessage(ulong arg1, ulong arg2, string message)
   {
      // 👇 判断是否是发给自己的命令
      if (message.StartsWith("#ready:"))
      {
         string targetId = message.Substring("#ready:".Length);
         if (targetId == Orchard.playerid.ToString())
         {
            
         }
      }
   }
   private void LobbyEventOnOnLobbyPlayerJoined(ulong arg1, ulong arg2)
   {
      if (lobbyServiceProvider.CurrentLobbyService.LobbyId == arg1)
      {
         serverQueueProvider.JoinQueue(arg2);   
      }
      Orchard.Log("Test");
   }
   
}
