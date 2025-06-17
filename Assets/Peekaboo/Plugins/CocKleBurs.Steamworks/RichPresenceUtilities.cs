
using UnityEngine;
using Steamworks;

public static class RichPresenceUtilities
{
    /// <summary>
    /// 为当前用户设置一个丰富状态（Rich Presence）键值对
    /// </summary>
    /// <param name="key">状态的键，例如 "status"、"game"</param>
    /// <param name="value">状态的值，例如 "Playing Level 1"</param>
    /// <returns>返回布尔值，表示设置是否成功</returns>
    public static bool SetRichPresence(string key, string value)
    {
        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
        {
            Debug.Log("Rich Presence 的键和值不能为空！");
            return false;
        }

        try
        {
            bool result = SteamFriends.SetRichPresence(key, value);
            if (result)
            {
//                GameDebug.LogClient($"成功设置 Rich Presence：{key} = {value}");
            }
            else
            {
               Debug.Log($"设置 Rich Presence 失败：{key} = {value}");
            }
            return result;
        }
        catch (System.Exception ex)
        {
           Debug.Log($"设置 Rich Presence 时发生异常：{ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 清除当前用户的所有丰富状态数据
    /// </summary>
    public static void ClearRichPresence()
    {
        try
        {
            SteamFriends.ClearRichPresence();
            Debug.Log("已清除当前用户的所有 Rich Presence 数据。");
        }
        catch (System.Exception ex)
        {
            Debug.Log($"清除 Rich Presence 时发生异常：{ex.Message}");
        }
    }
}