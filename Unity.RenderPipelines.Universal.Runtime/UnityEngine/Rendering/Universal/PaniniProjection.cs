using System;

namespace UnityEngine.Rendering.Universal
{
	[VolumeComponentMenu("Post-processing/Panini Projection")]
	[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
	[Serializable]
	public sealed class PaniniProjection : VolumeComponent, IPostProcessComponent
	{
		public bool IsActive()
		{
			return this.distance.value > 0f;
		}

		[Obsolete("Unused #from(2023.1)", false)]
		public bool IsTileCompatible()
		{
			return false;
		}

		[Tooltip("Panini projection distance.")]
		public ClampedFloatParameter distance = new ClampedFloatParameter(0f, 0f, 1f, false);

		[Tooltip("Panini projection crop to fit.")]
		public ClampedFloatParameter cropToFit = new ClampedFloatParameter(1f, 0f, 1f, false);
	}
}
