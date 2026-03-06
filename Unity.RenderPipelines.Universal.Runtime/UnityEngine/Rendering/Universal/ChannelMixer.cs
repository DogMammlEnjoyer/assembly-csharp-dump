using System;

namespace UnityEngine.Rendering.Universal
{
	[VolumeComponentMenu("Post-processing/Channel Mixer")]
	[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
	[Serializable]
	public sealed class ChannelMixer : VolumeComponent, IPostProcessComponent
	{
		public bool IsActive()
		{
			return this.redOutRedIn.value != 100f || this.redOutGreenIn.value != 0f || this.redOutBlueIn.value != 0f || this.greenOutRedIn.value != 0f || this.greenOutGreenIn.value != 100f || this.greenOutBlueIn.value != 0f || this.blueOutRedIn.value != 0f || this.blueOutGreenIn.value != 0f || this.blueOutBlueIn.value != 100f;
		}

		[Obsolete("Unused #from(2023.1)", false)]
		public bool IsTileCompatible()
		{
			return true;
		}

		[Tooltip("Modify influence of the red channel in the overall mix.")]
		public ClampedFloatParameter redOutRedIn = new ClampedFloatParameter(100f, -200f, 200f, false);

		[Tooltip("Modify influence of the green channel in the overall mix.")]
		public ClampedFloatParameter redOutGreenIn = new ClampedFloatParameter(0f, -200f, 200f, false);

		[Tooltip("Modify influence of the blue channel in the overall mix.")]
		public ClampedFloatParameter redOutBlueIn = new ClampedFloatParameter(0f, -200f, 200f, false);

		[Tooltip("Modify influence of the red channel in the overall mix.")]
		public ClampedFloatParameter greenOutRedIn = new ClampedFloatParameter(0f, -200f, 200f, false);

		[Tooltip("Modify influence of the green channel in the overall mix.")]
		public ClampedFloatParameter greenOutGreenIn = new ClampedFloatParameter(100f, -200f, 200f, false);

		[Tooltip("Modify influence of the blue channel in the overall mix.")]
		public ClampedFloatParameter greenOutBlueIn = new ClampedFloatParameter(0f, -200f, 200f, false);

		[Tooltip("Modify influence of the red channel in the overall mix.")]
		public ClampedFloatParameter blueOutRedIn = new ClampedFloatParameter(0f, -200f, 200f, false);

		[Tooltip("Modify influence of the green channel in the overall mix.")]
		public ClampedFloatParameter blueOutGreenIn = new ClampedFloatParameter(0f, -200f, 200f, false);

		[Tooltip("Modify influence of the blue channel in the overall mix.")]
		public ClampedFloatParameter blueOutBlueIn = new ClampedFloatParameter(100f, -200f, 200f, false);
	}
}
