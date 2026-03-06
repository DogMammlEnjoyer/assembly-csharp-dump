using System;
using UnityEngine.Categorization;

namespace UnityEngine.Rendering.Universal
{
	[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
	[CategoryInfo(Name = "Render Graph", Order = 50)]
	[ElementInfo(Order = -10)]
	[Serializable]
	public class RenderGraphSettings : IRenderPipelineGraphicsSettings
	{
		public int version
		{
			get
			{
				return (int)this.m_Version;
			}
		}

		bool IRenderPipelineGraphicsSettings.isAvailableInPlayerBuild
		{
			get
			{
				return true;
			}
		}

		public bool enableRenderCompatibilityMode
		{
			get
			{
				return this.m_EnableRenderCompatibilityMode && !RenderGraphGraphicsAutomatedTests.enabled;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_EnableRenderCompatibilityMode, value, "m_EnableRenderCompatibilityMode");
			}
		}

		[SerializeField]
		[HideInInspector]
		private RenderGraphSettings.Version m_Version;

		[SerializeField]
		[Tooltip("When enabled, URP does not use the Render Graph API to construct and execute the frame. Use this option only for compatibility purposes.")]
		[RecreatePipelineOnChange]
		private bool m_EnableRenderCompatibilityMode;

		internal enum Version
		{
			Initial
		}
	}
}
