using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
	internal class BottleneckHistory
	{
		public BottleneckHistory(int initialCapacity)
		{
			this.m_Bottlenecks.Capacity = initialCapacity;
		}

		internal void DiscardOldSamples(int historySize)
		{
			while (this.m_Bottlenecks.Count >= historySize)
			{
				this.m_Bottlenecks.RemoveAt(0);
			}
			this.m_Bottlenecks.Capacity = historySize;
		}

		internal void AddBottleneckFromAveragedSample(FrameTimeSample frameHistorySampleAverage)
		{
			PerformanceBottleneck item = BottleneckHistory.DetermineBottleneck(frameHistorySampleAverage);
			this.m_Bottlenecks.Add(item);
		}

		internal void ComputeHistogram()
		{
			BottleneckHistogram histogram = default(BottleneckHistogram);
			for (int i = 0; i < this.m_Bottlenecks.Count; i++)
			{
				switch (this.m_Bottlenecks[i])
				{
				case PerformanceBottleneck.PresentLimited:
					histogram.PresentLimited += 1f;
					break;
				case PerformanceBottleneck.CPU:
					histogram.CPU += 1f;
					break;
				case PerformanceBottleneck.GPU:
					histogram.GPU += 1f;
					break;
				case PerformanceBottleneck.Balanced:
					histogram.Balanced += 1f;
					break;
				}
			}
			histogram.Balanced /= (float)this.m_Bottlenecks.Count;
			histogram.CPU /= (float)this.m_Bottlenecks.Count;
			histogram.GPU /= (float)this.m_Bottlenecks.Count;
			histogram.PresentLimited /= (float)this.m_Bottlenecks.Count;
			this.Histogram = histogram;
		}

		private static PerformanceBottleneck DetermineBottleneck(FrameTimeSample s)
		{
			if (s.GPUFrameTime == 0f || s.MainThreadCPUFrameTime == 0f)
			{
				return PerformanceBottleneck.Indeterminate;
			}
			float num = 0.8f * s.FullFrameTime;
			if (s.GPUFrameTime > num && s.MainThreadCPUFrameTime < num && s.RenderThreadCPUFrameTime < num)
			{
				return PerformanceBottleneck.GPU;
			}
			if (s.GPUFrameTime < num && (s.MainThreadCPUFrameTime > num || s.RenderThreadCPUFrameTime > num))
			{
				return PerformanceBottleneck.CPU;
			}
			if (s.MainThreadCPUPresentWaitTime > 0.5f && s.GPUFrameTime < num && s.MainThreadCPUFrameTime < num && s.RenderThreadCPUFrameTime < num)
			{
				return PerformanceBottleneck.PresentLimited;
			}
			return PerformanceBottleneck.Balanced;
		}

		internal void Clear()
		{
			this.m_Bottlenecks.Clear();
			this.Histogram = default(BottleneckHistogram);
		}

		private List<PerformanceBottleneck> m_Bottlenecks = new List<PerformanceBottleneck>();

		internal BottleneckHistogram Histogram;
	}
}
