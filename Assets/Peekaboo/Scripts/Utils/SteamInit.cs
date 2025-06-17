using System;
using System.Collections;
// using CocKleBurs.CodeGen.Weaver;
// using CocKleBurs.Console;
// using CocKleBurs.Core;
// using CocKleBurs.Network;
// using CocKleBurs.System;
using Steamworks;
using UnityEngine;

[DefaultExecutionOrder(-5)]
public class SteamInit : NetworkCherry
{
    [SerializeField]
    public static uint AppID ;
    
    protected void BeginPlay()
    {
   
        InitSteamClient(); 
        Debug.Log("Network state initialized");
    }

    private void ProcedureSystemOnOnProcedureChanged(string procedureid, string procedurename)
    {
        SetRichPresenceForProcedure(procedurename);
    }

    public void InitSteamClient()
    {
        try
        {
            if (!SteamClient.IsValid)
            {
                SteamClient.Init(AppID , true);
                Debug.Log("SteamClient init failed");
                base.Orchard.playerid= SteamClient.SteamId;
            }
            StartCoroutine(EnsureValidity());
           
        }
        catch (Exception e)
        {
           OpenSteamSupportPage();
        }
    }
  
    IEnumerator EnsureValidity()
    {
        yield return new WaitUntil(() => SteamClient.IsValid);
    }
    // 根据流程名称设置 Rich Presence
    private void SetRichPresenceForProcedure(string procedureName)
    {
        string richPresenceStatus = $"正在进行: {procedureName}";
       // RichPresenceUtilities.SetRichPresence("status", richPresenceStatus);
    }
    
    private void OpenSteamSupportPage()
    {
// #if !UNITY_EDITOR
//         string steamSupportUrl = Game.GameplaySettings.Steam.steamSupportPage; 
//         Application.OpenURL(steamSupportUrl);
//         Application.Quit();
// #else
      Debug.Log("初始化unity失败");
//#endif
    }
    private  void OnApplicationQuit()
    {
        SteamClient.Shutdown(); 
    }

    private void FixedUpdate()
    {
        SteamClient.RunCallbacks(); 
    }
    /*private void OnDestroy()
    {
        if (SteamClient.IsValid)
            SteamClient.Shutdown();
    }*/
}