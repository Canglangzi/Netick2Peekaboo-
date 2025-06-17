public interface IServerQueueListener
{
    void OnQueuePositionUpdated(ulong playerId, int position);
    void OnPlayerJoinedGame(ulong playerId);
}