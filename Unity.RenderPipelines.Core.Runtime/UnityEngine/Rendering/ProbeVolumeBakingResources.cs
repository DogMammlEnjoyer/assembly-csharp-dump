using System;
using UnityEngine.Categorization;

namespace UnityEngine.Rendering
{
	[SupportedOnRenderPipeline(new Type[]
	{

	})]
	[CategoryInfo(Name = "R: Adaptive Probe Volumes", Order = 1000)]
	[HideInInspector]
	[Serializable]
	internal class ProbeVolumeBakingResources : IRenderPipelineResources, IRenderPipelineGraphicsSettings
	{
		public int version
		{
			get
			{
				return this.m_Version;
			}
		}

		[SerializeField]
		[HideInInspector]
		private int m_Version = 1;

		[Header("Baking")]
		[ResourcePath("Editor/Lighting/ProbeVolume/ProbeVolumeCellDilation.compute", SearchType.ProjectPath)]
		public ComputeShader dilationShader;

		[ResourcePath("Editor/Lighting/ProbeVolume/ProbeVolumeSubdivide.compute", SearchType.ProjectPath)]
		public ComputeShader subdivideSceneCS;

		[ResourcePath("Editor/Lighting/ProbeVolume/VoxelizeScene.shader", SearchType.ProjectPath)]
		public Shader voxelizeSceneShader;

		[ResourcePath("Editor/Lighting/ProbeVolume/VirtualOffset/TraceVirtualOffset.urtshader", SearchType.ProjectPath)]
		public ComputeShader traceVirtualOffsetCS;

		[ResourcePath("Editor/Lighting/ProbeVolume/VirtualOffset/TraceVirtualOffset.urtshader", SearchType.ProjectPath)]
		public RayTracingShader traceVirtualOffsetRT;

		[ResourcePath("Editor/Lighting/ProbeVolume/DynamicGI/DynamicGISkyOcclusion.urtshader", SearchType.ProjectPath)]
		public ComputeShader skyOcclusionCS;

		[ResourcePath("Editor/Lighting/ProbeVolume/DynamicGI/DynamicGISkyOcclusion.urtshader", SearchType.ProjectPath)]
		public RayTracingShader skyOcclusionRT;

		[ResourcePath("Editor/Lighting/ProbeVolume/RenderingLayerMask/TraceRenderingLayerMask.urtshader", SearchType.ProjectPath)]
		public ComputeShader renderingLayerCS;

		[ResourcePath("Editor/Lighting/ProbeVolume/RenderingLayerMask/TraceRenderingLayerMask.urtshader", SearchType.ProjectPath)]
		public RayTracingShader renderingLayerRT;
	}
}
