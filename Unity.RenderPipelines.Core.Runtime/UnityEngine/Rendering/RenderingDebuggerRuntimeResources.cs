using System;
using UnityEngine.Categorization;

namespace UnityEngine.Rendering
{
	[HideInInspector]
	[SupportedOnRenderPipeline(new Type[]
	{

	})]
	[CategoryInfo(Name = "R : Rendering Debugger Resources", Order = 100)]
	[ElementInfo(Order = 0)]
	[Serializable]
	internal class RenderingDebuggerRuntimeResources : IRenderPipelineResources, IRenderPipelineGraphicsSettings
	{
		int IRenderPipelineGraphicsSettings.version
		{
			get
			{
				return (int)this.m_version;
			}
		}

		[SerializeField]
		[HideInInspector]
		private RenderingDebuggerRuntimeResources.Version m_version;

		private enum Version
		{
			Initial,
			Count,
			Last = 0
		}
	}
}
