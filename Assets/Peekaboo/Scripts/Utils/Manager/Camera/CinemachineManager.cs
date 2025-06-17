using System;
using UnityEngine;
using Unity.Cinemachine;

public class CinemachineManager : Cherry
{
    // 单例实例
    public static CinemachineManager Instance { get; private set; }

    // 当前激活的虚拟相机
    private CinemachineCamera currentVirtualCamera;
    
    [Header("Cinemachine Cameras")]
    [SerializeField] private CinemachineCamera[] virtualCameras;
    [SerializeField] private CinemachineCamera defaultVirtualCamera;
    public Transform cameraTarget;
    
    private void Awake()
    {
 
        // 单例模式：确保只存在一个实例
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 销毁重复的实例
            return;
        }
        Instance = this; // 设置当前实例为单例

        // 保持在场景切换时不被销毁
      //  DontDestroyOnLoad(gameObject);

        // 初始化默认相机
        if (defaultVirtualCamera != null)
        {
            SwitchToCamera(defaultVirtualCamera);
        }
    }
    

    // 切换到指定的虚拟相机
    public void SwitchToCamera(CinemachineCamera newCamera)
    {
        if (currentVirtualCamera != null)
        {
            currentVirtualCamera.gameObject.SetActive(false); // 禁用当前相机
        }

        currentVirtualCamera = newCamera;
        currentVirtualCamera.gameObject.SetActive(true); // 启用新的虚拟相机
    }

    // 切换到指定索引的虚拟相机
    public void SwitchToCamera(int index)
    {
        if (index >= 0 && index < virtualCameras.Length)
        {
            SwitchToCamera(virtualCameras[index]);
        }
        else
        {
            Debug.LogWarning("Cinemachine camera index out of bounds.");
        }
    }

    // 获取当前激活的虚拟相机
    public CinemachineCamera GetCurrentVirtualCamera()
    {
        return currentVirtualCamera;
    }

    // 配置虚拟相机的跟随目标
    public void SetFollowTarget(Transform target)
    {
        if (currentVirtualCamera != null)
        {
            currentVirtualCamera.Follow = target;
            currentVirtualCamera.LookAt = target;
        }
    }

    // 配置虚拟相机的镜头属性，例如视场角（FOV）
    public void SetFieldOfView(float fov)
    {
        if (currentVirtualCamera != null)
        {
            var lens = currentVirtualCamera.Lens;
            lens.FieldOfView = fov;
            currentVirtualCamera.Lens = lens;
        }
    }

    // 配置虚拟相机的镜头属性（例如，远近裁剪面）
    public void SetNearClipPlane(float nearClip)
    {
        if (currentVirtualCamera != null)
        {
            var lens = currentVirtualCamera.Lens;
            lens.NearClipPlane = nearClip;
            currentVirtualCamera.Lens = lens;
        }
    }

    // 配置虚拟相机的镜头属性（例如，远裁剪面）
    public void SetFarClipPlane(float farClip)
    {
        if (currentVirtualCamera != null)
        {
            var lens = currentVirtualCamera.Lens;
            lens.FarClipPlane = farClip;
            currentVirtualCamera.Lens = lens;
        }
    }
    
    // 配置虚拟相机的观察目标
    public void SetLookAtTarget(Transform target)
    {
        if (currentVirtualCamera != null)
        {
            currentVirtualCamera.LookAt = target;
        }
    }

    // 配置默认虚拟相机的跟随目标
    public void SetDefaultFollowTarget(Transform target)
    {
        if (defaultVirtualCamera != null)
        {
            defaultVirtualCamera.Follow = target;
        }
    }

    // 配置默认虚拟相机的观察目标
    public void SetDefaultLookAtTarget(Transform target)
    {
        if (defaultVirtualCamera != null)
        {
            defaultVirtualCamera.LookAt = target;
        }
    }

    public void SwitchToDefaultCamera()
    {
        if (defaultVirtualCamera != null)
        {
            SwitchToCamera(defaultVirtualCamera);
        }
        else
        {
            Debug.LogWarning("Default Cinemachine camera is not assigned.");
        }
    }
}
