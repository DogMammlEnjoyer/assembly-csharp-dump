using System;

namespace Photon.Voice
{
	public class AudioInChangeNotifierNotSupported : IAudioInChangeNotifier, IDisposable
	{
		public bool IsSupported
		{
			get
			{
				return false;
			}
		}

		public AudioInChangeNotifierNotSupported(Action callback, ILogger logger)
		{
		}

		public string Error
		{
			get
			{
				return "Current platform is not supported by AudioInChangeNotifier.";
			}
		}

		public void Dispose()
		{
		}
	}
}
