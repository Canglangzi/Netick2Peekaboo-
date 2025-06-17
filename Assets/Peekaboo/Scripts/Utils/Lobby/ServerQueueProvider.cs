using System;
using System.Collections.Generic;
using UnityEngine;

public class ServerQueueProvider : MonoBehaviour
{
    private IServerQueue serverQueue; // 排队服务接口
    private int maxPlayers; // 最大玩家数
    public static event Action<ulong, int> OnQueuePositionUpdated; 
    public void JoinQueue(ulong playerId)
    {
        if (serverQueue == null)
        {
            Debug.LogError("ServerQueue is not initialized.");
            return;
        }

        serverQueue.JoinQueue(playerId);
    }

    // 玩家退出队列
    public void LeaveQueue(ulong playerId)
    {
        if (serverQueue == null)
        {
            Debug.LogError("ServerQueue is not initialized.");
            return;
        }

        serverQueue.LeaveQueue(playerId);
    }

    // 获取排队数量
    public int GetQueueCount()
    {
        return serverQueue != null ? serverQueue.GetQueueCount() : 0;
    }

    // 获取排队中的所有玩家
    public IEnumerable<ulong> GetQueuePlayers()
    {
        return serverQueue != null ? serverQueue.GetQueuePlayers() : null;
    }

    // 更新玩家排队通知
    public void UpdateQueueNotifications()
    {
        foreach (var playerId in serverQueue.GetQueuePlayers())
        {
            int position = serverQueue.GetQueueCount(); // 假设队列按顺序排列
            serverQueue.SendQueueNotification(playerId, position);
            OnQueuePositionUpdated?.Invoke(playerId, position);
        }
    }

    // 玩家成功加入游戏时的通知
    public void NotifyPlayerJoinedGame(ulong playerId)
    {
        if (serverQueue != null)
        {
            serverQueue.SendPlayerJoinedNotification(playerId);
        }
    }
}