/*using Netick;
using Netick.Unity;
using UnityEngine;
using Unity.Cinemachine;

namespace Peekaboo.GamePlay
{
    public class PlayerCameraSetup : NetworkBehaviour
    {
        [Header("Cinemachine 设置")]
        [SerializeField] private CinemachineCamera _playerFollowCamera;
        [SerializeField] private float _rotationSpeed = 2.0f;
        [SerializeField] private float _pitchClampMin = -89f;
        [SerializeField] private float _pitchClampMax = 89f;
        
        [Networked, Smooth] 
        public float CameraPitch { get; private set; } 
        
        [Networked, Smooth]
        public float CharacterYaw { get; private set; }
        
        private PlayerInput _sandboxInput;
        private float _predictedPitchInput;
        private float _predictedYawInput;
        
        [SerializeField] private Transform _characterTransform; // 玩家角色旋转的Transform
        [SerializeField] private Transform _cameraPivot; // 相机旋转轴

        public override void NetworkStart()
        {
            // 初始化相机系统
            InitializeCameraSystem();
            
            // 初始化旋转角度
            if (IsServer || IsInputSource)
            {
                CameraPitch = _cameraPivot.localEulerAngles.x;
                CharacterYaw = _characterTransform.eulerAngles.y;
            }
            else
            {
                ApplyCameraRotation(CameraPitch);
                ApplyCharacterRotation(CharacterYaw);
            }
        }

        private void InitializeCameraSystem()
        {
            // 确保Cinemachine管理器存在
            if (CinemachineManager.Instance == null)
            {
                Debug.LogError("CinemachineManager 未在场景中找到!");
                return;
            }
            
            // 配置玩家跟随相机
            if (_playerFollowCamera != null)
            {
                CinemachineManager.Instance.SetFollowTarget(_characterTransform);
                CinemachineManager.Instance.SetLookAtTarget(_characterTransform);
                CinemachineManager.Instance.SwitchToCamera(_playerFollowCamera);
            }
            else
            {
                Debug.LogWarning("未分配玩家跟随相机，使用默认相机");
                CinemachineManager.Instance.SwitchToDefaultCamera();
            }
        }

        public override void NetworkUpdate()
        {
            if (!IsInputSource || !Sandbox.InputEnabled)
                return;
            
            // 获取输入
            Vector2 lookInput = _sandboxInput.GetCurrentFrameLookInput();
            
            // 累积预测输入量
            _predictedPitchInput -= lookInput.y * _rotationSpeed;
            _predictedYawInput += lookInput.x * _rotationSpeed;
            
            // 应用预测旋转
            ApplyCameraRotation(ClampPitch(CameraPitch + _predictedPitchInput));
            ApplyCharacterRotation(CharacterYaw + _predictedYawInput);
        }
        
        public override void NetworkFixedUpdate()
        {
            // 处理权威输入
            if (FetchInput(out PlayerInput input))
            {
                _sandboxInput = input;
                
                // 更新俯仰角（垂直旋转）
                CameraPitch = ClampPitch(CameraPitch - input.LookY * _rotationSpeed);
                ApplyCameraRotation(CameraPitch);
                
                // 更新偏航角（水平旋转）
                CharacterYaw += input.LookX * _rotationSpeed;
                ApplyCharacterRotation(CharacterYaw);
            }

            // 重置预测输入
            _predictedPitchInput = 0;
            _predictedYawInput = 0;
        }

        public override void NetworkRender()
        {
            if (!IsProxy)
                return;
            
            ApplyCameraRotation(CameraPitch);
            ApplyCharacterRotation(CharacterYaw);
        }

        [OnChanged(nameof(CameraPitch), invokeDuringResimulation: true)]
        private void OnCameraPitchChanged(OnChangedData changeData)
        {
            if (!IsProxy)
                return;
            
            ApplyCameraRotation(CameraPitch);
        }
        
        [OnChanged(nameof(CharacterYaw), invokeDuringResimulation: true)]
        private void OnCharacterYawChanged(OnChangedData changeData)
        {
            if (!IsProxy)
                return;
            
            ApplyCharacterRotation(CharacterYaw);
        }

        private float ClampPitch(float pitch)
        {
            return Mathf.Clamp(pitch, _pitchClampMin, _pitchClampMax);
        }
        
        private float NormalizeAngle(float angle)
        {
            // 将角度标准化到0-360范围
            angle %= 360;
            if (angle < 0) angle += 360;
            return angle;
        }

        private void ApplyCameraRotation(float pitch) 
        {
            if (_cameraPivot == null)
                return;

            Vector3 newRotation = _cameraPivot.localEulerAngles;
            newRotation.x = pitch;
            _cameraPivot.localEulerAngles = newRotation;
        }
        
        private void ApplyCharacterRotation(float yaw)
        {
            if (_characterTransform == null)
                return;

            Vector3 newRotation = _characterTransform.eulerAngles;
            newRotation.y = yaw;
            _characterTransform.eulerAngles = newRotation;
            
            // 更新Cinemachine相机目标
            if (CinemachineManager.Instance != null)
            {
                CinemachineManager.Instance.SetLookAtTarget(_characterTransform);
            }
        }
        
        // 公开的相机控制API
        public void SetCameraTarget(Transform target)
        {
            if (CinemachineManager.Instance != null)
            {
                CinemachineManager.Instance.SetFollowTarget(target);
                CinemachineManager.Instance.SetLookAtTarget(target);
            }
        }
        
        public void SwitchToCamera(CinemachineCamera camera)
        {
            if (CinemachineManager.Instance != null)
            {
                CinemachineManager.Instance.SwitchToCamera(camera);
            }
        }
        
        public void SwitchToDefaultCamera()
        {
            if (CinemachineManager.Instance != null)
            {
                CinemachineManager.Instance.SwitchToDefaultCamera();
            }
        }
        
        public void SetCameraFOV(float fov)
        {
            if (CinemachineManager.Instance != null)
            {
                CinemachineManager.Instance.SetFieldOfView(fov);
            }
        }
        
        public void ResetCameraRotation()
        {
            CameraPitch = 0;
            CharacterYaw = _characterTransform.eulerAngles.y;
            ApplyCameraRotation(CameraPitch);
            ApplyCharacterRotation(CharacterYaw);
        }
    }
}*/