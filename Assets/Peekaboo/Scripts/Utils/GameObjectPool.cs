using System.Collections.Generic;
using UnityEngine;

public struct GameObjectPool 
{
    private GameObject _prefab;
    private Queue<GameObject> _objectPool;
    private int _maxSize;
    
    public GameObjectPool(GameObject prefab, int maxSize = 0)
    {
        _prefab = prefab;
        _maxSize = maxSize;
        _objectPool = new Queue<GameObject>();
        
        // 预创建对象
        int preCreateCount = Mathf.Min(5, maxSize > 0 ? maxSize : 5);
        for (int i = 0; i < preCreateCount; i++)
        {
            GameObject obj = CreateNewPoolObject();
            _objectPool.Enqueue(obj);
        }
    }
    
    public GameObject GetPooledObject(Vector3 position, Quaternion rotation)
    {
        GameObject obj = null;
        
        // 尝试从池中获取对象
        if (_objectPool.Count > 0)
        {
            obj = _objectPool.Dequeue();
        }
        // 如果池中没有对象且未达到最大大小限制，创建新对象
        else if (_maxSize == 0 || _objectPool.Count < _maxSize)
        {
            obj = CreateNewPoolObject();
        }
        
        if (obj != null)
        {
            PrepareObject(obj, position, rotation);
        }
        
        return obj;
    }
    
    public void ReturnToPool(GameObject obj)
    {
        if (obj == null) return;
        
        ResetObjectTransform(obj);
        obj.SetActive(false);
        
        // 如果设置了池大小且当前池大小已满，直接销毁对象
        if (_maxSize > 0 && _objectPool.Count >= _maxSize)
        {
            Object.Destroy(obj);
            return;
        }
        
        _objectPool.Enqueue(obj);
    }
    
    private GameObject CreateNewPoolObject()
    {
        GameObject newObj = Object.Instantiate(_prefab);
        ResetObjectTransform(newObj);
        newObj.SetActive(false);
        return newObj;
    }
    
    private void ResetObjectTransform(GameObject obj)
    {
        obj.transform.SetParent(null);
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
    }
    
    private void PrepareObject(GameObject obj, Vector3 position, Quaternion rotation)
    {
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);
        
        // 调用池化对象的重置接口
        var resettables = obj.GetComponents<IPoolable>();
        foreach (var resettable in resettables)
        {
            resettable.OnSpawn();
        }
    }
}

// 池化对象接口
public interface IPoolable
{
    void OnSpawn();
    void OnReturn();
}