using System;
using UnityEngine.Categorization;

namespace UnityEngine.Rendering.Universal
{
	[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
	[CategoryInfo(Name = "R: Universal Renderer Shaders", Order = 1000)]
	[HideInInspector]
	[Serializable]
	public class UniversalRendererResources : IRenderPipelineResources, IRenderPipelineGraphicsSettings
	{
		public int version
		{
			get
			{
				return this.m_Version;
			}
		}

		bool IRenderPipelineGraphicsSettings.isAvailableInPlayerBuild
		{
			get
			{
				return true;
			}
		}

		public Shader copyDepthPS
		{
			get
			{
				return this.m_CopyDepthPS;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_CopyDepthPS, value, "m_CopyDepthPS");
			}
		}

		public Shader cameraMotionVector
		{
			get
			{
				return this.m_CameraMotionVector;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_CameraMotionVector, value, "m_CameraMotionVector");
			}
		}

		public Shader stencilDeferredPS
		{
			get
			{
				return this.m_StencilDeferredPS;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_StencilDeferredPS, value, "m_StencilDeferredPS");
			}
		}

		public Shader clusterDeferred
		{
			get
			{
				return this.m_ClusterDeferred;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_ClusterDeferred, value, "m_ClusterDeferred");
			}
		}

		public Shader stencilDitherMaskSeedPS
		{
			get
			{
				return this.m_StencilDitherMaskSeedPS;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_StencilDitherMaskSeedPS, value, "m_StencilDitherMaskSeedPS");
			}
		}

		public Shader decalDBufferClear
		{
			get
			{
				return this.m_DBufferClear;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_DBufferClear, value, "m_DBufferClear");
			}
		}

		[SerializeField]
		[HideInInspector]
		private int m_Version;

		[SerializeField]
		[ResourcePath("Shaders/Utils/CopyDepth.shader", SearchType.ProjectPath)]
		private Shader m_CopyDepthPS;

		[SerializeField]
		[ResourcePath("Shaders/CameraMotionVectors.shader", SearchType.ProjectPath)]
		private Shader m_CameraMotionVector;

		[SerializeField]
		[ResourcePath("Shaders/Utils/StencilDeferred.shader", SearchType.ProjectPath)]
		private Shader m_StencilDeferredPS;

		[SerializeField]
		[ResourcePath("Shaders/Utils/ClusterDeferred.shader", SearchType.ProjectPath)]
		private Shader m_ClusterDeferred;

		[SerializeField]
		[ResourcePath("Shaders/Utils/StencilDitherMaskSeed.shader", SearchType.ProjectPath)]
		private Shader m_StencilDitherMaskSeedPS;

		[Header("Decal Renderer Feature Specific")]
		[SerializeField]
		[ResourcePath("Runtime/Decal/DBuffer/DBufferClear.shader", SearchType.ProjectPath)]
		private Shader m_DBufferClear;
	}
}
