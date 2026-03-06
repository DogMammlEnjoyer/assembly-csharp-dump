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
	internal class ProbeVolumeRuntimeResources : IRenderPipelineResources, IRenderPipelineGraphicsSettings
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

		[Header("Runtime")]
		[ResourcePath("Runtime/Lighting/ProbeVolume/ProbeVolumeBlendStates.compute", SearchType.ProjectPath)]
		public ComputeShader probeVolumeBlendStatesCS;

		[ResourcePath("Runtime/Lighting/ProbeVolume/ProbeVolumeUploadData.compute", SearchType.ProjectPath)]
		public ComputeShader probeVolumeUploadDataCS;

		[ResourcePath("Runtime/Lighting/ProbeVolume/ProbeVolumeUploadDataL2.compute", SearchType.ProjectPath)]
		public ComputeShader probeVolumeUploadDataL2CS;
	}
}
