using System;
using Liv.Lck.Encoding;
using Liv.NGFX;

namespace Liv.Lck.Streaming
{
	internal interface ILckNativeStreamingService
	{
		bool CreateNativeStreamer();

		void DestroyNativeStreamer();

		bool HasNativeStreamer();

		bool StartNativeStreamer(int width, int height);

		bool StopNativeStreamer();

		void SetNativeStreamerLogLevel(LogLevel logLevel);

		LckEncodedPacketCallback GetStreamPacketCallback();
	}
}
