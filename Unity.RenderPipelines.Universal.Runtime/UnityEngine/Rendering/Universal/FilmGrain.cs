using System;

namespace UnityEngine.Rendering.Universal
{
	[VolumeComponentMenu("Post-processing/Film Grain")]
	[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
	[Serializable]
	public sealed class FilmGrain : VolumeComponent, IPostProcessComponent
	{
		public bool IsActive()
		{
			return this.intensity.value > 0f && (this.type.value != FilmGrainLookup.Custom || this.texture.value != null);
		}

		[Obsolete("Unused #from(2023.1)", false)]
		public bool IsTileCompatible()
		{
			return true;
		}

		[Tooltip("The type of grain to use. You can select a preset or provide your own texture by selecting Custom.")]
		public FilmGrainLookupParameter type = new FilmGrainLookupParameter(FilmGrainLookup.Thin1, false);

		[Tooltip("Use the slider to set the strength of the Film Grain effect.")]
		public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f, false);

		[Tooltip("Controls the noisiness response curve based on scene luminance. Higher values mean less noise in light areas.")]
		public ClampedFloatParameter response = new ClampedFloatParameter(0.8f, 0f, 1f, false);

		[Tooltip("A tileable texture to use for the grain. The neutral value is 0.5 where no grain is applied.")]
		public NoInterpTextureParameter texture = new NoInterpTextureParameter(null, false);
	}
}
