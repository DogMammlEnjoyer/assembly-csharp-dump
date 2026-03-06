using System;
using Liv.NGFX;
using UnityEngine.Scripting;

namespace Liv.Lck.Streaming
{
	internal class NullLckStreamer : ILckStreamer, ILckCaptureStateProvider, IDisposable
	{
		public LckResult<bool> IsPaused()
		{
			return LckResult<bool>.NewError(LckError.StreamerNotImplemented, "LCK: No valid streamer module has been loaded. Ensure tv.liv.lck-streaming is installed.");
		}

		public LckCaptureState CurrentCaptureState
		{
			get
			{
				return LckCaptureState.Idle;
			}
		}

		[Preserve]
		public NullLckStreamer()
		{
		}

		public void Dispose()
		{
		}

		public bool IsStreaming
		{
			get
			{
				return false;
			}
		}

		public LckResult StartStreaming()
		{
			return LckResult.NewError(LckError.StreamerNotImplemented, "LCK: No valid streamer module has been loaded. Ensure tv.liv.lck-streaming is installed.");
		}

		public LckResult StopStreaming(LckService.StopReason stopReason)
		{
			return LckResult.NewError(LckError.StreamerNotImplemented, "LCK: No valid streamer module has been loaded. Ensure tv.liv.lck-streaming is installed.");
		}

		public LckResult<TimeSpan> GetStreamDuration()
		{
			return LckResult<TimeSpan>.NewError(LckError.StreamerNotImplemented, "LCK: No valid streamer module has been loaded. Ensure tv.liv.lck-streaming is installed.");
		}

		public void SetLogLevel(LogLevel logLevel)
		{
		}
	}
}
