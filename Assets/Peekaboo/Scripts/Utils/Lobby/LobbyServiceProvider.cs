using UnityEngine;
using System;

public class LobbyServiceProvider : NetworkCherry
{
    [SerializeField]
    private MonoBehaviour lobbyServiceBehaviour;

    private ILobbyService _currentLobbyService;

    public ILobbyService CurrentLobbyService => _currentLobbyService;
    private void Awake()
    {
        // 自动从 MonoBehaviour 中获取接口
        if (lobbyServiceBehaviour is ILobbyService service)
        {
            _currentLobbyService = service;
        }
        else
        {
            Debug.LogError("Assigned LobbyServiceBehaviour does not implement ILobbyService.");
        }
    }

    public void SetLobbyService(ILobbyService lobbyService)
    {
        _currentLobbyService = lobbyService;
      
    }
    
    public void CreateLobby(int maxPlayers)
    {
        if (_currentLobbyService != null)
        {
            _currentLobbyService.CreateLobby(maxPlayers);
            base.Orchard.EventLobby.NotifyLobbyCreated(maxPlayers);
        }
        else
        {
            Debug.LogError("LobbyService is not set.");
        }
    }

    public void JoinLobby(string lobbyId)
    {
        if (_currentLobbyService != null)
        {
            _currentLobbyService.JoinLobby(lobbyId);
            base.Orchard.EventLobby.NotifyLobbyJoined(lobbyId);
        }
        else
        {
            Debug.LogError("LobbyService is not set.");
        }
    }

    public void LeaveLobby()
    {
        if (_currentLobbyService != null)
        {
            _currentLobbyService.LeaveLobby();
            base.Orchard.EventLobby.NotifyLobbyLeft();
        }
        else
        {
            Debug.LogError("LobbyService is not set.");
        }
    }

    public void SetLobbyData(string key, string value)
    {
        if (_currentLobbyService != null)
        {
            _currentLobbyService.SetLobbyData(key, value);
            base.Orchard.EventLobby.NotifyLobbyDataSet(key, value);
        }
        else
        {
            Debug.LogError("LobbyService is not set.");
        }
    }

    public void GetLobbyList()
    {
        if (_currentLobbyService != null)
        {
            _currentLobbyService.GetLobbyList();
            base.Orchard.EventLobby.NotifyLobbyListRequested();
        }
        else
        {
            Debug.LogError("LobbyService is not set.");
        }
    }

    public void SetPublic()
    {
        if (_currentLobbyService != null)
        {
            _currentLobbyService.SetPublic();
        }
        else
        {
            Debug.LogError("LobbyService is not set.");
        }
    }

    public void SetFriendsOnly()
    {
        if (_currentLobbyService != null)
        {
            _currentLobbyService.SetFriendsOnly();
        }
        else
        {
            Debug.LogError("LobbyService is not set.");
        }
    }

    public void SetInvisible()
    {
        if (_currentLobbyService != null)
        {
            _currentLobbyService.SetInvisible();
        }
        else
        {
            Debug.LogError("LobbyService is not set.");
        }
    }

    public void InviteFriend(ulong friendId)
    {
        if (_currentLobbyService != null)
        {
            _currentLobbyService.InviteFriend(friendId);
        }
        else
        {
            Debug.LogError("LobbyService is not set.");
        }
    }

    public void SetMemberData(string key, string value)
    {
        if (_currentLobbyService != null)
        {
            _currentLobbyService.SetMemberData(key, value);
        }
        else
        {
            Debug.LogError("LobbyService is not set.");
        }
    }

    public string GetMemberData(ulong steamId, string key)
    {
        if (_currentLobbyService != null)
        {
            return _currentLobbyService.GetMemberData(steamId, key);
        }
        else
        {
            Debug.LogError("LobbyService is not set.");
            return null;
        }
    }

    public void SetGameServer(ulong serverSteamId)
    {
        if (_currentLobbyService != null)
        {
            _currentLobbyService.SetGameServer(serverSteamId);
        }
        else
        {
            Debug.LogError("LobbyService is not set.");
        }
    }

    public ulong GetGameServer()
    {
        if (_currentLobbyService != null)
        {
            return _currentLobbyService.GetGameServer();
        }
        else
        {
            Debug.LogError("LobbyService is not set.");
            return 0;
        }
    }
}
