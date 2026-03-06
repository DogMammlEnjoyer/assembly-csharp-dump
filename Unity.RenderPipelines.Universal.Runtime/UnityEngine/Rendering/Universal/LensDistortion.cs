using System;

namespace UnityEngine.Rendering.Universal
{
	[VolumeComponentMenu("Post-processing/Lens Distortion")]
	[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
	[Serializable]
	public sealed class LensDistortion : VolumeComponent, IPostProcessComponent
	{
		public bool IsActive()
		{
			return Mathf.Abs(this.intensity.value) > 0f && (this.xMultiplier.value > 0f || this.yMultiplier.value > 0f);
		}

		[Obsolete("Unused #from(2023.1)", false)]
		public bool IsTileCompatible()
		{
			return false;
		}

		[Tooltip("Total distortion amount.")]
		public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, -1f, 1f, false);

		[Tooltip("Intensity multiplier on X axis. Set it to 0 to disable distortion on this axis.")]
		public ClampedFloatParameter xMultiplier = new ClampedFloatParameter(1f, 0f, 1f, false);

		[Tooltip("Intensity multiplier on Y axis. Set it to 0 to disable distortion on this axis.")]
		public ClampedFloatParameter yMultiplier = new ClampedFloatParameter(1f, 0f, 1f, false);

		[Tooltip("Distortion center point. 0.5,0.5 is center of the screen.")]
		public Vector2Parameter center = new Vector2Parameter(new Vector2(0.5f, 0.5f), false);

		[Tooltip("Controls global screen scaling for the distortion effect. Use this to hide the screen borders when using high \"Intensity.\"")]
		public ClampedFloatParameter scale = new ClampedFloatParameter(1f, 0.01f, 5f, false);
	}
}
