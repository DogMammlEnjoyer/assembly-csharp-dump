using System;

namespace UnityEngine.Rendering.Universal
{
	public class UniversalPostProcessingData : ContextItem
	{
		public override void Reset()
		{
			this.isEnabled = false;
			this.gradingMode = ColorGradingMode.LowDynamicRange;
			this.lutSize = 0;
			this.useFastSRGBLinearConversion = false;
			this.supportScreenSpaceLensFlare = false;
			this.supportDataDrivenLensFlare = false;
		}

		public bool isEnabled;

		public ColorGradingMode gradingMode;

		public int lutSize;

		public bool useFastSRGBLinearConversion;

		public bool supportScreenSpaceLensFlare;

		public bool supportDataDrivenLensFlare;
	}
}
