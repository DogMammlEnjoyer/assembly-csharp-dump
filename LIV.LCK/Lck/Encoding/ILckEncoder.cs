using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Liv.Lck.Collections;
using Liv.NGFX;

namespace Liv.Lck.Encoding
{
	internal interface ILckEncoder : IDisposable
	{
		bool IsActive();

		bool IsPaused();

		LckResult StartEncoding(CameraTrackDescriptor cameraTrackDescriptor, IEnumerable<LckEncodedPacketHandler> encodedPacketHandlers);

		Task<LckResult> StopEncodingAsync();

		bool EncodeFrame(float videoTimeSeconds, AudioBuffer audioData, bool encodeVideo);

		void SetLogLevel(LogLevel logLevel);

		EncoderSessionData GetCurrentSessionData();
	}
}
