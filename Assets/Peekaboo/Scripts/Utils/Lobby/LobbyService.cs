public interface ILobbyService
{
    void CreateLobby(int maxPlayers);
    void JoinLobby(string lobbyId);
    void LeaveLobby();
    void SetLobbyData(string key, string value);
    string GetLobbyData(string key);
    void GetLobbyList();

    ulong LobbyId { get; set; }
    int MaxPlayers { get; }
    int CurrentPlayers { get; }
    bool IsInLobby { get; }

    void SetPublic();
    void SetFriendsOnly();
    void SetInvisible();
    void InviteFriend(ulong friendId);

    void SetMemberData(string key, string value);
    string GetMemberData(ulong steamId, string key);
    void SetGameServer(ulong serverSteamId);
    ulong GetGameServer();
}