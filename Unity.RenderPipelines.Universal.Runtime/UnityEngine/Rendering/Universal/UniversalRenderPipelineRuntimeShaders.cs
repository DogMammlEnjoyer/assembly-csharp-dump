using System;
using UnityEngine.Categorization;

namespace UnityEngine.Rendering.Universal
{
	[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
	[CategoryInfo(Name = "R: Runtime Shaders", Order = 1000)]
	[HideInInspector]
	[Serializable]
	public class UniversalRenderPipelineRuntimeShaders : IRenderPipelineResources, IRenderPipelineGraphicsSettings
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

		public Shader fallbackErrorShader
		{
			get
			{
				return this.m_FallbackErrorShader;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_FallbackErrorShader, value, "m_FallbackErrorShader");
			}
		}

		public Shader blitHDROverlay
		{
			get
			{
				return this.m_BlitHDROverlay;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_BlitHDROverlay, value, "m_BlitHDROverlay");
			}
		}

		public Shader coreBlitPS
		{
			get
			{
				return this.m_CoreBlitPS;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_CoreBlitPS, value, "m_CoreBlitPS");
			}
		}

		public Shader coreBlitColorAndDepthPS
		{
			get
			{
				return this.m_CoreBlitColorAndDepthPS;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_CoreBlitColorAndDepthPS, value, "m_CoreBlitColorAndDepthPS");
			}
		}

		public Shader samplingPS
		{
			get
			{
				return this.m_SamplingPS;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_SamplingPS, value, "m_SamplingPS");
			}
		}

		public Shader terrainDetailLitShader
		{
			get
			{
				return this.m_TerrainDetailLit;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_TerrainDetailLit, value, "terrainDetailLitShader");
			}
		}

		public Shader terrainDetailGrassBillboardShader
		{
			get
			{
				return this.m_TerrainDetailGrassBillboard;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_TerrainDetailGrassBillboard, value, "terrainDetailGrassBillboardShader");
			}
		}

		public Shader terrainDetailGrassShader
		{
			get
			{
				return this.m_TerrainDetailGrass;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_TerrainDetailGrass, value, "terrainDetailGrassShader");
			}
		}

		[SerializeField]
		[HideInInspector]
		private int m_Version;

		[SerializeField]
		[ResourcePath("Shaders/Utils/FallbackError.shader", SearchType.ProjectPath)]
		private Shader m_FallbackErrorShader;

		[SerializeField]
		[ResourcePath("Shaders/Utils/BlitHDROverlay.shader", SearchType.ProjectPath)]
		internal Shader m_BlitHDROverlay;

		[SerializeField]
		[ResourcePath("Shaders/Utils/CoreBlit.shader", SearchType.ProjectPath)]
		internal Shader m_CoreBlitPS;

		[SerializeField]
		[ResourcePath("Shaders/Utils/CoreBlitColorAndDepth.shader", SearchType.ProjectPath)]
		internal Shader m_CoreBlitColorAndDepthPS;

		[SerializeField]
		[ResourcePath("Shaders/Utils/Sampling.shader", SearchType.ProjectPath)]
		private Shader m_SamplingPS;

		[Header("Terrain")]
		[SerializeField]
		[ResourcePath("Shaders/Terrain/TerrainDetailLit.shader", SearchType.ProjectPath)]
		private Shader m_TerrainDetailLit;

		[SerializeField]
		[ResourcePath("Shaders/Terrain/WavingGrassBillboard.shader", SearchType.ProjectPath)]
		private Shader m_TerrainDetailGrassBillboard;

		[SerializeField]
		[ResourcePath("Shaders/Terrain/WavingGrass.shader", SearchType.ProjectPath)]
		private Shader m_TerrainDetailGrass;
	}
}
