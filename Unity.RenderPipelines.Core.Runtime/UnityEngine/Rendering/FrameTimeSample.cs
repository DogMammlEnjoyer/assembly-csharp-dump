using System;

namespace UnityEngine.Rendering
{
	internal struct FrameTimeSample
	{
		internal FrameTimeSample(float initValue)
		{
			this.FramesPerSecond = initValue;
			this.FullFrameTime = initValue;
			this.MainThreadCPUFrameTime = initValue;
			this.MainThreadCPUPresentWaitTime = initValue;
			this.RenderThreadCPUFrameTime = initValue;
			this.GPUFrameTime = initValue;
		}

		internal float FramesPerSecond;

		internal float FullFrameTime;

		internal float MainThreadCPUFrameTime;

		internal float MainThreadCPUPresentWaitTime;

		internal float RenderThreadCPUFrameTime;

		internal float GPUFrameTime;
	}
}
