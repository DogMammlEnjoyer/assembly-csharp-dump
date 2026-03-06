using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.Rendering
{
	public class DebugFrameTiming
	{
		public int bottleneckHistorySize { get; set; } = 60;

		public int sampleHistorySize { get; set; } = 30;

		public DebugFrameTiming()
		{
			this.m_FrameHistory = new FrameTimeSampleHistory(this.sampleHistorySize);
			this.m_BottleneckHistory = new BottleneckHistory(this.bottleneckHistorySize);
		}

		public void UpdateFrameTiming()
		{
			this.m_Timing[0] = default(FrameTiming);
			this.m_Sample = default(FrameTimeSample);
			FrameTimingManager.CaptureFrameTimings();
			FrameTimingManager.GetLatestTimings(1U, this.m_Timing);
			if (this.m_Timing.Length != 0)
			{
				this.m_Sample.FullFrameTime = (float)this.m_Timing.First<FrameTiming>().cpuFrameTime;
				this.m_Sample.FramesPerSecond = ((this.m_Sample.FullFrameTime > 0f) ? (1000f / this.m_Sample.FullFrameTime) : 0f);
				this.m_Sample.MainThreadCPUFrameTime = (float)this.m_Timing.First<FrameTiming>().cpuMainThreadFrameTime;
				this.m_Sample.MainThreadCPUPresentWaitTime = (float)this.m_Timing.First<FrameTiming>().cpuMainThreadPresentWaitTime;
				this.m_Sample.RenderThreadCPUFrameTime = (float)this.m_Timing.First<FrameTiming>().cpuRenderThreadFrameTime;
				this.m_Sample.GPUFrameTime = (float)this.m_Timing.First<FrameTiming>().gpuFrameTime;
			}
			this.m_FrameHistory.DiscardOldSamples(this.sampleHistorySize);
			this.m_FrameHistory.Add(this.m_Sample);
			this.m_FrameHistory.ComputeAggregateValues();
			this.m_BottleneckHistory.DiscardOldSamples(this.bottleneckHistorySize);
			this.m_BottleneckHistory.AddBottleneckFromAveragedSample(this.m_FrameHistory.SampleAverage);
			this.m_BottleneckHistory.ComputeHistogram();
		}

		public void RegisterDebugUI(List<DebugUI.Widget> list)
		{
			list.Add(new DebugUI.Foldout
			{
				displayName = "Frame Stats",
				opened = true,
				columnLabels = new string[]
				{
					"Avg",
					"Min",
					"Max"
				},
				children = 
				{
					new DebugUI.ValueTuple
					{
						displayName = "Frame Rate (FPS)",
						values = new DebugUI.Value[]
						{
							new DebugUI.Value
							{
								refreshRate = 0.2f,
								formatString = "{0:F1}",
								getter = (() => this.m_FrameHistory.SampleAverage.FramesPerSecond)
							},
							new DebugUI.Value
							{
								refreshRate = 0.2f,
								formatString = "{0:F1}",
								getter = (() => this.m_FrameHistory.SampleMin.FramesPerSecond)
							},
							new DebugUI.Value
							{
								refreshRate = 0.2f,
								formatString = "{0:F1}",
								getter = (() => this.m_FrameHistory.SampleMax.FramesPerSecond)
							}
						}
					},
					new DebugUI.ValueTuple
					{
						displayName = "Frame Time",
						values = new DebugUI.Value[]
						{
							new DebugUI.Value
							{
								refreshRate = 0.2f,
								formatString = "{0:F2}ms",
								getter = (() => this.m_FrameHistory.SampleAverage.FullFrameTime)
							},
							new DebugUI.Value
							{
								refreshRate = 0.2f,
								formatString = "{0:F2}ms",
								getter = (() => this.m_FrameHistory.SampleMin.FullFrameTime)
							},
							new DebugUI.Value
							{
								refreshRate = 0.2f,
								formatString = "{0:F2}ms",
								getter = (() => this.m_FrameHistory.SampleMax.FullFrameTime)
							}
						}
					},
					new DebugUI.ValueTuple
					{
						displayName = "CPU Main Thread Frame",
						values = new DebugUI.Value[]
						{
							new DebugUI.Value
							{
								refreshRate = 0.2f,
								formatString = "{0:F2}ms",
								getter = (() => this.m_FrameHistory.SampleAverage.MainThreadCPUFrameTime)
							},
							new DebugUI.Value
							{
								refreshRate = 0.2f,
								formatString = "{0:F2}ms",
								getter = (() => this.m_FrameHistory.SampleMin.MainThreadCPUFrameTime)
							},
							new DebugUI.Value
							{
								refreshRate = 0.2f,
								formatString = "{0:F2}ms",
								getter = (() => this.m_FrameHistory.SampleMax.MainThreadCPUFrameTime)
							}
						}
					},
					new DebugUI.ValueTuple
					{
						displayName = "CPU Render Thread Frame",
						values = new DebugUI.Value[]
						{
							new DebugUI.Value
							{
								refreshRate = 0.2f,
								formatString = "{0:F2}ms",
								getter = (() => this.m_FrameHistory.SampleAverage.RenderThreadCPUFrameTime)
							},
							new DebugUI.Value
							{
								refreshRate = 0.2f,
								formatString = "{0:F2}ms",
								getter = (() => this.m_FrameHistory.SampleMin.RenderThreadCPUFrameTime)
							},
							new DebugUI.Value
							{
								refreshRate = 0.2f,
								formatString = "{0:F2}ms",
								getter = (() => this.m_FrameHistory.SampleMax.RenderThreadCPUFrameTime)
							}
						}
					},
					new DebugUI.ValueTuple
					{
						displayName = "CPU Present Wait",
						values = new DebugUI.Value[]
						{
							new DebugUI.Value
							{
								refreshRate = 0.2f,
								formatString = "{0:F2}ms",
								getter = (() => this.m_FrameHistory.SampleAverage.MainThreadCPUPresentWaitTime)
							},
							new DebugUI.Value
							{
								refreshRate = 0.2f,
								formatString = "{0:F2}ms",
								getter = (() => this.m_FrameHistory.SampleMin.MainThreadCPUPresentWaitTime)
							},
							new DebugUI.Value
							{
								refreshRate = 0.2f,
								formatString = "{0:F2}ms",
								getter = (() => this.m_FrameHistory.SampleMax.MainThreadCPUPresentWaitTime)
							}
						}
					},
					new DebugUI.ValueTuple
					{
						displayName = "GPU Frame",
						values = new DebugUI.Value[]
						{
							new DebugUI.Value
							{
								refreshRate = 0.2f,
								formatString = "{0:F2}ms",
								getter = (() => this.m_FrameHistory.SampleAverage.GPUFrameTime)
							},
							new DebugUI.Value
							{
								refreshRate = 0.2f,
								formatString = "{0:F2}ms",
								getter = (() => this.m_FrameHistory.SampleMin.GPUFrameTime)
							},
							new DebugUI.Value
							{
								refreshRate = 0.2f,
								formatString = "{0:F2}ms",
								getter = (() => this.m_FrameHistory.SampleMax.GPUFrameTime)
							}
						}
					}
				}
			});
			list.Add(new DebugUI.Foldout
			{
				displayName = "Bottlenecks",
				children = 
				{
					new DebugUI.ProgressBarValue
					{
						displayName = "CPU",
						getter = (() => this.m_BottleneckHistory.Histogram.CPU)
					},
					new DebugUI.ProgressBarValue
					{
						displayName = "GPU",
						getter = (() => this.m_BottleneckHistory.Histogram.GPU)
					},
					new DebugUI.ProgressBarValue
					{
						displayName = "Present limited",
						getter = (() => this.m_BottleneckHistory.Histogram.PresentLimited)
					},
					new DebugUI.ProgressBarValue
					{
						displayName = "Balanced",
						getter = (() => this.m_BottleneckHistory.Histogram.Balanced)
					}
				}
			});
		}

		internal void Reset()
		{
			this.m_BottleneckHistory.Clear();
			this.m_FrameHistory.Clear();
		}

		private const string k_FpsFormatString = "{0:F1}";

		private const string k_MsFormatString = "{0:F2}ms";

		private const float k_RefreshRate = 0.2f;

		internal FrameTimeSampleHistory m_FrameHistory;

		internal BottleneckHistory m_BottleneckHistory;

		private FrameTiming[] m_Timing = new FrameTiming[1];

		private FrameTimeSample m_Sample;
	}
}
