using System;
using System.Runtime.InteropServices;
using Liv.Lck.Encoding;
using Liv.NGFX;
using UnityEngine.Scripting;

namespace Liv.Lck.Recorder
{
	internal class LckNativeRecordingService : ILckNativeRecordingService
	{
		[DllImport("qck")]
		private static extern IntPtr GetMuxerCallbackFunction();

		[DllImport("qck")]
		private static extern IntPtr CreateMuxer();

		[DllImport("qck")]
		private static extern void DestroyMuxer(IntPtr muxerContext);

		[DllImport("qck")]
		private static extern bool StartMuxer(IntPtr muxerContext, ref MuxerConfig config);

		[DllImport("qck")]
		private static extern void StopMuxer(IntPtr muxerContext);

		[DllImport("qck")]
		private static extern void SetMuxerLogLevel(IntPtr muxerContext, uint level);

		[Preserve]
		public LckNativeRecordingService()
		{
		}

		public bool CreateNativeMuxer()
		{
			this._nativeMuxerContext = LckNativeRecordingService.CreateMuxer();
			if (!this.HasNativeMuxer())
			{
				return false;
			}
			this.UpdateNativeMuxerLogLevel();
			return true;
		}

		public void DestroyNativeMuxer()
		{
			if (!this.HasNativeMuxer())
			{
				return;
			}
			LckNativeRecordingService.DestroyMuxer(this._nativeMuxerContext);
			this._nativeMuxerContext = IntPtr.Zero;
		}

		public bool HasNativeMuxer()
		{
			return this._nativeMuxerContext != IntPtr.Zero;
		}

		public bool StartNativeMuxer(ref MuxerConfig config)
		{
			return LckNativeRecordingService.StartMuxer(this._nativeMuxerContext, ref config);
		}

		public bool StopNativeMuxer()
		{
			LckNativeRecordingService.StopMuxer(this._nativeMuxerContext);
			return true;
		}

		public void SetNativeMuxerLogLevel(LogLevel logLevel)
		{
			this._logLevel = logLevel;
			if (this.HasNativeMuxer())
			{
				this.UpdateNativeMuxerLogLevel();
			}
		}

		public LckEncodedPacketCallback GetMuxPacketCallback()
		{
			return new LckEncodedPacketCallback(this._nativeMuxerContext, LckNativeRecordingService.GetMuxerCallbackFunction());
		}

		private void UpdateNativeMuxerLogLevel()
		{
			LckNativeRecordingService.SetMuxerLogLevel(this._nativeMuxerContext, (uint)this._logLevel);
		}

		private const string RecordingLib = "qck";

		private IntPtr _nativeMuxerContext = IntPtr.Zero;

		private LogLevel _logLevel = LogLevel.Error;
	}
}
