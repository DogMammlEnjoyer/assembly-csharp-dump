using System;

namespace Photon.Voice
{
	public class AudioDesc : IAudioDesc, IDisposable
	{
		public AudioDesc(int samplingRate, int channels, string error)
		{
			this.SamplingRate = samplingRate;
			this.Channels = channels;
			this.Error = error;
		}

		public int SamplingRate { get; private set; }

		public int Channels { get; private set; }

		public string Error { get; private set; }

		public void Dispose()
		{
		}
	}
}
