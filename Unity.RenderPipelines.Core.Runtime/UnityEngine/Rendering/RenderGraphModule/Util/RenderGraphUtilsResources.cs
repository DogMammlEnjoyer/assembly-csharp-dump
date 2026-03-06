using System;
using System.ComponentModel;

namespace UnityEngine.Rendering.RenderGraphModule.Util
{
	[HideInInspector]
	[Category("Resources/Render Graph Helper Function Resources")]
	[SupportedOnRenderPipeline(new Type[]
	{

	})]
	[Serializable]
	internal class RenderGraphUtilsResources : IRenderPipelineResources, IRenderPipelineGraphicsSettings
	{
		int IRenderPipelineGraphicsSettings.version
		{
			get
			{
				return (int)this.m_Version;
			}
		}

		public Shader coreCopyPS
		{
			get
			{
				return this.m_CoreCopyPS;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_CoreCopyPS, value, "m_CoreCopyPS");
			}
		}

		[SerializeField]
		[HideInInspector]
		private RenderGraphUtilsResources.Version m_Version;

		[SerializeField]
		[ResourcePath("Shaders/CoreCopy.shader", SearchType.ProjectPath)]
		internal Shader m_CoreCopyPS;

		public enum Version
		{
			Initial,
			Count,
			Latest = 0
		}
	}
}
