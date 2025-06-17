using System.Collections.Generic;

public interface IServerQueue
{
    // 玩家请求加入队列
    void JoinQueue(ulong playerId);

    // 玩家退出队列
    void LeaveQueue(ulong playerId);

    // 获取排队的玩家数量
    int GetQueueCount();

    // 向玩家发送排队状态更新通知
    void SendQueueNotification(ulong playerId, int positionInQueue);

    // 玩家成功进入游戏后发送通知
    void SendPlayerJoinedNotification(ulong playerId);

    // 获取队列中的所有玩家（如果需要）
    IEnumerable<ulong> GetQueuePlayers();
}