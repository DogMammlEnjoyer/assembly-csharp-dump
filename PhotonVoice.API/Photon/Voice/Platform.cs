using System;
using Photon.Voice.Windows;

namespace Photon.Voice
{
	public static class Platform
	{
		public static IDeviceEnumerator CreateAudioInEnumerator(ILogger logger)
		{
			return new AudioInEnumerator(logger);
		}

		public static IAudioInChangeNotifier CreateAudioInChangeNotifier(Action callback, ILogger logger)
		{
			return new AudioInChangeNotifierNotSupported(callback, logger);
		}

		public static IEncoder CreateDefaultAudioEncoder<T>(ILogger logger, VoiceInfo info)
		{
			Codec codec = info.Codec;
			if (codec == Codec.Raw)
			{
				return new RawCodec.Encoder<T>();
			}
			if (codec == Codec.AudioOpus)
			{
				return OpusCodec.Factory.CreateEncoder<T[]>(info, logger);
			}
			throw new UnsupportedCodecException("Platform.CreateDefaultAudioEncoder", info.Codec);
		}

		public static IAudioDesc CreateDefaultAudioSource(ILogger logger, DeviceInfo dev, int samplingRate, int channels, object otherParams = null)
		{
			return new WindowsAudioInPusher(dev.IsDefault ? -1 : dev.IDInt, logger);
		}
	}
}
