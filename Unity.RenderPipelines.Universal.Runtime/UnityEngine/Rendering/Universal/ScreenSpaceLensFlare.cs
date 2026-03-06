using System;

namespace UnityEngine.Rendering.Universal
{
	[VolumeComponentMenu("Post-processing/Screen Space Lens Flare")]
	[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
	[Serializable]
	public class ScreenSpaceLensFlare : VolumeComponent, IPostProcessComponent
	{
		public ScreenSpaceLensFlare()
		{
			base.displayName = "Screen Space Lens Flare";
		}

		public bool IsActive()
		{
			return this.intensity.value > 0f;
		}

		public bool IsStreaksActive()
		{
			return this.streaksIntensity.value > 0f;
		}

		[Obsolete("Unused #from(2023.1)", false)]
		public bool IsTileCompatible()
		{
			return false;
		}

		public MinFloatParameter intensity = new MinFloatParameter(0f, 0f, false);

		public ColorParameter tintColor = new ColorParameter(Color.white, false);

		[AdditionalProperty]
		public ClampedIntParameter bloomMip = new ClampedIntParameter(1, 0, 5, false);

		[Header("Flares")]
		public MinFloatParameter firstFlareIntensity = new MinFloatParameter(1f, 0f, false);

		public MinFloatParameter secondaryFlareIntensity = new MinFloatParameter(1f, 0f, false);

		public MinFloatParameter warpedFlareIntensity = new MinFloatParameter(1f, 0f, false);

		[AdditionalProperty]
		public Vector2Parameter warpedFlareScale = new Vector2Parameter(new Vector2(1f, 1f), false);

		public ClampedIntParameter samples = new ClampedIntParameter(1, 1, 3, false);

		[AdditionalProperty]
		public ClampedFloatParameter sampleDimmer = new ClampedFloatParameter(0.5f, 0.1f, 1f, false);

		public ClampedFloatParameter vignetteEffect = new ClampedFloatParameter(1f, 0f, 1f, false);

		public ClampedFloatParameter startingPosition = new ClampedFloatParameter(1.25f, 1f, 3f, false);

		public ClampedFloatParameter scale = new ClampedFloatParameter(1.5f, 1f, 4f, false);

		[Header("Streaks")]
		public MinFloatParameter streaksIntensity = new MinFloatParameter(0f, 0f, false);

		public ClampedFloatParameter streaksLength = new ClampedFloatParameter(0.5f, 0f, 1f, false);

		public FloatParameter streaksOrientation = new FloatParameter(0f, false);

		public ClampedFloatParameter streaksThreshold = new ClampedFloatParameter(0.25f, 0f, 1f, false);

		[SerializeField]
		[AdditionalProperty]
		public ScreenSpaceLensFlareResolutionParameter resolution = new ScreenSpaceLensFlareResolutionParameter(ScreenSpaceLensFlareResolution.Quarter, false);

		[Header("Chromatic Abberation")]
		public ClampedFloatParameter chromaticAbberationIntensity = new ClampedFloatParameter(0.5f, 0f, 1f, false);
	}
}
