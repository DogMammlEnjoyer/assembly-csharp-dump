using System;

namespace UnityEngine.InputSystem.LowLevel
{
	[Serializable]
	public struct InputMetrics
	{
		public int maxNumDevices { readonly get; set; }

		public int currentNumDevices { readonly get; set; }

		public int maxStateSizeInBytes { readonly get; set; }

		public int currentStateSizeInBytes { readonly get; set; }

		public int currentControlCount { readonly get; set; }

		public int currentLayoutCount { readonly get; set; }

		public int totalEventBytes { readonly get; set; }

		public int totalEventCount { readonly get; set; }

		public int totalUpdateCount { readonly get; set; }

		public double totalEventProcessingTime { readonly get; set; }

		public double totalEventLagTime { readonly get; set; }

		public float averageEventBytesPerFrame
		{
			get
			{
				return (float)this.totalEventBytes / (float)this.totalUpdateCount;
			}
		}

		public double averageProcessingTimePerEvent
		{
			get
			{
				return this.totalEventProcessingTime / (double)this.totalEventCount;
			}
		}

		public double averageLagTimePerEvent
		{
			get
			{
				return this.totalEventLagTime / (double)this.totalEventCount;
			}
		}
	}
}
