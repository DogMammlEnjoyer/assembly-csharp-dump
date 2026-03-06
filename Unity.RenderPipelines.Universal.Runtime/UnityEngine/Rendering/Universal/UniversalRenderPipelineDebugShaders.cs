using System;
using UnityEngine.Categorization;

namespace UnityEngine.Rendering.Universal
{
	[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
	[CategoryInfo(Name = "R: Debug Shaders", Order = 1000)]
	[HideInInspector]
	[Serializable]
	public class UniversalRenderPipelineDebugShaders : IRenderPipelineResources, IRenderPipelineGraphicsSettings
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
				return false;
			}
		}

		public Shader debugReplacementPS
		{
			get
			{
				return this.m_DebugReplacementPS;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_DebugReplacementPS, value, "m_DebugReplacementPS");
			}
		}

		public Shader hdrDebugViewPS
		{
			get
			{
				return this.m_HdrDebugViewPS;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_HdrDebugViewPS, value, "m_HdrDebugViewPS");
			}
		}

		public ComputeShader probeVolumeSamplingDebugComputeShader
		{
			get
			{
				return this.m_ProbeVolumeSamplingDebugComputeShader;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_ProbeVolumeSamplingDebugComputeShader, value, "m_ProbeVolumeSamplingDebugComputeShader");
			}
		}

		[SerializeField]
		[ResourcePath("Shaders/Debug/DebugReplacement.shader", SearchType.ProjectPath)]
		private Shader m_DebugReplacementPS;

		[SerializeField]
		[ResourcePath("Shaders/Debug/HDRDebugView.shader", SearchType.ProjectPath)]
		private Shader m_HdrDebugViewPS;

		[SerializeField]
		[ResourcePath("Shaders/Debug/ProbeVolumeSamplingDebugPositionNormal.compute", SearchType.ProjectPath)]
		private ComputeShader m_ProbeVolumeSamplingDebugComputeShader;
	}
}
