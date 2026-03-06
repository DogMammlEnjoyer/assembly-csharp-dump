using System;

namespace Liv.Lck.Encoding
{
	internal struct LckEncodedPacketHandler
	{
		public readonly ILckCaptureStateProvider CaptureStateProvider { get; }

		public readonly LckEncodedPacketCallback EncodedPacketCallback { get; }

		public LckEncodedPacketHandler(ILckCaptureStateProvider captureStateProvider, LckEncodedPacketCallback encodedPacketCallback)
		{
			this.CaptureStateProvider = captureStateProvider;
			this.EncodedPacketCallback = encodedPacketCallback;
		}
	}
}
