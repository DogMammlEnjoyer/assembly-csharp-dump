using System;
using Liv.Lck.Encoding;
using Liv.NGFX;

namespace Liv.Lck.Recorder
{
	internal interface ILckNativeRecordingService
	{
		bool CreateNativeMuxer();

		void DestroyNativeMuxer();

		bool HasNativeMuxer();

		bool StartNativeMuxer(ref MuxerConfig config);

		bool StopNativeMuxer();

		void SetNativeMuxerLogLevel(LogLevel logLevel);

		LckEncodedPacketCallback GetMuxPacketCallback();
	}
}
