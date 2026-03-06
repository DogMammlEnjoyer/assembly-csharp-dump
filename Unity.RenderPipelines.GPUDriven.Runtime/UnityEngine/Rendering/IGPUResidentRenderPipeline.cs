using System;

namespace UnityEngine.Rendering
{
	public interface IGPUResidentRenderPipeline
	{
		GPUResidentDrawerSettings gpuResidentDrawerSettings { get; }

		GPUResidentDrawerMode gpuResidentDrawerMode { get; set; }

		public static void ReinitializeGPUResidentDrawer()
		{
			GPUResidentDrawer.Reinitialize();
		}

		bool IsGPUResidentDrawerSupportedBySRP(bool logReason = false)
		{
			string message;
			LogType severity;
			bool flag = this.IsGPUResidentDrawerSupportedBySRP(out message, out severity);
			if (logReason && !flag)
			{
				GPUResidentDrawer.LogMessage(message, severity);
			}
			return flag;
		}

		bool IsGPUResidentDrawerSupportedBySRP(out string message, out LogType severity)
		{
			message = string.Empty;
			severity = LogType.Log;
			return true;
		}

		public static bool IsGPUResidentDrawerSupportedByProjectConfiguration(bool logReason = false)
		{
			string text;
			LogType logType;
			bool result = GPUResidentDrawer.IsProjectSupported(out text, out logType);
			if (logReason && !string.IsNullOrEmpty(text))
			{
				Debug.LogWarning(text);
			}
			return result;
		}

		public static bool IsGPUResidentDrawerEnabled()
		{
			return GPUResidentDrawer.IsEnabled();
		}
	}
}
