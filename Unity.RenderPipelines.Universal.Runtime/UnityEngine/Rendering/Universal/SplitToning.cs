using System;

namespace UnityEngine.Rendering.Universal
{
	[VolumeComponentMenu("Post-processing/Split Toning")]
	[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
	[Serializable]
	public sealed class SplitToning : VolumeComponent, IPostProcessComponent
	{
		public bool IsActive()
		{
			return this.shadows != Color.grey || this.highlights != Color.grey;
		}

		[Obsolete("Unused #from(2023.1)", false)]
		public bool IsTileCompatible()
		{
			return true;
		}

		[Tooltip("The color to use for shadows.")]
		public ColorParameter shadows = new ColorParameter(Color.grey, false, false, true, false);

		[Tooltip("The color to use for highlights.")]
		public ColorParameter highlights = new ColorParameter(Color.grey, false, false, true, false);

		[Tooltip("Balance between the colors in the highlights and shadows.")]
		public ClampedFloatParameter balance = new ClampedFloatParameter(0f, -100f, 100f, false);
	}
}
