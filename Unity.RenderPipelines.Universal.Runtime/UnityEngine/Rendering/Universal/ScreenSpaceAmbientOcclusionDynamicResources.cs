using System;
using UnityEngine.Categorization;

namespace UnityEngine.Rendering.Universal
{
	[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
	[CategoryInfo(Name = "R: SSAO Noise Textures", Order = 1000)]
	[ElementInfo(Order = 0)]
	[HideInInspector]
	[Serializable]
	internal class ScreenSpaceAmbientOcclusionDynamicResources : IRenderPipelineResources, IRenderPipelineGraphicsSettings
	{
		public Texture2D[] BlueNoise256Textures
		{
			get
			{
				return this.m_BlueNoise256Textures;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_BlueNoise256Textures, value, "BlueNoise256Textures");
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
		[ResourceFormattedPaths("Textures/BlueNoise256/LDR_LLL1_{0}.png", 0, 7, SearchType.ProjectPath)]
		private Texture2D[] m_BlueNoise256Textures;

		[SerializeField]
		[HideInInspector]
		private int m_Version;
	}
}
