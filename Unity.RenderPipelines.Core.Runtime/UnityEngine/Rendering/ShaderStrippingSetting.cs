using System;
using UnityEngine.Categorization;

namespace UnityEngine.Rendering
{
	[SupportedOnRenderPipeline(new Type[]
	{

	})]
	[CategoryInfo(Name = "Additional Shader Stripping Settings", Order = 40)]
	[ElementInfo(Order = 0)]
	[Serializable]
	public class ShaderStrippingSetting : IRenderPipelineGraphicsSettings
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

		public bool exportShaderVariants
		{
			get
			{
				return this.m_ExportShaderVariants;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_ExportShaderVariants, value, "exportShaderVariants");
			}
		}

		public ShaderVariantLogLevel shaderVariantLogLevel
		{
			get
			{
				return this.m_ShaderVariantLogLevel;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_ShaderVariantLogLevel, value, "shaderVariantLogLevel");
			}
		}

		public bool stripRuntimeDebugShaders
		{
			get
			{
				return this.m_StripRuntimeDebugShaders;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_StripRuntimeDebugShaders, value, "stripRuntimeDebugShaders");
			}
		}

		[SerializeField]
		[HideInInspector]
		private ShaderStrippingSetting.Version m_Version;

		[SerializeField]
		[Tooltip("Controls whether to output shader variant information to a file.")]
		private bool m_ExportShaderVariants = true;

		[SerializeField]
		[Tooltip("Controls the level of logging of shader variant information outputted during the build process. Information appears in the Unity Console when the build finishes.")]
		private ShaderVariantLogLevel m_ShaderVariantLogLevel;

		[SerializeField]
		[Tooltip("When enabled, all debug display shader variants are removed when you build for the Unity Player. This decreases build time, but prevents the use of most Rendering Debugger features in Player builds.")]
		private bool m_StripRuntimeDebugShaders = true;

		internal enum Version
		{
			Initial
		}
	}
}
