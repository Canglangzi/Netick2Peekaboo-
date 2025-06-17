using System.Collections.Generic;
//using CocKleBurs.Console;
using Netick.Unity;
using UnityEngine;

public class SandboxRegistry : MonoBehaviour
{
    // 单例实例
    private static SandboxRegistry _instance;

    // 所有沙盒的列表
    [SerializeField]private List<NetworkSandbox> _sandboxes = new List<NetworkSandbox>();

    // 获取单例实例
    public static SandboxRegistry Instance
    {
        get
        {
            if (_instance == null)
            {
                // 如果单例还没有实例化，则在场景中查找
                _instance = FindObjectOfType<SandboxRegistry>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("SandboxRegistry");
                    _instance = obj.AddComponent<SandboxRegistry>();

                    // 设置为不销毁，确保在场景切换中持久化
                    DontDestroyOnLoad(obj);
                }
                else
                {
                    // 防止重复添加的情况
                    DontDestroyOnLoad(_instance.gameObject);
                }
            }
            return _instance;
        }
    }
    public Orchard  DefaultSandbox;


    // 注册沙盒
    public virtual void RegisterSandbox(NetworkSandbox sandbox)
    {
        if (!_sandboxes.Contains(sandbox))
        {
            _sandboxes.Add(sandbox);
           Debug.Log("Sandbox registered: " + sandbox.Name); // 可以添加日志记录注册沙盒
        }
    }

    // 注销沙盒
    public virtual void UnregisterSandbox(NetworkSandbox sandbox)
    {
        if (_sandboxes.Contains(sandbox))
        {
            _sandboxes.Remove(sandbox);
           Debug.Log("Sandbox unregistered: " + sandbox.Name); // 可以添加日志记录注销沙盒
        }
    }

    // 获取所有沙盒
    public List<NetworkSandbox> GetAllSandboxes()
    {
        return _sandboxes;
    }

    // 通过索引获取沙盒
    public NetworkSandbox GetSandboxByIndex(int index)
    {
        return _sandboxes.Count > index ? _sandboxes[index] : null;
    }

    // 清除所有沙盒
    public virtual void ClearSandboxes()
    {
        _sandboxes.Clear();
       Debug.Log("All sandboxes cleared."); // 可以添加日志记录清除所有沙盒
    }
}
