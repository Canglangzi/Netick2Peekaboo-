using Steamworks;

namespace CocKleBurs.Steamworks
{
    ///<summary>
    /// 表示用户或 Steam 大厅的 ID。
    ///</summary>
    public struct SteamId
    {
        /// <summary>
        /// 表示 Steam ID 的底层 ulong 值。
        /// </summary>
        public ulong Value;

        /// <summary>
        /// 从 ulong 类型隐式转换为 SteamId 结构体。
        /// </summary>
        /// <param name="value">要转换的 ulong 值。</param>
        /// <returns>返回一个 SteamId 结构体。</returns>
        public static implicit operator SteamId(ulong value) => new SteamId() { Value = value };

        /// <summary>
        /// 从 SteamId 隐式转换为 ulong 类型。
        /// </summary>
        /// <param name="value">要转换的 SteamId 结构体。</param>
        /// <returns>返回 SteamId 的底层 ulong 值。</returns>
        public static implicit operator ulong(SteamId value) => value.Value;

        /// <summary>
        /// 将 SteamId 转换为字符串表示形式。
        /// </summary>
        /// <returns>返回 SteamId 的字符串表示形式。</returns>
        public override string ToString() => this.Value.ToString();

        /// <summary>
        /// 从 SteamId 中提取账号 ID。
        /// </summary>
        public uint AccountId => (uint)(this.Value & (ulong)uint.MaxValue);

        /// <summary>
        /// 检查 SteamId 是否有效（大于 0）。
        /// </summary>
        public bool IsValid => this.Value > 0UL;
    }
}
