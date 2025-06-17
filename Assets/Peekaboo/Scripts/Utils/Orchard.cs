using System;
using System.Collections.Generic;
using Netick.Unity;
using UnityEngine;


/// <summary>
/// 世界容器（果园） - 管理所有子系统
/// </summary>
public partial class Orchard : NetworkSandbox
{
    [Header("World")]
    [SerializeField] private Color worldColor = Color.white;

    // 所有子系统（包括管理器）
    [SerializeReference]private List<IManagedSubsystem> allSubsystems = new List<IManagedSubsystem>();
    
    // 类型到子系统的映射（快速查找）
    private Dictionary<Type, List<IManagedSubsystem>> typeLookup = new Dictionary<Type, List<IManagedSubsystem>>();
    
    // 组件到子系统的映射（对于MonoBehaviour）
    private Dictionary<Component, IManagedSubsystem> componentSubsystems = new Dictionary<Component, IManagedSubsystem>();

    private void Awake()
    {
        InitializeSubsystems();
    }

    private void InitializeSubsystems()
    {
        RegisterSubsystems(GetComponents<IManagedSubsystem>());
        
        foreach (Transform child in transform)
        {
            RegisterSubsystems(child.GetComponents<IManagedSubsystem>());
        }
        
        StartAllSubsystems();
    }
    
    private void RegisterSubsystems(IEnumerable<IManagedSubsystem> subsystems)
    {
        foreach (var subsystem in subsystems)
        {
            // 跳过已注册的子系统
            if (allSubsystems.Contains(subsystem)) continue;
            
            allSubsystems.Add(subsystem);
            
            // 如果是MonoBehaviour，添加类型映射
            Component component = subsystem as Component;
            if (component != null)
            {
                componentSubsystems[component] = subsystem;
            }
            
            // 更新类型查找表
            AddToTypeLookup(subsystem.GetType(), subsystem);
            
            // 对于基类接口也需要添加
            Type[] interfaces = subsystem.GetType().GetInterfaces();
            foreach (Type intf in interfaces)
            {
                if (intf != typeof(IManagedSubsystem))
                {
                    AddToTypeLookup(intf, subsystem);
                }
            }
        }
    }
    
    private void AddToTypeLookup(Type type, IManagedSubsystem subsystem)
    {
        if (!typeLookup.TryGetValue(type, out List<IManagedSubsystem> list))
        {
            list = new List<IManagedSubsystem>();
            typeLookup[type] = list;
        }
        
        if (!list.Contains(subsystem))
        {
            list.Add(subsystem);
        }
    }
    
    private void StartAllSubsystems()
    {
        foreach (var subsystem in allSubsystems)
        {
            try
            {
                subsystem.OnStart();
            }
            catch (Exception ex)
            {
                Debug.LogError($"子系统启动失败: {subsystem.GetType().Name} - {ex.Message}");
            }
        }
    }

    /// <summary>
    /// 获取第一个匹配类型的子系统
    /// </summary>
    public T GetSubsystem<T>() where T : class
    {
        Type type = typeof(T);
        if (typeLookup.TryGetValue(type, out List<IManagedSubsystem> subsystems) && subsystems.Count > 0)
        {
            return subsystems[0] as T;
        }
        
        // 尝试获取MonoBehaviour组件作为后备
        if (typeof(Component).IsAssignableFrom(type))
        {
            Component comp = GetComponentInChildren(type, true) as Component;
            if (comp != null)
            {
                var subsystem = comp as IManagedSubsystem;
                if (subsystem != null)
                {
                    RegisterSubsystem(subsystem);
                    return comp as T;
                }
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// 获取所有匹配类型的子系统
    /// </summary>
    public IEnumerable<T> GetSubsystems<T>() where T : class
    {
        Type type = typeof(T);
        if (typeLookup.TryGetValue(type, out List<IManagedSubsystem> subsystems) && subsystems.Count > 0)
        {
            foreach (var subsystem in subsystems)
            {
                yield return subsystem as T;
            }
        }
        else
        {
        }
    }
    
    /// <summary>
    /// 注册新子系统
    /// </summary>
    public void RegisterSubsystem(IManagedSubsystem subsystem)
    {
        if (allSubsystems.Contains(subsystem)) return;
        
        allSubsystems.Add(subsystem);
        
        // 更新类型映射
        AddToTypeLookup(subsystem.GetType(), subsystem);
        
        // 如果是组件，添加到组件映射
        Component component = subsystem as Component;
        if (component != null)
        {
            componentSubsystems[component] = subsystem;
        }
        
        // 立即启动子系统
        try
        {
            subsystem.OnStart();
        }
        catch (Exception ex)
        {
            Debug.LogError($"子系统启动失败: {subsystem.GetType().Name} - {ex.Message}");
        }
    }
    
    /// <summary>
    /// 注销子系统
    /// </summary>
    public void UnregisterSubsystem(IManagedSubsystem subsystem)
    {
        if (!allSubsystems.Contains(subsystem)) return;
        
        // 从主列表移除
        allSubsystems.Remove(subsystem);
        
        // 从组件映射移除
        Component component = subsystem as Component;
        if (component != null && componentSubsystems.ContainsKey(component))
        {
            componentSubsystems.Remove(component);
        }
        
        // 从类型查找表移除
        foreach (var list in typeLookup.Values)
        {
            list.Remove(subsystem);
        }
        
        // 销毁子系统
        try
        {
            subsystem.Destroy();
        }
        catch (Exception ex)
        {
            Debug.LogError($"子系统销毁失败: {subsystem.GetType().Name} - {ex.Message}");
        }
    }

    /// <summary>
    /// 获取世界颜色
    /// </summary>
    public Color GetWorldColor() => worldColor;
    
    
    /// <summary>
    /// 添加新对象到World
    /// </summary>
    public GameObject AddObjectToWorld(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return Instantiate(prefab, position, rotation, transform);
    }
    
    /// <summary>
    /// 从组件获取子系统
    /// </summary>
    public T GetSubsystemFromComponent<T>(Component component) where T : class
    {
        if (componentSubsystems.TryGetValue(component, out IManagedSubsystem subsystem))
        {
            return subsystem as T;
        }
        return null;
    }

    private void OnDestroy()
    {
        // 复制一份列表以防修改
        List<IManagedSubsystem> subsystemsToDestroy = new List<IManagedSubsystem>(allSubsystems);
        
        foreach (var subsystem in subsystemsToDestroy)
        {
            UnregisterSubsystem(subsystem);
        }
        
    }
}
