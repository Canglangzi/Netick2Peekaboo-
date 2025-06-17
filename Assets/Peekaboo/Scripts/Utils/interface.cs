using Netick.Unity;
using UnityEngine;

public interface INetickSceneLoadDone
{
    /// <summary>
    /// 场景加载完成时调用的方法。
    /// </summary>
    /// <param name="sandbox">当前网络沙盒实例。</param>
    void OnSceneLoadDone(Orchard sandbox);
}
