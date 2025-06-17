//using CocKleBurs.Network;
using Netick;
using Netick.Unity;
using UnityEngine;

    public class ClientEntity : NetworkCherry
    {
        [Networked] public PlayerRef PlayerId { get; set; }
        public static ClientEntity LocalPlayer;
        [OnChanged(nameof(PlayerId))]
        void OnPlayerIdChanged(OnChangedData onChangedData)
        {
            OnPlayerIdAssigned();
        }

        /// <summary>
        /// This Means PlayerRef Property is assigned/changed by Server
        /// </summary>
        protected virtual void OnPlayerIdAssigned()
        {
            if (Object.IsInputSource)
                LocalPlayer = this;
        }

        public virtual void OnDespawned()
        {
        }
        
    }