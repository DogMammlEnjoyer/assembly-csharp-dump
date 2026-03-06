using System;
using UnityEngine.Categorization;

namespace UnityEngine.Rendering
{
	[SupportedOnRenderPipeline(new Type[]
	{

	})]
	[CategoryInfo(Name = "Render Graph", Order = 50)]
	[ElementInfo(Order = 0)]
	[Serializable]
	public class RenderGraphGlobalSettings : IRenderPipelineGraphicsSettings
	{
		bool IRenderPipelineGraphicsSettings.isAvailableInPlayerBuild
		{
			get
			{
				return true;
			}
		}

		int IRenderPipelineGraphicsSettings.version
		{
			get
			{
				return (int)this.m_version;
			}
		}

		public bool enableCompilationCaching
		{
			get
			{
				return this.m_EnableCompilationCaching;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_EnableCompilationCaching, value, "enableCompilationCaching");
			}
		}

		public bool enableValidityChecks
		{
			get
			{
				return this.m_EnableValidityChecks;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_EnableValidityChecks, value, "enableValidityChecks");
			}
		}

		[SerializeField]
		[HideInInspector]
		private RenderGraphGlobalSettings.Version m_version;

		[RecreatePipelineOnChange]
		[SerializeField]
		[Tooltip("Enable caching of render graph compilation from one frame to another.")]
		private bool m_EnableCompilationCaching = true;

		[RecreatePipelineOnChange]
		[SerializeField]
		[Tooltip("Enable validity checks of render graph in Editor and Development mode. Always disabled in Release build.")]
		private bool m_EnableValidityChecks = true;

		private enum Version
		{
			Initial,
			Count,
			Last = 0
		}
	}
}
