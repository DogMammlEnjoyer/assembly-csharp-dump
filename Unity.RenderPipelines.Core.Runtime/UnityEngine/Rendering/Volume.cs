using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering
{
	[ExecuteAlways]
	[AddComponentMenu("Miscellaneous/Volume")]
	public class Volume : MonoBehaviour, IVolume
	{
		public bool isGlobal
		{
			get
			{
				return this.m_IsGlobal;
			}
			set
			{
				this.m_IsGlobal = value;
				if (!this.m_IsGlobal)
				{
					this.UpdateColliders();
				}
			}
		}

		public VolumeProfile profile
		{
			get
			{
				if (this.m_InternalProfile == null)
				{
					this.m_InternalProfile = ScriptableObject.CreateInstance<VolumeProfile>();
					if (this.sharedProfile != null)
					{
						this.m_InternalProfile.name = this.sharedProfile.name;
						foreach (VolumeComponent original in this.sharedProfile.components)
						{
							VolumeComponent item = Object.Instantiate<VolumeComponent>(original);
							this.m_InternalProfile.components.Add(item);
						}
					}
				}
				return this.m_InternalProfile;
			}
			set
			{
				this.m_InternalProfile = value;
			}
		}

		public List<Collider> colliders
		{
			get
			{
				return this.m_Colliders;
			}
		}

		internal GameObject cachedGameObject
		{
			get
			{
				return this.m_CachedGameObject;
			}
		}

		internal VolumeProfile profileRef
		{
			get
			{
				if (!(this.m_InternalProfile == null))
				{
					return this.m_InternalProfile;
				}
				return this.sharedProfile;
			}
		}

		public bool HasInstantiatedProfile()
		{
			return this.m_InternalProfile != null;
		}

		private void OnEnable()
		{
			this.m_CachedGameObject = base.gameObject;
			this.m_PreviousLayer = this.cachedGameObject.layer;
			VolumeManager.instance.Register(this);
			this.UpdateColliders();
		}

		private void OnDisable()
		{
			VolumeManager.instance.Unregister(this);
		}

		private void Update()
		{
			this.UpdateLayer();
			this.UpdatePriority();
		}

		public void UpdateColliders()
		{
			base.GetComponents<Collider>(this.m_Colliders);
		}

		internal void UpdateLayer()
		{
			int layer = this.cachedGameObject.layer;
			if (layer == this.m_PreviousLayer)
			{
				return;
			}
			VolumeManager.instance.UpdateVolumeLayer(this, this.m_PreviousLayer, layer);
			this.m_PreviousLayer = layer;
		}

		internal void UpdatePriority()
		{
			if (Mathf.Abs(this.priority - this.m_PreviousPriority) <= Mathf.Epsilon)
			{
				return;
			}
			VolumeManager.instance.SetLayerDirty(this.cachedGameObject.layer);
			this.m_PreviousPriority = this.priority;
		}

		private void OnValidate()
		{
			this.blendDistance = Mathf.Max(this.blendDistance, 0f);
		}

		[SerializeField]
		[FormerlySerializedAs("isGlobal")]
		private bool m_IsGlobal = true;

		[Delayed]
		[FormerlySerializedAs("m_Priority")]
		public float priority;

		[FormerlySerializedAs("m_BlendDistance")]
		public float blendDistance;

		[Range(0f, 1f)]
		[FormerlySerializedAs("m_Weight")]
		public float weight = 1f;

		public VolumeProfile sharedProfile;

		private readonly List<Collider> m_Colliders = new List<Collider>();

		private GameObject m_CachedGameObject;

		private int m_PreviousLayer;

		private float m_PreviousPriority;

		private VolumeProfile m_InternalProfile;
	}
}
