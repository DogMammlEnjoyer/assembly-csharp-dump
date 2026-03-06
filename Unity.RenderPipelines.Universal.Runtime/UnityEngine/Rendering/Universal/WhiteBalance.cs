using System;

namespace UnityEngine.Rendering.Universal
{
	[VolumeComponentMenu("Post-processing/White Balance")]
	[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
	[Serializable]
	public sealed class WhiteBalance : VolumeComponent, IPostProcessComponent
	{
		public bool IsActive()
		{
			return this.temperature.value != 0f || this.tint.value != 0f;
		}

		[Obsolete("Unused #from(2023.1)", false)]
		public bool IsTileCompatible()
		{
			return true;
		}

		[Tooltip("Sets the white balance to a custom color temperature.")]
		public ClampedFloatParameter temperature = new ClampedFloatParameter(0f, -100f, 100f, false);

		[Tooltip("Sets the white balance to compensate for a green or magenta tint.")]
		public ClampedFloatParameter tint = new ClampedFloatParameter(0f, -100f, 100f, false);
	}
}
