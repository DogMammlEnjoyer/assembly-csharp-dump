using System;
using System.Runtime.InteropServices;
using Liv.Lck.Encoding;
using Liv.NGFX;

namespace Liv.Lck.Streaming
{
	internal class LckNativeStreamingService : ILckNativeStreamingService
	{
		[DllImport("qck_streaming")]
		private static extern IntPtr CreateStreamer();

		[DllImport("qck_streaming")]
		private static extern void DestroyStreamer(IntPtr streamerContext);

		[DllImport("qck_streaming")]
		private static extern bool StartStreamer(IntPtr streamerContext, int width, int height);

		[DllImport("qck_streaming")]
		private static extern void StopStreamer(IntPtr streamerContext);

		[DllImport("qck_streaming")]
		private static extern void SetStreamerLogLevel(IntPtr streamerContext, uint level);

		[DllImport("qck_streaming")]
		private static extern IntPtr GetStreamerCallbackFunction();

		[DllImport("qck_streaming")]
		private static extern IntPtr SetPacketInterleaverEnabled(IntPtr streamerContext, bool enabled);

		public bool CreateNativeStreamer()
		{
			this._streamerContext = LckNativeStreamingService.CreateStreamer();
			if (!this.HasNativeStreamer())
			{
				return false;
			}
			this.UpdateNativeStreamerLogLevel();
			return true;
		}

		public void DestroyNativeStreamer()
		{
			if (!this.HasNativeStreamer())
			{
				return;
			}
			LckNativeStreamingService.DestroyStreamer(this._streamerContext);
			this._streamerContext = IntPtr.Zero;
		}

		public bool HasNativeStreamer()
		{
			return this._streamerContext != IntPtr.Zero;
		}

		public bool StartNativeStreamer(int width, int height)
		{
			return LckNativeStreamingService.StartStreamer(this._streamerContext, width, height);
		}

		public bool StopNativeStreamer()
		{
			LckNativeStreamingService.StopStreamer(this._streamerContext);
			return true;
		}

		public void SetNativeStreamerLogLevel(LogLevel logLevel)
		{
			this._logLevel = logLevel;
			if (this.HasNativeStreamer())
			{
				this.UpdateNativeStreamerLogLevel();
			}
		}

		public LckEncodedPacketCallback GetStreamPacketCallback()
		{
			return new LckEncodedPacketCallback(this._streamerContext, LckNativeStreamingService.GetStreamerCallbackFunction());
		}

		private void UpdateNativeStreamerLogLevel()
		{
			LckNativeStreamingService.SetStreamerLogLevel(this._streamerContext, (uint)this._logLevel);
		}

		private const string StreamingLib = "qck_streaming";

		private IntPtr _streamerContext = IntPtr.Zero;

		private LogLevel _logLevel = LogLevel.Error;
	}
}
