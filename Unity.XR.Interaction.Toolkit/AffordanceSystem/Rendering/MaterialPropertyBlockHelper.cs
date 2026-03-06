using System;

namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Rendering
{
	[AddComponentMenu("Affordance System/Rendering/Material Property Block Helper", 12)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Rendering.MaterialPropertyBlockHelper.html")]
	[Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
	public class MaterialPropertyBlockHelper : MaterialHelperBase
	{
		protected void OnDestroy()
		{
			this.m_PropertyBlock = null;
		}

		protected void LateUpdate()
		{
			if (!this.m_IsDirty || !base.isInitialized)
			{
				return;
			}
			base.rendererTarget.SetPropertyBlock(this.m_PropertyBlock, base.materialIndex);
			this.m_IsDirty = false;
		}

		public MaterialPropertyBlock GetMaterialPropertyBlock(bool markPropertyBlockAsDirty = true)
		{
			if (markPropertyBlockAsDirty)
			{
				this.m_IsDirty = true;
			}
			return this.m_PropertyBlock;
		}

		protected override void Initialize()
		{
			base.Initialize();
			this.m_PropertyBlock = new MaterialPropertyBlock();
			base.rendererTarget.GetPropertyBlock(this.m_PropertyBlock, base.materialIndex);
		}

		private MaterialPropertyBlock m_PropertyBlock;

		private bool m_IsDirty;
	}
}
