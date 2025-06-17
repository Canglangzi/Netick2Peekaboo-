using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class CKBSteamAPI
{
    #region Get User Info

    public static void GetSteamUserInfo(MonoBehaviour context, ulong steamId, Action<SteamUserInfo> onSuccess, Action<string> onError = null)
    {
        string url = $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={SteamInit.AppID}&steamids={steamId}";
        context.StartCoroutine(Fetch(url, (json) =>
        {
            var root = JsonUtility.FromJson<PlayerSummaryWrapper>(json);
            if (root.response.players.Length > 0)
            {
                var player = root.response.players[0];
                onSuccess?.Invoke(new SteamUserInfo
                {
                    PersonaName = player.personaname,
                    AvatarSmallUrl = player.avatar,
                    AvatarMediumUrl = player.avatarmedium,
                    AvatarFullUrl = player.avatarfull
                });
            }
            else
            {
                onError?.Invoke("No player found.");
            }
        }, onError));
    }

    public static void DownloadAvatar(MonoBehaviour context, string url, Action<Texture2D> onSuccess, Action<string> onError = null)
    {
        context.StartCoroutine(DownloadImage(url, onSuccess, onError));
    }

    #endregion

    #region Get Friends

    public static void GetFriendList(MonoBehaviour context, ulong steamId, Action<Friend[]> onSuccess, Action<string> onError = null)
    {
        string url = $"https://api.steampowered.com/ISteamUser/GetFriendList/v1/?key={ SteamInit.AppID}&steamid={steamId}&relationship=friend";
        context.StartCoroutine(Fetch(url, (json) =>
        {
            var root = JsonUtility.FromJson<FriendListWrapper>(json);
            onSuccess?.Invoke(root.friendslist.friends);
        }, onError));
    }

    #endregion

    #region Get Achievements

    public static void GetPlayerAchievements(MonoBehaviour context, ulong steamId, uint appId, Action<Achievement[]> onSuccess, Action<string> onError = null)
    {
        string url = $"https://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v1/?key={SteamInit.AppID}&steamid={steamId}&appid={appId}";
        context.StartCoroutine(Fetch(url, (json) =>
        {
            var root = JsonUtility.FromJson<AchievementsWrapper>(json);
            onSuccess?.Invoke(root.playerstats.achievements ?? new Achievement[0]);
        }, onError));
    }

    #endregion

    #region Internals

    private static IEnumerator Fetch(string url, Action<string> onSuccess, Action<string> onError)
    {
        UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (req.result != UnityWebRequest.Result.Success)
#else
        if (req.isNetworkError || req.isHttpError)
#endif
        {
            onError?.Invoke($"Request Error: {req.error}");
        }
        else
        {
            onSuccess?.Invoke(req.downloadHandler.text);
        }
    }

    private static IEnumerator DownloadImage(string url, Action<Texture2D> onSuccess, Action<string> onError = null)
    {
        UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);
        yield return req.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (req.result != UnityWebRequest.Result.Success)
#else
        if (req.isNetworkError || req.isHttpError)
#endif
        {
            onError?.Invoke($"Image download failed: {req.error}");
        }
        else
        {
            Texture2D tex = DownloadHandlerTexture.GetContent(req);
            onSuccess?.Invoke(tex);
        }
    }

    #endregion

    #region Data Structures

    [Serializable] public class SteamUserInfo
    {
        public string PersonaName;
        public string AvatarSmallUrl;
        public string AvatarMediumUrl;
        public string AvatarFullUrl;
    }

    [Serializable] private class PlayerSummaryWrapper { public PlayerSummaryResponse response; }
    [Serializable] private class PlayerSummaryResponse { public Player[] players; }
    [Serializable] private class Player
    {
        public string personaname;
        public string avatar;
        public string avatarmedium;
        public string avatarfull;
    }

    [Serializable] private class FriendListWrapper { public FriendList friendslist; }
    [Serializable] private class FriendList { public Friend[] friends; }
    [Serializable] public class Friend
    {
        public string steamid;
        public string relationship;
        public string friend_since;
    }

    [Serializable] private class AchievementsWrapper { public AchievementStats playerstats; }
    [Serializable] private class AchievementStats
    {
        public string steamID;
        public Achievement[] achievements;
    }

    [Serializable] public class Achievement
    {
        public string apiname;
        public int achieved;
        public string unlocktime;
    }

    #endregion
}
