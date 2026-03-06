using System;

namespace UnityEngine.Rendering.Universal
{
	[VolumeComponentMenu("Post-processing/Lift, Gamma, Gain")]
	[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
	[Serializable]
	public sealed class LiftGammaGain : VolumeComponent, IPostProcessComponent
	{
		public bool IsActive()
		{
			Vector4 rhs = new Vector4(1f, 1f, 1f, 0f);
			return this.lift != rhs || this.gamma != rhs || this.gain != rhs;
		}

		[Obsolete("Unused #from(2023.1)", false)]
		public bool IsTileCompatible()
		{
			return true;
		}

		public Vector4Parameter lift = new Vector4Parameter(new Vector4(1f, 1f, 1f, 0f), false);

		public Vector4Parameter gamma = new Vector4Parameter(new Vector4(1f, 1f, 1f, 0f), false);

		public Vector4Parameter gain = new Vector4Parameter(new Vector4(1f, 1f, 1f, 0f), false);
	}
}
