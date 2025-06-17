using Netick;
using UnityEngine;

namespace Peekaboo.GamePlay
{
    [Networked]
    public struct PlayerInput : INetworkInput
    {
    
        [Networked][Smooth] public float Horizontal { get; set;}
        [Networked][Smooth] public float Vertical{ get; set;}
        [Networked][Smooth] public NetworkBool JumpRequested{get; set;}

        [Networked][Smooth]public  NetworkBool SprintRequested{get; set;}

        [Networked][Smooth]public  NetworkBool CrouchRequested{get; set;}
        [Networked][Smooth]public NetworkBool CrawlRequested { get; set; }
        [Networked]public float ScrollWheel { get; set; }
        
        
        // 新增鼠标视角输入
        [Networked][Smooth] public float LookX { get; set; }
        [Networked][Smooth] public float LookY { get; set; }

        /// <summary>
        /// 获取当前帧的视角输入
        /// </summary>
        /// <returns>包含水平和垂直输入的Vector2</returns>
        public Vector2 GetCurrentFrameLookInput()
        {
            return new Vector2(LookX, LookY);
        }
    }

}
