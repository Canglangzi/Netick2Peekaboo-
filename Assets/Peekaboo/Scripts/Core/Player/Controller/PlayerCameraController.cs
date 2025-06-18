using Netick;
using Netick.Unity;
using UnityEngine;

namespace Peekaboo.GamePlay
{
    public class PlayerCameraController : NetworkBehaviour
    {
        [Header("旋转设置")]
        [SerializeField] private float _rotationSpeed = 2.0f;
        [SerializeField] private float _pitchClampMin = -89f;
        [SerializeField] private float _pitchClampMax = 89f;
        [SerializeField] private float _smoothingFactor = 15f; // 新增平滑因子
        
        [Networked] 
        public float CameraPitch { get; private set; } 
        
        [Networked]
        public float CharacterYaw { get; private set; }
        
        private PlayerInput  _sandboxInput;
        private float _predictedPitchInput;
        private float _predictedYawInput;
        
        // 平滑处理变量
        private float _currentSmoothedPitch;
        private float _currentSmoothedYaw;
        
        [SerializeField] private Transform _characterTransform; // 玩家角色旋转的Transform
        [SerializeField] private Transform _cameraPivot; // 相机旋转轴

        public override void NetworkStart()
        {
            // 初始化旋转角度
            if (IsServer || IsInputSource)
            {
                CameraPitch = _cameraPivot.localEulerAngles.x;
                CharacterYaw = _characterTransform.eulerAngles.y;
            }
            
            // 初始化平滑值
            _currentSmoothedPitch = CameraPitch;
            _currentSmoothedYaw = CharacterYaw;
        }

        public override void NetworkUpdate()
        {
            if (!IsInputSource || !Sandbox.InputEnabled)
                return;
            
            // 获取输入 - 使用deltaTime确保帧率无关
            Vector2 lookInput = _sandboxInput.GetCurrentFrameLookInput();
            float deltaTime = Sandbox.FixedDeltaTime; // 使用固定时间步长保持一致性
            
            // 累积预测输入量
            _predictedPitchInput -= lookInput.y * _rotationSpeed * deltaTime;
            _predictedYawInput += lookInput.x * _rotationSpeed * deltaTime;
            
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
                
                // 使用固定时间步长更新旋转
                float fixedDelta = Sandbox.FixedDeltaTime;
                
                // 更新俯仰角（垂直旋转）
                CameraPitch = ClampPitch(CameraPitch - input.LookY * _rotationSpeed * fixedDelta);
                
                // 更新偏航角（水平旋转）
                CharacterYaw += input.LookX * _rotationSpeed * fixedDelta;
            }

            // 重置预测输入
            _predictedPitchInput = 0;
            _predictedYawInput = 0;
            
            // 更新平滑值
            _currentSmoothedPitch = Mathf.Lerp(_currentSmoothedPitch, CameraPitch, _smoothingFactor * Sandbox.FixedDeltaTime);
            _currentSmoothedYaw = Mathf.Lerp(_currentSmoothedYaw, CharacterYaw, _smoothingFactor * Sandbox.FixedDeltaTime);
        }

        public override void NetworkRender()
        {
            // 对所有客户端应用平滑旋转
            ApplyCameraRotation(_currentSmoothedPitch);
            ApplyCharacterRotation(_currentSmoothedYaw);
        }

        [OnChanged(nameof(CameraPitch))]
        private void OnCameraPitchChanged(OnChangedData changeData)
        {
            // 代理端立即更新平滑值
            if (IsProxy)
            {
                _currentSmoothedPitch = CameraPitch;
            }
        }
        
        [OnChanged(nameof(CharacterYaw))]
        private void OnCharacterYawChanged(OnChangedData changeData)
        {
            // 代理端立即更新平滑值
            if (IsProxy)
            {
                _currentSmoothedYaw = CharacterYaw;
            }
        }

        private float ClampPitch(float pitch)
        {
            // 处理角度环绕问题
            if (pitch > 180f) pitch -= 360f;
            if (pitch < -180f) pitch += 360f;
            
            return Mathf.Clamp(pitch, _pitchClampMin, _pitchClampMax);
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
        }
    }
}