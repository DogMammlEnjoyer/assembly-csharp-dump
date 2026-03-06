using System;

namespace UnityEngine.Rendering.Universal
{
	public struct PostProcessingData
	{
		internal PostProcessingData(ContextContainer frameData)
		{
			this.frameData = frameData;
		}

		internal UniversalPostProcessingData universalPostProcessingData
		{
			get
			{
				return this.frameData.Get<UniversalPostProcessingData>();
			}
		}

		public ref ColorGradingMode gradingMode
		{
			get
			{
				return ref this.frameData.Get<UniversalPostProcessingData>().gradingMode;
			}
		}

		public ref int lutSize
		{
			get
			{
				return ref this.frameData.Get<UniversalPostProcessingData>().lutSize;
			}
		}

		public ref bool useFastSRGBLinearConversion
		{
			get
			{
				return ref this.frameData.Get<UniversalPostProcessingData>().useFastSRGBLinearConversion;
			}
		}

		public ref bool supportScreenSpaceLensFlare
		{
			get
			{
				return ref this.frameData.Get<UniversalPostProcessingData>().supportScreenSpaceLensFlare;
			}
		}

		public ref bool supportDataDrivenLensFlare
		{
			get
			{
				return ref this.frameData.Get<UniversalPostProcessingData>().supportDataDrivenLensFlare;
			}
		}

		private ContextContainer frameData;
	}
}
