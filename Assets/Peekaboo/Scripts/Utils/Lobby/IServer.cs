public interface IServer
{
    void ConfigureServer(string serverName, ushort gamePort, ushort queryPort, string modDir, string versionString);
    void InitializeServer();     // 初始化底层服务，例如 SteamServer.Init
    void StartServer();          // 启动服务器逻辑（如开放连接、开启游戏流程等）
    void StopServer();           // 停止服务器
    bool IsServerRunning();      // 当前是否运行中
    void Tick();                 // 每帧回调
}