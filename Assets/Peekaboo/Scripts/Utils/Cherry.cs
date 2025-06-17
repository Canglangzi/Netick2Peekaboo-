using Netick;
using Netick.Unity;
using UnityEngine;

public class Cherry : MonoBehaviour ,INetickSceneLoadDone
{
    public NetworkConnection  localConnection ;
    private Orchard _orchard;
    public virtual Orchard Orchard
    {
        get
        {
            // 尝试使用缓存的值
            if (_orchard != null) return _orchard;
            Debug.LogError($"Sandbox ({ _orchard.GetType().Name}) 不是 Orchard 类型");
            return null;
        }
    }

    public void OnSceneLoadDone(Orchard sandbox)
    {
        _orchard = sandbox;
        localConnection  =  _orchard.LocalPlayer as NetworkConnection;
        OnActive(localConnection);
    }
	
    public virtual void OnActive(NetworkConnection connection) { }

}
