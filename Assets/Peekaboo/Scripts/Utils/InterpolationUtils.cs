using UnityEngine;

public static class InterpolationUtils
{
    public static float SmoothDamp(float current, float target, ref float velocity, float smoothTime, float time, float maxSpeed = Mathf.Infinity)
    {
        // 计算 omega 和 dt
        float omega = 2f / smoothTime;
        float dt = time;

        // 计算理想速度
        float delta = target - current;
        float desiredVelocity = (delta * omega) - (velocity * omega);

        // 更新速度并限制最大速度
        velocity += desiredVelocity * dt;
        velocity = Mathf.Clamp(velocity, -maxSpeed, maxSpeed);

        // 计算新值
        float newValue = current + delta * dt + velocity * dt;

        // 当目标值接近当前值时停止
        if (Mathf.Abs(delta) < 0.0001f)
        {
            velocity = 0f;  // 停止速度
            newValue = target;  // 直接设置目标
        }

        return newValue;
    }

     public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 velocity, float smoothTime,float time, float maxSpeed = Mathf.Infinity)
    {
        // 计算平滑时间对应的omega
        float omega = 2f / smoothTime;

        // 当前帧的时间增量
        float dt = time;

        // 计算目标位置和当前位置的差值
        Vector3 delta = target - current;

        // 计算理想速度，delta乘以omega表示目标的加速度，velocity乘以omega表示当前的减速度
        Vector3 desiredVelocity = (delta * omega) - (velocity * omega);

        // 更新速度
        velocity += desiredVelocity * dt;

        // 限制最大速度
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        // 计算新的位置
        Vector3 newPosition = current + delta * dt + velocity * dt;

        // 如果已经非常接近目标（即目标与当前的差距已经非常小），可以提前结束计算
        if (delta.magnitude < 0.0001f)
        {
            velocity = Vector3.zero;  // 目标到达，停止速度
            newPosition = target;     // 直接将当前位置设置为目标位置
        }

        return newPosition;
    }
    public static float SpringDamp(float current, float target, ref float velocity, float frequency, float dampingRatio, float maxSpeed, float deltaTime)
    {
        // 计算当前位置和目标位置的偏移量
        float displacement = target - current;

        // 计算弹簧的固有频率 omega_n（自然频率）
        float omega_n = 2f * Mathf.PI * frequency;

        // 计算阻尼比（zeta），影响振荡特性
        float zeta = dampingRatio;

        // 计算振荡的频率项
        float omega_d = omega_n * Mathf.Sqrt(1f - zeta * zeta); // 阻尼振荡的频率

        // 计算指数衰减项
        float expTerm = Mathf.Exp(-zeta * omega_n * deltaTime);

        // 计算振荡的余弦部分
        float cosTerm = Mathf.Cos(omega_d * deltaTime);

        // 计算振荡的正弦部分
        float sinTerm = Mathf.Sin(omega_d * deltaTime);

        // 计算弹簧力（F_spring = k * x），k 是弹簧常数（与频率相关）
        float springForce = displacement * omega_n * omega_n;

        // 计算阻尼力（F_damping = c * v），c 是阻尼系数
        float dampingForce = velocity * 2f * zeta * omega_n;

        // 合成弹簧力和阻尼力
        float force = springForce - dampingForce;

        // 更新物体的速度：加速度 = 合力 / 质量（假设质量为1）
        velocity += force * deltaTime;

        // 限制物体的最大速度，防止过快的物理运动
        velocity = Mathf.Clamp(velocity, -maxSpeed, maxSpeed);

        // 计算新的位置：基于速度计算新的位置
        float newPosition = current + velocity * deltaTime;

        // 当物体接近目标时，精确停止并防止微小的振荡
        if (Mathf.Abs(displacement) < 0.001f)
        {
            velocity = 0f;  // 停止速度
            newPosition = target; // 直接将物体放置在目标位置
        }

        return newPosition;
    }

    // SpringDamp的Vector3版本
    public static Vector3 SpringDamp(Vector3 current, Vector3 target, ref Vector3 velocity, float frequency, float dampingRatio, float maxSpeed, float deltaTime)
    {
        // 计算当前位置和目标位置的偏移量
        Vector3 displacement = target - current;

        // 计算弹簧的固有频率 omega_n（自然频率）
        float omega_n = 2f * Mathf.PI * frequency;

        // 计算阻尼比（zeta），影响振荡特性
        float zeta = dampingRatio;

        // 计算振荡的频率项
        float omega_d = omega_n * Mathf.Sqrt(1f - zeta * zeta); // 阻尼振荡的频率

        // 计算指数衰减项
        float expTerm = Mathf.Exp(-zeta * omega_n * deltaTime);

        // 计算振荡的余弦部分
        float cosTerm = Mathf.Cos(omega_d * deltaTime);

        // 计算振荡的正弦部分
        float sinTerm = Mathf.Sin(omega_d * deltaTime);

        // 计算弹簧力（F_spring = k * x），k 是弹簧常数（与频率相关）
        Vector3 springForce = displacement * omega_n * omega_n;

        // 计算阻尼力（F_damping = c * v），c 是阻尼系数
        Vector3 dampingForce = velocity * 2f * zeta * omega_n;

        // 合成弹簧力和阻尼力
        Vector3 force = springForce - dampingForce;

        // 更新物体的速度：加速度 = 合力 / 质量（假设质量为1）
        velocity += force * deltaTime;

        // 限制物体的最大速度，防止过快的物理运动
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        // 计算新的位置：基于速度计算新的位置
        Vector3 newPosition = current + velocity * deltaTime;

        // 当物体接近目标时，精确停止并防止微小的振荡
        if (displacement.magnitude < 0.001f)
        {
            velocity = Vector3.zero;  // 停止速度
            newPosition = target;     // 直接将物体放置在目标位置
        }

        return newPosition;
    }
    

}
