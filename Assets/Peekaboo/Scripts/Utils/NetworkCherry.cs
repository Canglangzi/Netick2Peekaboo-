using CockleBurs.GamePlay;
using Netick;
using Netick.Unity;
using UnityEngine;

public class NetworkCherry : NetworkBehaviour,INetickSceneLoadDone
{ 
	private bool _IsInputSource => Object.IsInputSource;
	
	public new virtual bool IsInputSource => _IsInputSource;
	
	private Transform m_transform;
	private RectTransform m_rectTransform;
	private GameObject m_gameObject;
	public NetworkConnection  localConnection ;
	// Serialized Fields
	[SerializeField]
	[HideInInspector]
	private bool m_ShowProperties = false;
		
	// Properties
	/// <summary>
	/// Defines if the SerializedProperties of this SuperBehaviour should be shown in the inspector.
	/// </summary>
	public bool ShowProperties { get => m_ShowProperties; set => m_ShowProperties = value; }
	
	/// <summary>
	/// This is a cached version of the transform property.
	/// </summary>
	public new Transform transform
	{
		get
		  { 
			  if (IsDestroyed) return null;
			  try
				{
					if (m_transform == null)
					{
						m_transform = base.transform;
					}
					return m_transform;
				}
				catch
				{
					// In case unity uses they're null override to hide the object still exists
					return null;
				}
		  }
		}
		
		/// <summary>
        /// The GameObject attached to this SuperBehaviour.<br/>
        /// This is a cached version of the gameObject property.
        /// </summary>
		public new GameObject gameObject
		{
			get
			{
				if (IsDestroyed) return null;
				try
				{
					if (m_gameObject == null)
					{
						m_gameObject = base.gameObject;
					}
					return m_gameObject;
				}
				catch
				{
					// In case unity uses they're null override to hide the object still exists
					return null;
				}
			}
		}
		
		/// <summary>
        /// Returns if this instance has been marked as destroyed by Unity.
        /// </summary>
        /// <returns>True if the instance has been destroyed, false otherwise.</returns>
		public bool IsDestroyed => this == null;
		
        private Orchard _orchard;
        public virtual Orchard Orchard
        {
            get
            {
                if (_orchard is  null)
                {
	                _orchard = Sandbox as Orchard;
                }
                return _orchard;
            }
        }
        public virtual GameplaySettings GameplaySettings
        {
	        get
	        {
		        return Game.GameplaySettings;
	        }
        }
        public virtual void OnSceneLoadDone(Orchard sandbox)
        {
	       // _orchard = sandbox;
	       //localConnection  = Sandbox.LocalPlayer as NetworkConnection;
	        OnActive(localConnection);
        }
	
        public virtual void OnActive(NetworkConnection connection) { }

        public float NetworkScaledFixedDeltaTime => _orchard.ScaledFixedDeltaTime;
}
