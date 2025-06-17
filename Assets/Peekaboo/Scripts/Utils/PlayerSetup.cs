
using Netick.Unity;
using UnityEngine;

namespace CLz_Network
{
    public class PlayerSetup : NetworkBehaviour
    {
        public Transform playerBody; // �������
        public Transform playerArms; // ����ֱ�
       

        public static PlayerSetup localPlayer; // ������ҵľ�̬���� 

        public override void NetworkStart()
        {
            if (IsInputSource || Sandbox.IsVisible)
            {
                SetPlayerLayers(true);
            }
            else
            {
                // �����������ͼ��
                SetPlayerLayers(false);
            }            
        }
        
        private void SetPlayerLayers(bool isLocalPlayer)
        {
            if (isLocalPlayer)
            {
                SetLayerRecursively(playerArms, LayerMask.NameToLayer("MyArms"));
                SetLayerRecursively(playerBody, LayerMask.NameToLayer("MyBody"));
            }
            else
            {
                SetLayerRecursively(playerArms, LayerMask.NameToLayer("OtherArms"));
                SetLayerRecursively(playerBody, LayerMask.NameToLayer("OtherBody"));
            }
        }

        private void SetLayerRecursively(Transform trans, int layer)
        {
            trans.gameObject.layer = layer;
            foreach (Transform child in trans)
            {
                SetLayerRecursively(child, layer);
            }
        }
    }
}
