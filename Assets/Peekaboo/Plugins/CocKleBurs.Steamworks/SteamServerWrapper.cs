using System;
using System.Net;
using System.Collections.Generic;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

public class SteamServerWrapper : IServer
{
    private string serverName;
    private ushort gamePort;
    private ushort queryPort;
    private string modDir;
    private string versionString;
    private bool isRunning = false;

    private SteamId steamId;
    public Dictionary<SteamId, Connection> Connections = new();

    public void ConfigureServer(string serverName, ushort gamePort, ushort queryPort, string modDir, string versionString)
    {
        this.serverName = serverName;
        this.gamePort = gamePort;
        this.queryPort = queryPort;
        this.modDir = modDir;
        this.versionString = versionString;
    }

    public void InitializeServer()
    {
        SteamServer.Init(2800560, new SteamServerInit
        {
            DedicatedServer = true,
            GameDescription = "CKB Steam Game",
            GamePort = gamePort,
            QueryPort = queryPort,
            ModDir = modDir,
            Secure = true,
            VersionString = versionString,
            IpAddress = IPAddress.Loopback
        }, true);

        if (SteamServer.IsValid)
        {
            SteamServer.ServerName = serverName;
            SteamServer.AdvertiseServer = true;
            SteamServer.LogOnAnonymous();

            steamId = SteamServer.SteamId;
            Debug.Log($"Steam server initialized. ID: {steamId}");
        }
        else
        {
            Debug.LogError("Steam server init failed.");
        }

        RegisterEvents();
    }

    public void StartServer()
    {
        if (!SteamServer.IsValid)
        {
            Debug.LogError("Steam server not initialized.");
            return;
        }

        isRunning = true;
        Debug.Log("Steam server started.");
    }

    public void StopServer()
    {
        if (!SteamServer.IsValid)
            return;

        isRunning = false;

        SteamServer.LogOff();
        SteamServer.Shutdown();
        UnregisterEvents();
        Debug.Log("Steam server stopped.");
    }

    public bool IsServerRunning() => isRunning;

    public void Tick()
    {
        if (isRunning)
        {
            SteamServer.RunCallbacks();
        }
    }

    private void RegisterEvents()
    {
        SteamServer.OnSteamNetAuthenticationStatus += OnAuthStatus;
        SteamServer.OnSteamServersConnected += OnServersConnected;
        SteamServer.OnValidateAuthTicketResponse += OnValidateAuth;
    }

    private void UnregisterEvents()
    {
        SteamServer.OnSteamNetAuthenticationStatus -= OnAuthStatus;
        SteamServer.OnSteamServersConnected -= OnServersConnected;
        SteamServer.OnValidateAuthTicketResponse -= OnValidateAuth;
    }

    private void OnAuthStatus(SteamNetworkingAvailability availability)
    {
        Debug.Log("Steam Auth Status: " + availability);
    }

    private void OnServersConnected()
    {
        Debug.Log("Connected to Steam servers.");
    }

    private void OnValidateAuth(SteamId user, SteamId owner, AuthResponse response)
    {
        Debug.Log($"Auth result: {response} for {user}");

        if (response != AuthResponse.OK && Connections.TryGetValue(user, out var conn))
        {
            conn.Close();
        }
    }
}
