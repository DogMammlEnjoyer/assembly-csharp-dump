using System;
using UnityEngine.Categorization;

namespace UnityEngine.Rendering
{
	[SupportedOnRenderPipeline(new Type[]
	{

	})]
	[CategoryInfo(Name = "R: VRS - Runtime Resources", Order = 1000)]
	[HideInInspector]
	[Serializable]
	public sealed class VrsRenderPipelineRuntimeResources : IRenderPipelineResources, IRenderPipelineGraphicsSettings
	{
		public int version
		{
			get
			{
				return 0;
			}
		}

		bool IRenderPipelineGraphicsSettings.isAvailableInPlayerBuild
		{
			get
			{
				return true;
			}
		}

		public ComputeShader textureComputeShader
		{
			get
			{
				return this.m_TextureComputeShader;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_TextureComputeShader, value, "m_TextureComputeShader");
			}
		}

		public Shader visualizationShader
		{
			get
			{
				return this.m_VisualizationShader;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_VisualizationShader, value, "m_VisualizationShader");
			}
		}

		public VrsLut visualizationLookupTable
		{
			get
			{
				return this.m_VisualizationLookupTable;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_VisualizationLookupTable, value, "m_VisualizationLookupTable");
			}
		}

		public VrsLut conversionLookupTable
		{
			get
			{
				return this.m_ConversionLookupTable;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_ConversionLookupTable, value, "m_ConversionLookupTable");
			}
		}

		[SerializeField]
		[ResourcePath("Runtime/Vrs/Shaders/VrsTexture.compute", SearchType.ProjectPath)]
		private ComputeShader m_TextureComputeShader;

		[SerializeField]
		[ResourcePath("Runtime/Vrs/Shaders/VrsVisualization.shader", SearchType.ProjectPath)]
		private Shader m_VisualizationShader;

		[SerializeField]
		[Tooltip("Colors to visualize the shading rates")]
		private VrsLut m_VisualizationLookupTable = VrsLut.CreateDefault();

		[SerializeField]
		[Tooltip("Colors to convert between shading rates and textures")]
		private VrsLut m_ConversionLookupTable = VrsLut.CreateDefault();
	}
}
