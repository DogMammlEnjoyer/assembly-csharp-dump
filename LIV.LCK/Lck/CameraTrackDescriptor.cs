using System;

namespace Liv.Lck
{
	[Serializable]
	public struct CameraTrackDescriptor
	{
		public CameraTrackDescriptor(CameraResolutionDescriptor cameraResolutionDescriptor, uint bitrate = 5242880U, uint framerate = 30U, uint audioBitrate = 192000U)
		{
			this.CameraResolutionDescriptor = cameraResolutionDescriptor;
			this.Bitrate = bitrate;
			this.Framerate = framerate;
			this.AudioBitrate = audioBitrate;
		}

		public CameraResolutionDescriptor CameraResolutionDescriptor;

		public uint Bitrate;

		public uint Framerate;

		public uint AudioBitrate;
	}
}
