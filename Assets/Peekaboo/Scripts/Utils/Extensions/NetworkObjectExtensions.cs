using UnityEngine;
using System.Collections.Generic;
using Netick.Unity;

public static class NetworkObjectExtensions
{
    /// <summary>
    /// 获取对象本身的 NetworkObject 组件。
    /// </summary>
    /// <param name="gameObject">要查找的 GameObject。</param>
    /// <returns>找到的 NetworkObject 组件，如果没有找到则返回 null。</returns>
    public static NetworkObject GetNetworkObject(this GameObject gameObject)
    {
        return gameObject.GetComponent<NetworkObject>();
    }

    /// <summary>
    /// 获取对象本身或其父级中的 NetworkObject 组件。
    /// </summary>
    /// <param name="transform">要查找的 Transform。</param>
    /// <returns>找到的 NetworkObject 组件，如果没有找到则返回 null。</returns>
    public static NetworkObject GetNetworkObjectInParent(this Transform transform)
    {
        return transform.GetComponentInParent<NetworkObject>();
    }

    /// <summary>
    /// 获取对象本身或其子级中的 NetworkObject 组件。
    /// </summary>
    /// <param name="transform">要查找的 Transform。</param>
    /// <returns>找到的 NetworkObject 组件，如果没有找到则返回 null。</returns>
    public static NetworkObject GetNetworkObjectInChildren(this Transform transform)
    {
        return transform.GetComponentInChildren<NetworkObject>();
    }

    /// <summary>
    /// 获取对象本身、其父级或其子级中的 NetworkObject 组件。
    /// </summary>
    /// <param name="transform">要查找的 Transform。</param>
    /// <returns>找到的 NetworkObject 组件，如果没有找到则返回 null。</returns>
    public static NetworkObject GetNetworkObjectAnywhere(this Transform transform)
    {
        // 先在自身查找
        NetworkObject nob = transform.GetComponent<NetworkObject>();
        if (nob != null) return nob;

        // 再在父级查找
        nob = transform.GetComponentInParent<NetworkObject>();
        if (nob != null) return nob;

        // 最后在子级查找
        return transform.GetComponentInChildren<NetworkObject>();
    }

    /// <summary>
    /// 在给定的位置获取 NetworkObject 组件。
    /// </summary>
    /// <param name="position">要查找的位置。</param>
    /// <param name="radius">查找的半径。</param>
    /// <returns>找到的 NetworkObject 组件，如果没有找到则返回 null。</returns>
    public static NetworkObject GetNetworkObjectAtPosition(Vector3 position, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(position, radius);
        foreach (var collider in colliders)
        {
            NetworkObject nob = collider.GetComponent<NetworkObject>();
            if (nob != null)
            {
                return nob;
            }
        }
        return null;
    }

    /// <summary>
    /// 通过标签获取对象及其子级的 NetworkObject 组件。
    /// </summary>
    /// <param name="transform">要查找的 Transform。</param>
    /// <param name="tag">要查找的标签。</param>
    /// <returns>找到的 NetworkObject 组件，如果没有找到则返回 null。</returns>
    public static NetworkObject GetNetworkObjectWithTag(this Transform transform, string tag)
    {
        if (transform.CompareTag(tag))
        {
            return transform.GetComponent<NetworkObject>();
        }

        foreach (Transform child in transform)
        {
            NetworkObject result = child.GetNetworkObjectWithTag(tag);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }

    /// <summary>
    /// 获取对象及其子级中所有的 NetworkObject 组件。
    /// </summary>
    /// <param name="transform">要查找的 Transform。</param>
    /// <returns>找到的所有 NetworkObject 组件列表。</returns>
    public static List<NetworkObject> GetAllNetworkObjectsInChildren(this Transform transform)
    {
        List<NetworkObject> networkObjects = new List<NetworkObject>();
        GetAllNetworkObjectsInChildrenRecursive(transform, networkObjects);
        return networkObjects;
    }

    private static void GetAllNetworkObjectsInChildrenRecursive(Transform transform, List<NetworkObject> networkObjects)
    {
        NetworkObject nob = transform.GetComponent<NetworkObject>();
        if (nob != null)
        {
            networkObjects.Add(nob);
        }

        foreach (Transform child in transform)
        {
            GetAllNetworkObjectsInChildrenRecursive(child, networkObjects);
        }
    }

    /// <summary>
    /// 通过层级获取对象及其子级的 NetworkObject 组件。
    /// </summary>
    /// <param name="transform">要查找的 Transform。</param>
    /// <param name="layer">要查找的层级。</param>
    /// <returns>找到的 NetworkObject 组件，如果没有找到则返回 null。</returns>
    public static NetworkObject GetNetworkObjectInLayer(this Transform transform, int layer)
    {
        if (transform.gameObject.layer == layer)
        {
            return transform.GetComponent<NetworkObject>();
        }

        foreach (Transform child in transform)
        {
            NetworkObject result = child.GetNetworkObjectInLayer(layer);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }
}