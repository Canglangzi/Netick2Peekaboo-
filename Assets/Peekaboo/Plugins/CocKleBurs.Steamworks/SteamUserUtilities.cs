using System.Threading.Tasks;
using UnityEngine;
using Steamworks;
using Steamworks.Data;

public class SteamUserUtilities
{
    /// <summary>
    /// 获取指定用户的头像（小尺寸）
    /// </summary>
    /// <param name="steamId">用户的 SteamId</param>
    /// <returns>返回包含头像数据的 Data.Image 对象</returns>
    public static async Task<Image?> GetSmallAvatarAsync(SteamId steamId)
    {
        try
        {
            Debug.Log($"正在获取用户 {steamId} 的小头像...");
             Image? avatar = await SteamFriends.GetSmallAvatarAsync(steamId);
            if (avatar != null)
            {
                Debug.Log($"成功获取用户 {steamId} 的小头像。");
                return avatar;
            }
            else
            {
               Debug.Log($"用户 {steamId} 没有小头像。");
                return null;
            }
        }
        catch (System.Exception ex)
        {
              Debug.Log($"获取用户 {steamId} 小头像时出错: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 检查当前用户是否关注指定的 Steam 用户
    /// </summary>
    /// <param name="steamId">目标用户的 SteamId</param>
    /// <returns>返回布尔值，表示是否关注</returns>
    public static async Task<bool> IsFollowing(SteamId steamId)
    {
        try
        {
            Debug.Log($"检查当前用户是否关注 {steamId}...");
            bool isFollowing = await SteamFriends.IsFollowing(steamId);
            Debug.Log(isFollowing
                ? $"当前用户已关注 {steamId}。"
                : $"当前用户未关注 {steamId}。");
            return isFollowing;
        }
        catch (System.Exception ex)
        {
            Debug.Log($"检查是否关注用户 {steamId} 时出错: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 激活 Steam 界面以打开邀请对话框
    /// </summary>
    /// <param name="lobbyId">目标大厅的 SteamId</param>
    public static void OpenGameInviteOverlay(SteamId lobbyId)
    {
        try
        {
            Debug.Log($"尝试打开大厅 {lobbyId} 的邀请对话框...");
            SteamFriends.OpenGameInviteOverlay(lobbyId);
            Debug.Log($"成功打开大厅 {lobbyId} 的邀请对话框。");
        }
        catch (System.Exception ex)
        {
             Debug.Log($"打开大厅 {lobbyId} 的邀请对话框时出错: {ex.Message}");
        }
    }
    
    // 异步方法，接受Steam用户ID并返回Texture2D
    public static async Task<Texture2D> GetPlayerAvatarTextureAsync(Steamworks.SteamId id)
    {
        // 获取玩家的头像
        var image = await SteamFriends.GetLargeAvatarAsync(id);

        // 如果获取到了头像，转为Texture2D并返回
        if (image != null)
        {
            return GetTextureFromImage(image.Value);
        }

        // 如果没有获取到头像，返回空的Texture2D
        return null;
    }
    public static  Texture2D GetTextureFromImage(Steamworks.Data.Image image)
    {

        var texture = new Texture2D((int)image.Width, (int)image.Height);
        for (int x = 0; x < image.Width; x++)
        {
            for (int y = 0; y < image.Height; y++)
            {
                var p = image.GetPixel(x, (int)(image.Height - y - 1)); // Adjust y-coordinate
                texture.SetPixel(x, y, new UnityEngine.Color(p.r / 255.0f, p.g / 255.0f, p.b / 255.0f, p.a / 255.0f));
            }
        }
        texture.Apply();
        return texture;
    }
}
