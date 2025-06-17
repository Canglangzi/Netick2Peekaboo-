using System;
using Netick;
using Peekaboo.GamePlay;
using UnityEngine;

public partial class CKBCharacterController : NetworkCherry
{
    // ====== 配置参数 ======
    [Header("Movement Settings")]
    [SerializeField] private float walkingSpeed = 2.5f;
    [SerializeField] private float runningSpeed = 5f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float crawlSpeed = 0.5f;
    [SerializeField] private float jumpStrength = 5f;
    [SerializeField] private float gravityAcceleration = -9.81f;
    [SerializeField] private float slopeSlideSpeed = 1f;
    [SerializeField] private float stickToGroundForce = 0.5f;
    [SerializeField] private float maxFallSpeed = 350f;
    public float smoothTime = 0.3f; 

    [Header("Collider Settings")]
    [SerializeField] private float crouchHeight = 0.8f;
    [SerializeField] private float crouchRadius = 0.3f;
    [SerializeField] private float crawlHeight = 0.5f;
    [SerializeField] private float crawlRadius = 0.3f;

    [Header("Interaction Settings")]
    [SerializeField] private float interactRaycastDistance = 2f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask playerHitLayerMask;

    // ====== 组件引用缓存 ======
    [SerializeField] private CharacterController _cc;
    [SerializeField] private NetworkCharacterController _ncc;

    private Transform _orientation;

    private Vector3 _velocity;
    private Vector3 _platformVelocity;
    [SerializeField]private Vector3 _slideVelocity;
    private Vector3 _direction;
    private Vector3 _smoothDirection;
    private Vector3 _slopeDirection;
    private bool _didJump;
    private bool _isCrouching;
    private bool _isCrawling;
    private bool _isOnLadder;
    private bool _isClimbingLadder;
    private float _defaultHeight;
    private float _defaultRadius;
    private float _defaultStepOffset;
    private Vector3 _slopeSlideVelocity;
    private Vector3 _velocitySmooth; // 用于速度平滑的参考向量

    // ====== 网络同步属性 ======
    [Networked] public PlayerInput PlayerInput { get; set; }
    [Networked] public MovementState CurrentState { get; set; } = MovementState.Idle;
    [Networked][SerializeField] public CollisionFlags CollisionFlags { get; set; }
    [Smooth][Networked][SerializeField] private float _currentSpeed { get; set; }
    [Smooth][Networked][SerializeField] private float _targetSpeed { get; set; }
    
    public bool slideDownSlopes = true;
    public enum MovementState { Idle, Walking, Running, Crouching, Crawling }
    private enum CharacterGroundState { Grounded, Falling }

    // ====== 初始化 ======
    public override void NetworkStart()
    {
        // 确保组件引用正确
        if (!_cc) _cc = GetComponent<CharacterController>();
        if (!_ncc) _ncc = GetComponent<NetworkCharacterController>();
   
        // 方向参考点初始化
        _orientation = transform.Find("Orientation")?.transform ?? CreateOrientation();
        
        // 初始值缓存
        _defaultHeight = _cc.height;
        _defaultRadius = _cc.radius;
        _defaultStepOffset = _cc.stepOffset;
        _cc.skinWidth = _cc.radius / 5f;
    }

    private Transform CreateOrientation()
    {
        var go = new GameObject("Orientation");
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        return go.transform;
    }

    // ====== 输入处理 ======
    public override void NetworkUpdate()
    {
        if (!IsInputSource || !Sandbox.InputEnabled) return;

        var networkInput = Sandbox.GetInput<PlayerInput>();
        networkInput.Horizontal = Input.GetAxisRaw("Horizontal");
        networkInput.Vertical = Input.GetAxisRaw("Vertical");
        networkInput.ScrollWheel = Input.GetAxis("Mouse ScrollWheel");
        networkInput.JumpRequested  |= Input.GetButtonDown("Jump");
        networkInput.SprintRequested |= Input.GetKey(KeyCode.LeftShift);
        networkInput.CrouchRequested  |= Input.GetKeyDown(KeyCode.C);
        networkInput.CrawlRequested |= Input.GetKeyDown(KeyCode.Z);    
        networkInput.LookX = Input.GetAxis("Mouse X"); // X轴水平旋转
        networkInput.LookY = Input.GetAxis("Mouse Y"); // Y轴垂直旋转
        Sandbox.SetInput(networkInput );
    }

    // ====== 物理帧更新（核心逻辑） ======
    public override void NetworkFixedUpdate()
    {
        if (!FetchInput(out PlayerInput input)) return;
        
        PlayerInput = input;
        ProcessInput(input);
        UpdateMovementState();
        CalculateTargetSpeed();
        _cc.stepOffset = (!_ncc.IsGrounded || OnSlope()) ? 0 : _defaultStepOffset;
        ApplyPhysics();
        
        // 计算目标速度向量（水平移动 + 滑道滑动 + 平台速度）
        Vector3 targetMovement = _direction * _targetSpeed + _slideVelocity + _platformVelocity;
        targetMovement.y = _velocity.y; // 保持Y轴速度（重力和跳跃）
        // 使用InterpolationUtils.SmoothDamp进行速度平滑
        _velocity = InterpolationUtils.SmoothDamp(_velocity, targetMovement, ref _velocitySmooth, smoothTime, base.Orchard.ScaledFixedDeltaTime);
        
        // 应用移动
        CollisionFlags = _cc.Move(_velocity * base.Orchard.ScaledFixedDeltaTime);
        _ncc.Velocity = _velocity;
        
        // 更新当前速度值（用于网络同步）
        _currentSpeed = new Vector3(_velocity.x, 0, _velocity.z).magnitude;
        
        HandleSpecialStates();
        UpdateAnimator();
    }
    
    private void ProcessInput(PlayerInput input)
    {
        // 状态切换逻辑
        _didJump = input.JumpRequested;
        if (input.CrouchRequested) _isCrouching = !_isCrouching;
        if (input.CrawlRequested) _isCrawling = !_isCrawling;
    }
    
    private void UpdateMovementState()
    {
        if (_isCrawling) 
            CurrentState = MovementState.Crawling;
        else if (_isCrouching) 
            CurrentState = MovementState.Crouching;
        else if (PlayerInput.SprintRequested) 
            CurrentState = MovementState.Running;
        else if (PlayerInput.Horizontal != 0 || PlayerInput.Vertical != 0) 
            CurrentState = MovementState.Walking;
        else 
            CurrentState = MovementState.Idle;
    }
    
    private void CalculateTargetSpeed()
    {
        // 更新方向
        UpdateDirection();
        
        // 根据状态设置目标速度
        switch (CurrentState)
        {
            case MovementState.Crawling:
                _targetSpeed = crawlSpeed;
                break;
                
            case MovementState.Crouching:
                _targetSpeed = crouchSpeed;
                break;
                
            case MovementState.Walking:
                _targetSpeed = walkingSpeed;
                break;
                
            case MovementState.Running:
                _targetSpeed = runningSpeed;
                break;
                
            default: // Idle
                _targetSpeed = 0f;
                break;
        }
    }
    
    private void UpdateDirection()
    {
        // 获取原始输入值
        float horizontal = PlayerInput.Horizontal;
        float vertical = PlayerInput.Vertical;
    
        // 保持输入值的原始向量
        Vector3 rawDirection = new Vector3(horizontal, 0, vertical);
    
        // 使用Lerp进行输入向量的平滑过渡
        _smoothDirection = Vector3.Lerp(_smoothDirection, rawDirection, 10 * base.Orchard.ScaledFixedDeltaTime);
    
        // === 使用斜坡修正的方向计算目标移动方向 ===
        Vector3 slopeAdjustedForward = SlopeDirection(); // 获取斜坡修正后的前进方向
    
        // 计算斜坡修正的移动方向（使用平滑后的输入值）
        Vector3 slopeDirection = slopeAdjustedForward * _smoothDirection.z;
        Vector3 horizontalDirection = _orientation.right * _smoothDirection.x;
    
        // 组合成最终方向向量（与原始代码类似）
        _direction = slopeDirection + horizontalDirection;
    
        // 如果方向有效则进行标准化
        if (_direction.magnitude > 0.1f)
        {
            _direction.Normalize();
        }
        else
        {
            _direction = Vector3.zero;
        }
    
        // === 绘制调试射线 ===
        // 绘制原始输入方向（蓝色）
        Debug.DrawRay(transform.position, _orientation.forward * _smoothDirection.z * 2f, Color.blue, 0.1f);
    
        // 绘制斜坡修正方向（绿色）
        Debug.DrawRay(transform.position, slopeAdjustedForward * 2f, Color.green, 0.1f);
    
        // 绘制最终移动方向（红色）
        Debug.DrawRay(transform.position, _direction * 2f, Color.red, 0.1f);
    }
    
    private void ApplyPhysics()
    {
        // 重力处理
        if (!_ncc.IsGrounded)
        {
            _velocity.y += gravityAcceleration * base.Orchard.ScaledFixedDeltaTime;
            _velocity.y = Mathf.Max(_velocity.y, -maxFallSpeed);
        }
        
        // 斜坡滑动物理
        UpdateSlopeSlideVelocity();
        
        // 跳跃处理（保留原有跳跃逻辑）
        if (_ncc.IsGrounded && _didJump)
        {
            _velocity.y = Mathf.Sqrt(jumpStrength * -2f * gravityAcceleration);
        }
    }
    
    private void UpdateSlopeSlideVelocity()
    {
        if (slideDownSlopes && OnMaxedAngleSlope())
        {
            // 计算斜坡滑动方向
            _slopeSlideVelocity.x = _slopeDirection.x;
            _slopeSlideVelocity.y = -_slopeDirection.y;
            _slopeSlideVelocity.z = _slopeDirection.z;
            
            _slideVelocity = _slopeSlideVelocity * slopeSlideSpeed * base.Orchard.ScaledFixedDeltaTime;
        }
        else
        {
            _slideVelocity = Vector3.zero;
        }
    }

    private void HandleSpecialStates()
    {
        // 梯子状态处理
        if (_isOnLadder && _isClimbingLadder)
        {
            // 梯子状态下的移动限制
            _direction.x = 0;
            _direction.z = 0;
            
            // 梯子垂直移动
            if (PlayerInput.Vertical > 0)  // 向上爬升
            {
                _velocity.y = 3f;
            }
            else if (PlayerInput.Vertical < 0)  // 向下爬升
            {
                // 检测地面避免坠落
                if (!Physics.Raycast(transform.position, Vector3.down, 1.2f))
                {
                    _velocity.y = -1f;
                }
                else
                {
                    ExitLadderState();
                }
            }
            else
            {
                _velocity.y = 0;
            }
        }
        
        // 滑索状态处理
        if (IsOnZipline)
        {
            // 禁用水平移动
            _direction = Vector3.zero;
            _velocity.y = 0;
        }
    }

    public bool IsOnZipline { get; set; }

    private void UpdateAnimator()
    {
        // 更新动画参数
        bool isRunning = CurrentState == MovementState.Running;
        bool isCrouching = CurrentState == MovementState.Crouching;
        bool isCrawling = CurrentState == MovementState.Crawling;
        bool isJumping = _didJump;
        bool isIdle = CurrentState == MovementState.Idle;
        bool isFalling = !_ncc.IsGrounded && _ncc.Velocity.y < 0;

        // 使用Lerp平滑动画过渡
        // if (_characterAnimator)
        // {
        //     _characterAnimator.UpdateAnimatorState(
        //         PlayerInput.Horizontal, 
        //         PlayerInput.Vertical, 
        //         isCrouching, 
        //         isCrawling, 
        //         isJumping,  
        //         isRunning, 
        //         isIdle,
        //         isFalling,
        //         _currentSpeed, 
        //         base.Orchard.ScaledFixedDeltaTime
        //     );
        // }
    }
    
    // ====== 辅助方法 ======
    private bool OnMaxedAngleSlope()
    {
        if (_ncc.IsGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _cc.height))
        {
            _slopeDirection = hit.normal;
            return Vector3.Angle(_slopeDirection, Vector3.up) > _cc.slopeLimit;
        }
        return false;
    }
    public bool OnSlope()
    {
        //check if slope angle is more than 0
        if (SlopeAngle() > 0)
        {
            return true;
        }

        return false;
    }
    public float SlopeAngle()
    {
        //setup a raycast from position to down at the bottom of the collider
        RaycastHit slopeHit;
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit))
        {
            //get the direction result according to slope normal
            return (Vector3.Angle(Vector3.down, slopeHit.normal) - 180) * -1;
        }

        //if not on slope then slope is forward ;)
        return 0;
    }
    private Vector3 SlopeDirection()
    {
        if (Physics.Raycast(_orientation.position, Vector3.down, out RaycastHit hit, (_cc.height / 2) + 0.1f))
        {
            return Vector3.ProjectOnPlane(_orientation.forward, hit.normal);
        }
        return _orientation.forward;
    }
    
    private void ExitLadderState()
    {
        _isOnLadder = false;
        _isClimbingLadder = false;
        _velocity.y = 0;
    }
    
    // ====== 碰撞处理 ======
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // 物理物体交互
        if (hit.collider.attachedRigidbody is Rigidbody body && !body.isKinematic)
        {
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            body.AddForce(pushDir * 1.1f, ForceMode.Impulse);
        }

        // 头部碰撞检测
        if ((CollisionFlags & CollisionFlags.Above) != 0)
        {
            _velocity.y = Mathf.Min(_velocity.y, 0);
        }
    }
    
    // ====== 其他方法 ======
    public void AddPlatformVelocity(Vector3 platformVelocity) => _platformVelocity = platformVelocity;
    public void NullToInteraction() { /* 您的交互重置逻辑 */ }
    public void ChangeState(MovementState newState) => CurrentState = newState;
    public void GrabLadder() => _isClimbingLadder = true;
    public void DropLadder() => _isClimbingLadder = false;
}