using Steamworks;
using Steamworks.Data;
using System;
using System.Threading.Tasks;
//using CocKleBurs.Core;
using UnityEngine;

public class SteamLobbyService : Cherry, ILobbyService
{
    private Lobby _currentLobby;
    public Lobby CurrentLobby => _currentLobby;
    
    public ulong LobbyId
    {
        get => _currentLobby.Id;
        set { }
    }

    public int MaxPlayers => _currentLobby.MaxMembers;
    public int CurrentPlayers => _currentLobby.MemberCount;
    public bool IsInLobby => _currentLobby.Id > 0;

    public string GetLobbyData(string key)
    {
        return _currentLobby.GetData(key);
    }
    
    private void OnEnable()
    {
        SteamMatchmaking.OnLobbyCreated += LobbyCreated;
        SteamMatchmaking.OnLobbyEntered += LobbyEntered;
        SteamFriends.OnGameLobbyJoinRequested += GameLobbyJoinRequested;
        SteamMatchmaking.OnLobbyMemberJoined += SteamMatchmakingOnOnLobbyMemberJoined;
        SteamMatchmaking.OnChatMessage += OnLobbyChatMessage;
    }



    private void OnDisable()
    {
        SteamMatchmaking.OnLobbyCreated -= LobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= LobbyEntered;
        SteamFriends.OnGameLobbyJoinRequested -= GameLobbyJoinRequested;
        SteamMatchmaking.OnLobbyMemberJoined -= SteamMatchmakingOnOnLobbyMemberJoined;
        SteamMatchmaking.OnChatMessage -= OnLobbyChatMessage;
    }
    private void OnLobbyChatMessage(Lobby arg1, Friend arg2, string arg3)
    {
       base.Orchard.EventLobby.NotifyLobbyChatMessage(arg1.Id, arg2.Id, arg3);
    }
    private void SteamMatchmakingOnOnLobbyMemberJoined(Lobby arg1, Friend arg2)
    {
        Debug.Log("OnLobbyMemberJoined");
        base.Orchard.EventLobby.NotifyLobbyPlayerJoined(arg1.Id, arg2.Id);
    }

 
    public async void CreateLobby(int maxPlayers)
    {
        try
        {
            Debug.Log("Attempting to create a lobby...");

            var result = await SteamMatchmaking.CreateLobbyAsync(maxPlayers);
            if (result != null)
            {
                _currentLobby = (Lobby)result;
                _currentLobby.SetPublic();
                _currentLobby.SetJoinable(true);

                Debug.Log("Lobby created successfully.");
            }
            else
            {
                Debug.LogError("Failed to create lobby.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error occurred while creating the lobby: {ex.Message}");
        }
    }

    public async void JoinLobby(string lobbyId)
    {
        try
        {
            if (ulong.TryParse(lobbyId, out ulong lobbySteamId))
            {
                Debug.Log($"Attempting to join lobby {lobbyId}...");
                var lobby = new Lobby(lobbySteamId);
                await lobby.Join();

                Debug.Log("Successfully joined lobby.");
            }
            else
            {
                Debug.LogError("Invalid lobby ID format.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error while joining lobby: {ex.Message}");
        }
    }

    public void LeaveLobby()
    {
        try
        {
            _currentLobby.Leave();
            Debug.Log("Left lobby: " + _currentLobby.Id);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in LeaveLobby: {ex.Message}");
        }
    }

    public void SetLobbyData(string key, string value)
    {
        try
        {
            _currentLobby.SetData(key, value);
            Debug.Log($"Lobby data set: {key} = {value}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in SetLobbyData: {ex.Message}");
        }
    }

    public async void GetLobbyList()
    {
        try
        {
            Debug.Log("Requesting list of available lobbies...");

            var lobbies = await SteamMatchmaking.LobbyList.WithMaxResults(20).RequestAsync();

            if (lobbies.Length == 0)
            {
                Debug.Log("No lobbies found.");
                return;
            }

            foreach (var lobby in lobbies)
            {
                string name = lobby.GetData("name");
                Debug.Log($"Lobby: {lobby.Id}, Members: {lobby.MemberCount}/{lobby.MaxMembers}, Name: {name}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in GetLobbyList: {ex.Message}");
        }
    }

    private void LobbyCreated(Result result, Lobby lobby)
    {
        try
        {
            if (result == Result.OK)
            {
                _currentLobby = lobby;
                _currentLobby.SetPublic();
                _currentLobby.SetJoinable(true);

                Debug.Log("Lobby created and is now public and joinable.");
            }
            else
            {
                Debug.LogError($"Failed to create lobby. Result: {result}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in LobbyCreated: {ex.Message}");
        }
    }

    private void LobbyEntered(Lobby lobby)
    {
        try
        {
            _currentLobby = lobby;
            Debug.Log("Entered lobby: " + lobby.Id);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in LobbyEntered: {ex.Message}");
        }
    }

    private async void GameLobbyJoinRequested(Lobby lobby, SteamId steamId)
    {
        try
        {
            
            Debug.Log("Join request received for lobby: " + lobby.Id);
            await lobby.Join();
            Debug.Log("Successfully joined lobby via join request.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in GameLobbyJoinRequested: {ex.Message}");
        }
    }
    public void InviteFriend(ulong friendId)
    {
        try
        {
            ulong lobbyId = _currentLobby.Id.Value;
            Debug.Log($"当前房间 ID：{lobbyId}");
            SteamId friendSteamId = friendId;
            Debug.Log($"要邀请的好友 SteamID：{friendSteamId}");

            _currentLobby.InviteFriend(friendSteamId);
            
            Debug.Log($"Invited friend {friendId} to lobby.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error in InviteFriend: {ex.Message}");
        }
    }
    public void SetPublic()
    {
        try
        {
            _currentLobby.SetPublic();
            Debug.Log("Lobby set to public.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error in SetPublic: {ex.Message}");
        }
    }

    public void SetFriendsOnly()
    {
        try
        {
            _currentLobby.SetFriendsOnly();
            Debug.Log("Lobby set to friends only.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error in SetFriendsOnly: {ex.Message}");
        }
    }

    public void SetInvisible()
    {
        try
        {
            _currentLobby.SetInvisible();
            Debug.Log("Lobby set to invisible.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error in SetInvisible: {ex.Message}");
        }
    }

    public void SetGameServer(ulong serverSteamId)
    {
        if (_currentLobby.Id.Value != 0)
        {
            _currentLobby.SetGameServer(serverSteamId);
            Debug.Log($"Lobby associated with game server: {serverSteamId}");
        }
        else
        {
            Debug.LogWarning("No valid lobby to set game server.");
        }
    }
    public void SetMemberData(string key, string value)
    {
          _currentLobby.SetMemberData( key, value);
    }

    public string GetMemberData(ulong steamId, string key)
    {
        return  _currentLobby.GetMemberData(new Friend(steamId), key);;
    }
    public ulong GetGameServer()
    {
        uint ip = 0;
        ushort port = 0;
        SteamId serverId = 0;

        bool success = _currentLobby.GetGameServer( ref ip, ref port, ref serverId);

        if (success)
        {
            return serverId;
        }

        return 0; // 0 表示无效 SteamId
    }
}
