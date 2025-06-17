
using UnityEngine;
using Steamworks;
using Steamworks.Data;

public class SteamUserCallback: MonoBehaviour
{
    private static SteamUserCallback _instance;

    private void Start()
    {
        InitializeSteamCallbacks();
    }

    private void OnDestroy()
    {
        UnregisterSteamCallbacks();
    }

    /// <summary>
    /// 注册 SteamUser 
    /// </summary>
    private void InitializeSteamCallbacks()
    {
       // GameDebug.LogClient("Registering SteamUser callbacks...");

        SteamUser.OnClientGameServerDeny += HandleGameServerDeny;
        SteamUser.OnDurationControl += HandleDurationControl;
        SteamUser.OnGameWebCallback += HandleGameWebCallback;
        SteamUser.OnMicroTxnAuthorizationResponse += HandleMicroTxnAuthorization;
        SteamUser.OnValidateAuthTicketResponse += HandleAuthTicketResponse;

        SteamUser.OnSteamServersConnected += HandleServersConnected;
        SteamUser.OnSteamServerConnectFailure += HandleConnectFailure;
        SteamUser.OnSteamServersDisconnected += HandleServersDisconnected;
    }

    /// <summary>
    /// 注销 SteamUser 的事件回调
    /// </summary>
    private void UnregisterSteamCallbacks()
    {
      //  GameDebug.LogClient("Unregistering SteamUser callbacks...");

        SteamUser.OnClientGameServerDeny -= HandleGameServerDeny;
        SteamUser.OnDurationControl -= HandleDurationControl;
        SteamUser.OnGameWebCallback -= HandleGameWebCallback;
        SteamUser.OnMicroTxnAuthorizationResponse -= HandleMicroTxnAuthorization;
        SteamUser.OnValidateAuthTicketResponse -= HandleAuthTicketResponse;

        SteamUser.OnSteamServersConnected -= HandleServersConnected;
        SteamUser.OnSteamServerConnectFailure -= HandleConnectFailure;
        SteamUser.OnSteamServersDisconnected -= HandleServersDisconnected;
    }

    #region Callback Handlers

    private void HandleGameServerDeny()
    {
      //  GameDebug.LogClient("Connection to the game server was denied by Steam. Disconnecting...");
        // 可扩展：调用游戏逻辑断开服务器连接
    }

    private void HandleDurationControl(DurationControl durationControl)
    {
       // GameDebug.LogClient($"Duration control triggered. Current rate: {durationControl.Appid}");
        // 根据 data.rate 控制奖励或其他行为
    }

    private void HandleGameWebCallback(string url)
    {
       // GameDebug.LogClient($"Steam game web callback received. URL: {url}");
        // 可扩展：处理外部网站注册或其他逻辑
    }

    private void HandleMicroTxnAuthorization(AppId appId, ulong orderId, bool userAuthorized)
    {
        if (userAuthorized)
        {
          //  GameDebug.LogClient($"Microtransaction authorized. AppId: {appId}, OrderId: {orderId}");
            // 可扩展：添加处理用户成功购买的逻辑
        }
        else
        {
          //  GameDebug.LogClient($"Microtransaction NOT authorized. AppId: {appId}, OrderId: {orderId}");
        }
    }

    private void HandleServersConnected()
    {
       // GameDebug.LogClient("Successfully connected to Steam servers.");
        // 可扩展：进入游戏主界面等逻辑
    }

    private void HandleConnectFailure()
    {
       // GameDebug.LogClient("Failed to connect to Steam servers. Retrying...");
        // 可扩展：执行重试逻辑
    }

    private void HandleServersDisconnected()
    {
      //  GameDebug.LogClient("Disconnected from Steam servers.");
        // 可扩展：提示用户检查网络等逻辑
    }

    private void HandleAuthTicketResponse(SteamId userId, SteamId ownerId, AuthResponse response)
    {
       // GameDebug.LogClient($"Auth ticket response received. User: {userId}, Owner: {ownerId}, Response: {response}");

        if (response == AuthResponse.OK)
        {
           // GameDebug.LogClient("Authorization successful.");
        }
        else
        {
          //  GameDebug.LogClient($"Authorization failed with response: {response}");
        }
    }

    #endregion
}
