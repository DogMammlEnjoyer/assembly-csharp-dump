using System;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Rendering
{
	[Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
	public abstract class MaterialHelperBase : MonoBehaviour
	{
		public Renderer rendererTarget
		{
			get
			{
				return this.m_Renderer;
			}
			set
			{
				this.m_Renderer = value;
			}
		}

		public int materialIndex
		{
			get
			{
				return this.m_MaterialIndex;
			}
			set
			{
				this.m_MaterialIndex = value;
			}
		}

		private protected bool isInitialized { protected get; private set; }

		protected void OnEnable()
		{
			if (this.isInitialized)
			{
				return;
			}
			if (this.m_Renderer == null)
			{
				this.m_Renderer = base.GetComponentInParent<Renderer>();
			}
			if (this.m_Renderer == null)
			{
				XRLoggingUtils.LogError(string.Format("No renderer found on {0}. Disabling this material helper component.", this), this);
				base.enabled = false;
				return;
			}
			if (this.m_Renderer.sharedMaterials.Length == 0)
			{
				XRLoggingUtils.LogError(string.Format("Renderer found on {0} does not have any shared materials. Disabling this material helper component.", this), this);
				base.enabled = false;
				return;
			}
			if (this.m_MaterialIndex > this.m_Renderer.sharedMaterials.Length)
			{
				XRLoggingUtils.LogWarning(string.Format("Insufficient number of materials set on associated render for {0}.", this) + " Setting target material index to 0.", this);
				this.m_MaterialIndex = 0;
				return;
			}
			this.Initialize();
		}

		protected virtual void Initialize()
		{
			this.isInitialized = true;
		}

		public Material GetSharedMaterialForTarget()
		{
			return this.m_Renderer.sharedMaterials[this.materialIndex];
		}

		[SerializeField]
		private Renderer m_Renderer;

		[SerializeField]
		private int m_MaterialIndex;
	}
}
