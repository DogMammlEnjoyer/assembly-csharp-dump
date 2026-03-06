using System;

namespace Meta.Voice.TelemetryUtilities.PerformanceTracing
{
	public static class VsdkProfiler
	{
		public static void BeginSample(string sampleName)
		{
			if (VsdkProfiler.profilingEnabled)
			{
				VsdkProfiler.traceProvider.BeginSample(sampleName);
			}
		}

		public static void EndSample(string sampleName)
		{
			if (VsdkProfiler.profilingEnabled)
			{
				VsdkProfiler.traceProvider.EndSample(sampleName);
			}
		}

		public static ITraceProvider traceProvider = new UnityProfilerTraceProvider();

		public static bool profilingEnabled = false;
	}
}
