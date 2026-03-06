using System;

namespace UnityEngine.Rendering
{
	public abstract class RenderPipelineGlobalSettings<TGlobalRenderPipelineSettings, TRenderPipeline> : RenderPipelineGlobalSettings where TGlobalRenderPipelineSettings : RenderPipelineGlobalSettings where TRenderPipeline : RenderPipeline
	{
		public static TGlobalRenderPipelineSettings instance
		{
			get
			{
				return RenderPipelineGlobalSettings<TGlobalRenderPipelineSettings, TRenderPipeline>.s_Instance.Value;
			}
		}

		public virtual void Reset()
		{
		}

		private static Lazy<TGlobalRenderPipelineSettings> s_Instance = new Lazy<TGlobalRenderPipelineSettings>(() => GraphicsSettings.GetSettingsForRenderPipeline<TRenderPipeline>() as TGlobalRenderPipelineSettings);
	}
}
