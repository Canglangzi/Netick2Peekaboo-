using Unity.Cinemachine;
using UnityEngine;

public class PlayerCameraSetup : NetworkCherry
{
   public CinemachineCamera CinemachineCamera;
   public override void NetworkStart()
   {
      if (IsInputSource)
      {
         CinemachineCamera.Priority = 3;
      }
   }
#if UNITY_EDITOR

   public override void NetworkRender()
   {
      if (!Sandbox.IsVisible || !IsInputSource)
      {
         CinemachineCamera.Priority = -1;
      }
      else
      {
         CinemachineCamera.Priority = 3;
      }

   }
#endif
}
