using UnityEngine;
using UnityEngine.Events;
using Netick;
using Netick.Unity;
using UnityEngine.Serialization;

namespace Peekaboo.GamePlay
{
    [ExecutionOrder(-1000)]
    public class NetworkCharacterController : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] 
        private Transform _renderTransform;
        public LayerMask CollisionLayerMask;

        [Header("Events")]
        [HideInInspector] 
        public UnityEvent OnLand = new UnityEvent();

        // 网络同步属性
        [Networked] public NetworkBool IsGrounded { get; set; }
        [Networked] public Vector3 Velocity { get; set; } = Vector3.zero;
        [Networked][Smooth(false)] 
        public Vector3 Position { get; set; }
        [Networked] public DimensionalState CurrentDimensionalState { get; set; } = DimensionalState.Normal;

        // 组件引用
        protected CharacterController _characterController;
        private Interpolator _positionInterpolator;
        private RaycastHit _groundHit;

        public enum DimensionalState
        {
            Normal,
            Zipline,
            WallRunning,
        }

        private void Awake()
        {
            _characterController = GetComponentInChildren<CharacterController>();
        }

        public override void NetworkStart()
        {
            _positionInterpolator = FindInterpolator(nameof(Position));
            Position = transform.position;
        }

        public override void GameEngineIntoNetcode()
        {
            Position = transform.position;
            
            switch (CurrentDimensionalState)
            {
                case DimensionalState.Normal:
                    IsGrounded = CheckGroundHit(0.001f);
                    break;
                
                // 特殊维度状态处理
                case DimensionalState.Zipline:
                    IsGrounded = false;
                    break;
                    
                case DimensionalState.WallRunning:
                    IsGrounded = false;
                    break;

            }
        }

        public override void NetcodeIntoGameEngine()
        {
            transform.position = Position;
        }

        public override void NetworkRender()
        {
            const float TELEPORT_THRESHOLD = 50f;
            
            if (_positionInterpolator.GetInterpolationData(InterpolationSource.Auto, 
                    out Vector3 from, out Vector3 to, out float alpha))
            {
                _renderTransform.position = Vector3.Distance(from, to) > TELEPORT_THRESHOLD 
                    ? to 
                    : Vector3.Lerp(from, to, alpha);
            }
        }

        [OnChanged(nameof(IsGrounded))]
        private void HandleGroundedChanged(OnChangedData change)
        {
            if (!IsResimulating && IsGrounded)
                OnLand.Invoke();
        }
        public void EnterSpecialState(DimensionalState state)
        {
            CurrentDimensionalState = state;
            
            // 状态特定初始化
            switch (state)
            {
                case DimensionalState.Zipline:
                    Velocity = Vector3.zero; // 滑索时重置速度
                    break;
            }
        }
        
        public void ReturnToNormalState()
        {
            CurrentDimensionalState = DimensionalState.Normal;
        }
        
        
        private bool CheckGroundHit(float distance)
        {
            Vector3 capsuleBase = GetCharacterCapsuleBase();
            return Sandbox.Physics.SphereCast(
                capsuleBase + Vector3.up * _characterController.radius,
                _characterController.radius,
                Vector3.down,
                out _groundHit,
                _characterController.skinWidth + distance,
                CollisionLayerMask
            );
        }
        
        private Vector3 GetCharacterCapsuleBase()
        {
            return transform.position + 
                   transform.TransformVector(_characterController.center) - 
                   Vector3.up * _characterController.height / 2;
        }
    }
}