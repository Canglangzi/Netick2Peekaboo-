/*
using System;
using UnityEngine;
using Steamworks;
using CocKleBurs.Core;
using UnityEngine.SceneManagement;

/// <summary>
/// 处理 Steam 时间轴的测试脚本。
/// 
/// 该脚本初始化 Steam API，设置游戏时间轴模式，并监听特定的游戏事件。根据接收到的事件，
/// 它会向 Steam 时间轴添加相应的事件。事件包括玩家的受伤、治疗和回合的开始和结束。
/// 
/// 事件处理包括：
///
/// - Onplayer_incapacitated_start: 当玩家被击倒时触发，记录相关事件到时间轴。
/// - Onheal_success: 当玩家成功治疗时触发，记录治疗事件到时间轴。
/// - Onsurvival_round_start: 当游戏回合开始时触发，设置时间轴模式为 Playing。
/// - Onround_end: 当回合结束时触发，设置时间轴模式为 Staging。
/// 
/// 注意：
/// 1. 确保在游戏退出时正确移除事件监听器并关闭 Steam 客户端。
/// 2. Steam API 的初始化需要有效的应用 ID 和 Steam 客户端。
/// </summary>
public class SteamTimelineTest : MonoBehaviour
{
    private bool _enabled;
    private int _userid;
  
    private void Start()
    {
        try
        {
            // 初始化 Steam 客户端，应用 ID 480 代表测试应用
            SteamClient.Init(480, true);
            
            // 设置 Steam 时间轴的初始游戏模式为 Staging（预演模式）
            SteamTimeline.SetTimelineGameMode(TimelineGameMode.Staging);
            
            // 获取用户 ID
            _userid = Game.GetInt(DataGen.Userid);
            
            // 注册事件监听器
            Game.ListenForEvent(EventGen.survival_round_start, Onsurvival_round_start)
                .ListenForEvent(EventGen.heal_success, Onheal_success)
                .ListenForEvent(EventGen.round_end, Onround_end)
                .ListenForEvent(EventGen.player_incapacitated_start, Onplayer_incapacitated_start);
                  
            _enabled = true;
            Debug.Log("初始化成功");
        }
        catch (Exception e)
        {
            Debug.LogError("初始化 Steam API 失败: " + e.Message);
            _enabled = false;
        }
    }

    /// <summary>
    /// 处理玩家被击倒事件，记录相关信息到时间轴。
    /// </summary>
    private void Onplayer_incapacitated_start()
    { 
        string attackerDebug = Game.GetInt(DataGen.AttackerID).ToString() +
                               Game.GetString(DataGen.AttackerDisplayName);
        
        SteamTimeline.AddTimelineEvent(
            "foo", // 图标
            "Incapacitated", // 标题
            $"被 {attackerDebug} 击倒", // 描述
            0, // 优先级
            0f, // 开始偏移时间（秒）
            0f, // 持续时间（秒）
            TimelineEventClipPriority.Featured // 事件优先级
        ); 
        
        Debug.Log("玩家被击倒事件触发成功。");
        Debug.Log($"事件描述: 被 {attackerDebug} 击倒");
    }

    /// <summary>
    /// 处理玩家治疗成功事件，记录相关信息到时间轴。
    /// </summary>
    public void Onheal_success()
    {
        SteamTimeline.AddTimelineEvent(
            "medkit32",               // 图标
            "Healed",                // 标题
            $"恢复了 {Game.GetInt(DataGen.HealthRestored)} 生命值", // 描述
            0,                       // 优先级
        -5f,                     // 开始偏移时间（秒）
            5f,                      // 持续时间（秒）
            TimelineEventClipPriority.Standard // 事件优先级
        );
        
        Debug.Log("玩家治疗成功事件触发成功。");
        Debug.Log($"事件描述: 恢复了 {Game.GetInt(DataGen.HealthRestored)} 生命值");
    }

    /// <summary>
    /// 处理回合开始事件，设置时间轴模式为 Playing。
    /// </summary>
    public void Onsurvival_round_start()
    {
        SteamTimeline.SetTimelineGameMode(TimelineGameMode.Playing);
        
        Debug.Log("回合开始事件触发成功。时间轴模式设置为 Playing。");
    }

    /// <summary>
    /// 处理回合结束事件，设置时间轴模式为 Staging。
    /// </summary>
    private void Onround_end()
    {
        SteamTimeline.SetTimelineGameMode(TimelineGameMode.Staging);
        
        Debug.Log("回合结束事件触发成功。时间轴模式设置为 Staging。");
    }

    private void Update()
    {
        // 检测按键触发事件
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("触发玩家被击倒事件。");
            Onplayer_incapacitated_start();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("触发玩家治疗成功事件。");
            Onheal_success();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("触发回合开始事件。");
            Onsurvival_round_start();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("触发回合结束事件。");
            Onround_end();
        }
    }

    private void OnDestroy()
    {
        if (_enabled)
        {
            // 移除事件监听器
            Game.RemoveListener(EventGen.survival_round_start, Onsurvival_round_start)
                .RemoveListener(EventGen.heal_success, Onheal_success)
                .RemoveListener(EventGen.round_end, Onround_end)
                .RemoveListener(EventGen.player_incapacitated_start, Onplayer_incapacitated_start);
            
            // 关闭 Steam 客户端
            SteamClient.Shutdown();
            Debug.Log("Steam 客户端已关闭。");
        }
    }
}
*/
