using System;

namespace UnityEngine.Rendering.Universal
{
	[VolumeComponentMenu("Post-processing/Color Adjustments")]
	[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
	[Serializable]
	public sealed class ColorAdjustments : VolumeComponent, IPostProcessComponent
	{
		public bool IsActive()
		{
			return this.postExposure.value != 0f || this.contrast.value != 0f || this.colorFilter != Color.white || this.hueShift != 0f || this.saturation != 0f;
		}

		[Obsolete("Unused #from(2023.1)", false)]
		public bool IsTileCompatible()
		{
			return true;
		}

		[Tooltip("Adjusts the overall exposure of the scene in EV100. This is applied after HDR effect and right before tonemapping so it won't affect previous effects in the chain.")]
		public FloatParameter postExposure = new FloatParameter(0f, false);

		[Tooltip("Expands or shrinks the overall range of tonal values.")]
		public ClampedFloatParameter contrast = new ClampedFloatParameter(0f, -100f, 100f, false);

		[Tooltip("Tint the render by multiplying a color.")]
		public ColorParameter colorFilter = new ColorParameter(Color.white, true, false, true, false);

		[Tooltip("Shift the hue of all colors.")]
		public ClampedFloatParameter hueShift = new ClampedFloatParameter(0f, -180f, 180f, false);

		[Tooltip("Pushes the intensity of all colors.")]
		public ClampedFloatParameter saturation = new ClampedFloatParameter(0f, -100f, 100f, false);
	}
}
