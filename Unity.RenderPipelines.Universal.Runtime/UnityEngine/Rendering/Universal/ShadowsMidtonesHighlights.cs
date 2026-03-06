using System;

namespace UnityEngine.Rendering.Universal
{
	[VolumeComponentMenu("Post-processing/Shadows, Midtones, Highlights")]
	[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
	[Serializable]
	public sealed class ShadowsMidtonesHighlights : VolumeComponent, IPostProcessComponent
	{
		public bool IsActive()
		{
			Vector4 rhs = new Vector4(1f, 1f, 1f, 0f);
			return this.shadows != rhs || this.midtones != rhs || this.highlights != rhs;
		}

		[Obsolete("Unused #from(2023.1)", false)]
		public bool IsTileCompatible()
		{
			return true;
		}

		public Vector4Parameter shadows = new Vector4Parameter(new Vector4(1f, 1f, 1f, 0f), false);

		public Vector4Parameter midtones = new Vector4Parameter(new Vector4(1f, 1f, 1f, 0f), false);

		public Vector4Parameter highlights = new Vector4Parameter(new Vector4(1f, 1f, 1f, 0f), false);

		[Header("Shadow Limits")]
		[Tooltip("Start point of the transition between shadows and midtones.")]
		public MinFloatParameter shadowsStart = new MinFloatParameter(0f, 0f, false);

		[Tooltip("End point of the transition between shadows and midtones.")]
		public MinFloatParameter shadowsEnd = new MinFloatParameter(0.3f, 0f, false);

		[Header("Highlight Limits")]
		[Tooltip("Start point of the transition between midtones and highlights.")]
		public MinFloatParameter highlightsStart = new MinFloatParameter(0.55f, 0f, false);

		[Tooltip("End point of the transition between midtones and highlights.")]
		public MinFloatParameter highlightsEnd = new MinFloatParameter(1f, 0f, false);
	}
}
