using System;
using UnityEngine.Categorization;

namespace UnityEngine.Rendering.Universal
{
	[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
	[CategoryInfo(Name = "R: SSAO Shader", Order = 1000)]
	[ElementInfo(Order = 0)]
	[HideInInspector]
	[Serializable]
	internal class ScreenSpaceAmbientOcclusionPersistentResources : IRenderPipelineResources, IRenderPipelineGraphicsSettings
	{
		public Shader Shader
		{
			get
			{
				return this.m_Shader;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_Shader, value, "Shader");
			}
		}

		public bool isAvailableInPlayerBuild
		{
			get
			{
				return true;
			}
		}

		public int version
		{
			get
			{
				return this.m_Version;
			}
		}

		[SerializeField]
		[ResourcePath("Shaders/Utils/ScreenSpaceAmbientOcclusion.shader", SearchType.ProjectPath)]
		private Shader m_Shader;

		[SerializeField]
		[HideInInspector]
		private int m_Version;
	}
}
