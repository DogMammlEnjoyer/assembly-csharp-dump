using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering
{
	internal class FrameTimeSampleHistory
	{
		public FrameTimeSampleHistory(int initialCapacity)
		{
			this.m_Samples.Capacity = initialCapacity;
		}

		internal void Add(FrameTimeSample sample)
		{
			this.m_Samples.Add(sample);
		}

		internal void ComputeAggregateValues()
		{
			FrameTimeSample sampleAverage = default(FrameTimeSample);
			FrameTimeSample sampleMin = new FrameTimeSample(float.MaxValue);
			FrameTimeSample sampleMax = new FrameTimeSample(float.MinValue);
			FrameTimeSample sample = default(FrameTimeSample);
			for (int i = 0; i < this.m_Samples.Count; i++)
			{
				FrameTimeSample sample2 = this.m_Samples[i];
				FrameTimeSampleHistory.<ComputeAggregateValues>g__ForEachSampleMember|12_0(ref sampleMin, sample2, FrameTimeSampleHistory.s_SampleValueMin);
				FrameTimeSampleHistory.<ComputeAggregateValues>g__ForEachSampleMember|12_0(ref sampleMax, sample2, FrameTimeSampleHistory.s_SampleValueMax);
				FrameTimeSampleHistory.<ComputeAggregateValues>g__ForEachSampleMember|12_0(ref sampleAverage, sample2, FrameTimeSampleHistory.s_SampleValueAdd);
				FrameTimeSampleHistory.<ComputeAggregateValues>g__ForEachSampleMember|12_0(ref sample, sample2, FrameTimeSampleHistory.s_SampleValueCountValid);
			}
			FrameTimeSampleHistory.<ComputeAggregateValues>g__ForEachSampleMember|12_0(ref sampleMin, sample, FrameTimeSampleHistory.s_SampleValueEnsureValid);
			FrameTimeSampleHistory.<ComputeAggregateValues>g__ForEachSampleMember|12_0(ref sampleMax, sample, FrameTimeSampleHistory.s_SampleValueEnsureValid);
			FrameTimeSampleHistory.<ComputeAggregateValues>g__ForEachSampleMember|12_0(ref sampleAverage, sample, FrameTimeSampleHistory.s_SampleValueDivide);
			this.SampleAverage = sampleAverage;
			this.SampleMin = sampleMin;
			this.SampleMax = sampleMax;
		}

		internal void DiscardOldSamples(int sampleHistorySize)
		{
			while (this.m_Samples.Count >= sampleHistorySize)
			{
				this.m_Samples.RemoveAt(0);
			}
			this.m_Samples.Capacity = sampleHistorySize;
		}

		internal void Clear()
		{
			this.m_Samples.Clear();
		}

		[CompilerGenerated]
		internal static void <ComputeAggregateValues>g__ForEachSampleMember|12_0(ref FrameTimeSample aggregate, FrameTimeSample sample, Func<float, float, float> func)
		{
			aggregate.FramesPerSecond = func(aggregate.FramesPerSecond, sample.FramesPerSecond);
			aggregate.FullFrameTime = func(aggregate.FullFrameTime, sample.FullFrameTime);
			aggregate.MainThreadCPUFrameTime = func(aggregate.MainThreadCPUFrameTime, sample.MainThreadCPUFrameTime);
			aggregate.MainThreadCPUPresentWaitTime = func(aggregate.MainThreadCPUPresentWaitTime, sample.MainThreadCPUPresentWaitTime);
			aggregate.RenderThreadCPUFrameTime = func(aggregate.RenderThreadCPUFrameTime, sample.RenderThreadCPUFrameTime);
			aggregate.GPUFrameTime = func(aggregate.GPUFrameTime, sample.GPUFrameTime);
		}

		private List<FrameTimeSample> m_Samples = new List<FrameTimeSample>();

		internal FrameTimeSample SampleAverage;

		internal FrameTimeSample SampleMin;

		internal FrameTimeSample SampleMax;

		private static Func<float, float, float> s_SampleValueAdd = (float value, float other) => value + other;

		private static Func<float, float, float> s_SampleValueMin = delegate(float value, float other)
		{
			if (other <= 0f)
			{
				return value;
			}
			return Mathf.Min(value, other);
		};

		private static Func<float, float, float> s_SampleValueMax = (float value, float other) => Mathf.Max(value, other);

		private static Func<float, float, float> s_SampleValueCountValid = delegate(float value, float other)
		{
			if (other <= 0f)
			{
				return value;
			}
			return value + 1f;
		};

		private static Func<float, float, float> s_SampleValueEnsureValid = delegate(float value, float other)
		{
			if (other <= 0f)
			{
				return 0f;
			}
			return value;
		};

		private static Func<float, float, float> s_SampleValueDivide = delegate(float value, float other)
		{
			if (other <= 0f)
			{
				return 0f;
			}
			return value / other;
		};
	}
}
