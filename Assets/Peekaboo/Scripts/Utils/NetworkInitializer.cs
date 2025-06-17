using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class NetworkInitializer : NetworkCherry
{
   
   public override void NetworkStart()
   {
      LoadAsset();
   }

   private async UniTask LoadAsset()
   {
      var gameInstance = await Game.LoadAssetAsync<GameObject>(GameplaySettings.InstanceSettings.GameInstancePrefab);
      base.Orchard.NetworkSpawn(gameInstance);
   }
}
