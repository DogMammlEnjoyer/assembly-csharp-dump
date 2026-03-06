using System;
using UnityEngine.Categorization;

namespace UnityEngine.Rendering
{
	[SupportedOnRenderPipeline(new Type[]
	{

	})]
	[CategoryInfo(Name = "R: GPU Resident Drawers", Order = 1000)]
	[HideInInspector]
	[Serializable]
	internal class GPUResidentDrawerResources : IRenderPipelineResources, IRenderPipelineGraphicsSettings
	{
		int IRenderPipelineGraphicsSettings.version
		{
			get
			{
				return (int)this.m_Version;
			}
		}

		public ComputeShader instanceDataBufferCopyKernels
		{
			get
			{
				return this.m_InstanceDataBufferCopyKernels;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_InstanceDataBufferCopyKernels, value, "m_InstanceDataBufferCopyKernels");
			}
		}

		public ComputeShader instanceDataBufferUploadKernels
		{
			get
			{
				return this.m_InstanceDataBufferUploadKernels;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_InstanceDataBufferUploadKernels, value, "m_InstanceDataBufferUploadKernels");
			}
		}

		public ComputeShader transformUpdaterKernels
		{
			get
			{
				return this.m_TransformUpdaterKernels;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_TransformUpdaterKernels, value, "m_TransformUpdaterKernels");
			}
		}

		public ComputeShader windDataUpdaterKernels
		{
			get
			{
				return this.m_WindDataUpdaterKernels;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_WindDataUpdaterKernels, value, "m_WindDataUpdaterKernels");
			}
		}

		public ComputeShader occluderDepthPyramidKernels
		{
			get
			{
				return this.m_OccluderDepthPyramidKernels;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_OccluderDepthPyramidKernels, value, "m_OccluderDepthPyramidKernels");
			}
		}

		public ComputeShader instanceOcclusionCullingKernels
		{
			get
			{
				return this.m_InstanceOcclusionCullingKernels;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_InstanceOcclusionCullingKernels, value, "m_InstanceOcclusionCullingKernels");
			}
		}

		public ComputeShader occlusionCullingDebugKernels
		{
			get
			{
				return this.m_OcclusionCullingDebugKernels;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_OcclusionCullingDebugKernels, value, "m_OcclusionCullingDebugKernels");
			}
		}

		public Shader debugOcclusionTestPS
		{
			get
			{
				return this.m_DebugOcclusionTestPS;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_DebugOcclusionTestPS, value, "m_DebugOcclusionTestPS");
			}
		}

		public Shader debugOccluderPS
		{
			get
			{
				return this.m_DebugOccluderPS;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_DebugOccluderPS, value, "m_DebugOccluderPS");
			}
		}

		[SerializeField]
		[HideInInspector]
		private GPUResidentDrawerResources.Version m_Version;

		[SerializeField]
		[ResourcePath("Runtime/RenderPipelineResources/GPUDriven/InstanceDataBufferCopyKernels.compute", SearchType.ProjectPath)]
		private ComputeShader m_InstanceDataBufferCopyKernels;

		[SerializeField]
		[ResourcePath("Runtime/RenderPipelineResources/GPUDriven/InstanceDataBufferUploadKernels.compute", SearchType.ProjectPath)]
		private ComputeShader m_InstanceDataBufferUploadKernels;

		[SerializeField]
		[ResourcePath("Runtime/RenderPipelineResources/GPUDriven/InstanceTransformUpdateKernels.compute", SearchType.ProjectPath)]
		private ComputeShader m_TransformUpdaterKernels;

		[SerializeField]
		[ResourcePath("Runtime/RenderPipelineResources/GPUDriven/InstanceWindDataUpdateKernels.compute", SearchType.ProjectPath)]
		public ComputeShader m_WindDataUpdaterKernels;

		[SerializeField]
		[ResourcePath("Runtime/RenderPipelineResources/GPUDriven/OccluderDepthPyramidKernels.compute", SearchType.ProjectPath)]
		private ComputeShader m_OccluderDepthPyramidKernels;

		[SerializeField]
		[ResourcePath("Runtime/RenderPipelineResources/GPUDriven/InstanceOcclusionCullingKernels.compute", SearchType.ProjectPath)]
		private ComputeShader m_InstanceOcclusionCullingKernels;

		[SerializeField]
		[ResourcePath("Runtime/RenderPipelineResources/GPUDriven/OcclusionCullingDebug.compute", SearchType.ProjectPath)]
		private ComputeShader m_OcclusionCullingDebugKernels;

		[SerializeField]
		[ResourcePath("Runtime/RenderPipelineResources/GPUDriven/DebugOcclusionTest.shader", SearchType.ProjectPath)]
		private Shader m_DebugOcclusionTestPS;

		[SerializeField]
		[ResourcePath("Runtime/RenderPipelineResources/GPUDriven/DebugOccluder.shader", SearchType.ProjectPath)]
		private Shader m_DebugOccluderPS;

		public enum Version
		{
			Initial,
			Count,
			Latest = 0
		}
	}
}
