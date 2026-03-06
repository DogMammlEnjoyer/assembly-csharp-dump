using System;

namespace UnityEngine.Rendering.Universal
{
	public class UniversalRenderPipelineDebugDisplaySettings : DebugDisplaySettings<UniversalRenderPipelineDebugDisplaySettings>
	{
		private DebugDisplaySettingsCommon commonSettings { get; set; }

		public DebugDisplaySettingsMaterial materialSettings { get; private set; }

		public DebugDisplaySettingsRendering renderingSettings { get; private set; }

		public DebugDisplaySettingsLighting lightingSettings { get; private set; }

		public DebugDisplaySettingsVolume volumeSettings { get; private set; }

		internal DebugDisplaySettingsStats<URPProfileId> displayStats { get; private set; }

		internal DebugDisplayGPUResidentDrawer gpuResidentDrawerSettings { get; private set; }

		public override bool IsPostProcessingAllowed
		{
			get
			{
				DebugPostProcessingMode postProcessingDebugMode = this.renderingSettings.postProcessingDebugMode;
				switch (postProcessingDebugMode)
				{
				case DebugPostProcessingMode.Disabled:
					return false;
				case DebugPostProcessingMode.Auto:
				{
					bool flag = true;
					foreach (IDebugDisplaySettingsData debugDisplaySettingsData in this.m_Settings)
					{
						flag &= debugDisplaySettingsData.IsPostProcessingAllowed;
					}
					return flag;
				}
				case DebugPostProcessingMode.Enabled:
					return true;
				default:
					throw new ArgumentOutOfRangeException("debugPostProcessingMode", string.Format("Invalid post-processing state {0}", postProcessingDebugMode));
				}
			}
		}

		public override void Reset()
		{
			base.Reset();
			this.displayStats = base.Add<DebugDisplaySettingsStats<URPProfileId>>(new DebugDisplaySettingsStats<URPProfileId>(new UniversalRenderPipelineDebugDisplayStats()));
			this.materialSettings = base.Add<DebugDisplaySettingsMaterial>(new DebugDisplaySettingsMaterial());
			this.lightingSettings = base.Add<DebugDisplaySettingsLighting>(new DebugDisplaySettingsLighting());
			this.renderingSettings = base.Add<DebugDisplaySettingsRendering>(new DebugDisplaySettingsRendering());
			this.volumeSettings = base.Add<DebugDisplaySettingsVolume>(new DebugDisplaySettingsVolume());
			this.commonSettings = base.Add<DebugDisplaySettingsCommon>(new DebugDisplaySettingsCommon());
			this.gpuResidentDrawerSettings = base.Add<DebugDisplayGPUResidentDrawer>(new DebugDisplayGPUResidentDrawer());
			Texture.streamingTextureDiscardUnusedMips = false;
		}

		internal void UpdateDisplayStats()
		{
			if (this.displayStats != null)
			{
				this.displayStats.debugDisplayStats.Update();
			}
		}

		internal void UpdateMaterials()
		{
			if (this.renderingSettings.mipInfoMode != DebugMipInfoMode.None)
			{
				Texture.SetStreamingTextureMaterialDebugProperties((this.renderingSettings.canAggregateData && this.renderingSettings.showInfoForAllSlots) ? -1 : this.renderingSettings.mipDebugMaterialTextureSlot);
			}
		}
	}
}
