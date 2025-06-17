using System.Threading.Tasks;
using UnityEngine;
using Steamworks;
using Steamworks.Data; // 确保引入 Steamworks 命名空间

public static class LocalSteamConnection
{
    // 私有静态变量，保存 Steam ID 和用户名称
    private static SteamId _steamId;
    private static string _userName;
    private static  ulong _steamUlongid;
    private static Image? _mePlayerImage;
    private static Texture2D? _mePlayerTexture;

    // 公共静态属性，供外部访问
    public static SteamId Id
    {
        get { return _steamId; }
    }

    public static string UserName
    {
        get { return _userName; }
    }

    public static ulong SteamUlongId
    {
        get { return _steamId.Value; }
    }

    // 设置连接的信息的方法
    public static async Task SetConnection(SteamId id, string name)
    {
        _steamId = id;
        _userName = name;
        
        
       _mePlayerImage = await SteamFriends.GetLargeAvatarAsync(id);
       if (_mePlayerImage != null) _mePlayerTexture = SteamUserUtilities.GetTextureFromImage(_mePlayerImage.Value);
    }
    
    
}
