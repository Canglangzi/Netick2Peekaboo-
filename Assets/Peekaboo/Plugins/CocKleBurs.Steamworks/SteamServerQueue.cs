using System;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using Steamworks.Data;using UnityEngine;
using UnityEngine.Serialization;

public class SteamServerQueue :  IServerQueue
{
    private Queue<ulong> queue = new Queue<ulong>(); // 排队的玩家ID
    private int maxPlayers; // 服务器最大玩家数
    private int currentPlayers; // 当前服务器的玩家数
    private Lobby lobby; // Lobby 实例，用于发送通知
    public void Int(Lobby lobby, int maxPlayers)
    {
        this.lobby = lobby;
        this. maxPlayers =maxPlayers;
        this.currentPlayers = 0;
    }
    
    
    // 玩家请求加入队列
    public void JoinQueue(ulong playerId)
    {
        if (currentPlayers < maxPlayers)
        {
            // 服务器有空余位置，直接加入游戏
            AddPlayerToGame(playerId);
        }
        else
        {
            // 服务器满员，加入队列
            queue.Enqueue(playerId);
            SendQueueNotification(playerId, queue.Count);
        }
    }

    // 玩家退出队列
    public void LeaveQueue(ulong playerId)
    {
        if (queue.Contains(playerId))
        {
            queue = new Queue<ulong>(queue.Where(id => id != playerId));
            SendQueueNotification(playerId, queue.Count); // 通知退出的玩家队列位置已更新
        }
    }

    // 获取排队的玩家数量
    public int GetQueueCount() => queue.Count;

    // 向玩家发送排队状态更新通知
    public void SendQueueNotification(ulong playerId, int positionInQueue)
    {
        string message = $"You are in the queue at position {positionInQueue}. Please wait for an available spot.";
        lobby.SendChatString(message); // 向玩家发送排队通知
        Console.WriteLine(message); // 控制台输出
    }

    // 玩家成功进入游戏后发送通知
    public void SendPlayerJoinedNotification(ulong playerId)
    {
        string steamId = "76561198012345678";
        string message = $"#ready:{steamId}";
        lobby.SendChatString(message);
    }

    // 将玩家加入游戏
    private void AddPlayerToGame(ulong playerId)
    {
        currentPlayers++;
        Console.WriteLine($"Player {playerId} joined the game. Current players: {currentPlayers}/{maxPlayers}");

        // 通知玩家他们成功进入游戏
        SendPlayerJoinedNotification(playerId);
    }

    // 获取队列中的所有玩家
    public IEnumerable<ulong> GetQueuePlayers() => queue;
}
