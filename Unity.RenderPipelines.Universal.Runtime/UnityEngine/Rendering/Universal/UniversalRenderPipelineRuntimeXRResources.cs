using System;
using UnityEngine.Categorization;

namespace UnityEngine.Rendering.Universal
{
	[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
	[CategoryInfo(Name = "R: Runtime XR", Order = 1000)]
	[HideInInspector]
	[Serializable]
	public class UniversalRenderPipelineRuntimeXRResources : IRenderPipelineResources, IRenderPipelineGraphicsSettings
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

		public Shader xrOcclusionMeshPS
		{
			get
			{
				return this.m_xrOcclusionMeshPS;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_xrOcclusionMeshPS, value, "m_xrOcclusionMeshPS");
			}
		}

		public Shader xrMirrorViewPS
		{
			get
			{
				return this.m_xrMirrorViewPS;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_xrMirrorViewPS, value, "m_xrMirrorViewPS");
			}
		}

		public Shader xrMotionVector
		{
			get
			{
				return this.m_xrMotionVector;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_xrMotionVector, value, "m_xrMotionVector");
			}
		}

		internal bool valid
		{
			get
			{
				return !(this.xrOcclusionMeshPS == null) && !(this.xrMirrorViewPS == null) && !(this.m_xrMotionVector == null);
			}
		}

		[SerializeField]
		[ResourcePath("Shaders/XR/XROcclusionMesh.shader", SearchType.ProjectPath)]
		private Shader m_xrOcclusionMeshPS;

		[SerializeField]
		[ResourcePath("Shaders/XR/XRMirrorView.shader", SearchType.ProjectPath)]
		private Shader m_xrMirrorViewPS;

		[SerializeField]
		[ResourcePath("Shaders/XR/XRMotionVector.shader", SearchType.ProjectPath)]
		private Shader m_xrMotionVector;
	}
}
